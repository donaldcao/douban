using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework.Media;
using PanoramaApp2.Resources;
using PanoramaApp2.Utility;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace PanoramaApp2
{
    public partial class ImagePage : PhoneApplicationPage
    {
        private int imageIndex, pivotIndex;
        private ApplicationBarIconButton saveButton;
        private ApplicationBarIconButton viewCommentsButton;
        private ApplicationBarMenuItem setMenue;
        private ObservableCollection<MovieImage> imageCollection;
        private MovieImage currentImage;
        private int pivotNum;
        private PanoramaItem[] panoramaItems;
        private Image[] images;
        private ProgressBar[] progressBars;
        private Grid[] grids;
        private object ob = new object();

        public ImagePage()
        {
            InitializeComponent();
            imageCollection = App.imageCollectionPassed;
            imageIndex = imageCollection.IndexOf(App.imagePassed);
            pivotIndex = 0;
            currentImage = null;
            if (imageCollection.Count == 1)
            {
                pivotNum = 1;
            }
            else
            {
                pivotNum = 3;
            }
            panoramaItems = new PanoramaItem[pivotNum];
            images = new Image[pivotNum];
            progressBars = new ProgressBar[pivotNum];
            grids = new Grid[pivotNum];

            for (int i = 0; i < pivotNum; i++)
            {
                panoramaItems[i] = new PanoramaItem();
                images[i] = new Image();
                images[i].Tap += onImageTap;
                progressBars[i] = new ProgressBar();
                progressBars[i].Foreground = new SolidColorBrush(Colors.White);
                progressBars[i].IsIndeterminate = true;
                progressBars[i].Visibility = System.Windows.Visibility.Collapsed;
                grids[i] = new Grid();
                grids[i].Children.Add(progressBars[i]);
                grids[i].Children.Add(images[i]);

                panoramaItems[i].Content = grids[i];
                panorama.Items.Add(panoramaItems[i]);
            }

            panorama.SelectionChanged += Pivot_SelectionChanged;

            createApplicationBar();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.fromTombStone)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    await loadImage();
                }
            }
        }

        /// <summary>
        /// Pivot selection change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lock (ob)
            {
                int i = ((Panorama)sender).SelectedIndex;

                int preImageIndex, nextImageIndex, prePivotIndex, nextPivotIndex;

                // Left swipe
                if (pivotIndex == (i + 1) % pivotNum)
                {
                    imageIndex = (imageIndex == 0 ? imageCollection.Count - 1 : imageIndex - 1);
                }
                //Right swipe
                else if (i == (pivotIndex + 1) % pivotNum)
                {
                    imageIndex = (imageIndex + 1) % imageCollection.Count;
                }
                preImageIndex = (imageIndex == 0 ? imageCollection.Count - 1 : imageIndex - 1);
                nextImageIndex = (imageIndex + 1) % imageCollection.Count;
                prePivotIndex = (i == 0 ? pivotNum - 1 : i - 1);
                nextPivotIndex = (i + 1) % pivotNum;
                pivotIndex = i;
                if (imageCollection[preImageIndex].bitMap != null)
                {
                    images[prePivotIndex].Source = imageCollection[preImageIndex].bitMap;
                }
                else
                {
                    images[prePivotIndex].Source = null;
                }
                if (imageCollection[nextImageIndex].bitMap != null)
                {
                    images[nextPivotIndex].Source = imageCollection[nextImageIndex].bitMap;
                }
                else
                {
                    images[nextPivotIndex].Source = null;
                }
            }
            await loadImage();
        }

        /// <summary>
        /// Create application bar
        /// </summary>
        private void createApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0.9;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            saveButton = new ApplicationBarIconButton();
            saveButton.IconUri = new Uri("/Assets/save.png", UriKind.Relative);
            saveButton.Text = AppResources.SaveMenu;
            saveButton.Click += saveMenu_Click;
            ApplicationBar.Buttons.Add(saveButton);


            /*
            commentMenu = new ApplicationBarMenuItem(AppResources.CommentMenu);
            commentMenu.Click += commentMenu_Click;
            ApplicationBar.MenuItems.Add(commentMenu);
             */

            viewCommentsButton = new ApplicationBarIconButton();
            viewCommentsButton.IconUri = new Uri("/Assets/comments.png", UriKind.Relative);
            viewCommentsButton.Text = AppResources.CommentMenu;
            viewCommentsButton.Click += viewCommentButton_Click;
            ApplicationBar.Buttons.Add(viewCommentsButton);
        
            setMenue = new ApplicationBarMenuItem(AppResources.SetLockScreenMenu);
            setMenue.Click += setLockScreenMenu_Click;
            ApplicationBar.MenuItems.Add(setMenue);
        }

        /// <summary>
        /// Image tap event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ApplicationBar.IsVisible == true)
            {
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        /// <summary>
        /// Set lock screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void setLockScreenMenu_Click(object sender, EventArgs e)
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {

                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }

                if (isProvider)
                {
                    MessageBoxResult value = MessageBox.Show(AppResources.SetMessage, "", MessageBoxButton.OKCancel);
                    if (value == MessageBoxResult.OK)
                    {
                        if (imageCollection[imageIndex].bitMap == null)
                        {
                            MessageBox.Show(AppResources.SetFail);
                        }
                        else
                        {
                            WriteableBitmap wb = new WriteableBitmap(imageCollection[imageIndex].bitMap);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                wb.SaveJpeg(ms, imageCollection[imageIndex].bitMap.PixelWidth, imageCollection[imageIndex].bitMap.PixelHeight, 0, 100);
                                // This is important!!!
                                ms.Seek(0, SeekOrigin.Begin);
                                IsolatedStorageFile IsoStore = IsolatedStorageFile.GetUserStoreForApplication();
                                using (IsolatedStorageFileStream storageStream = IsoStore.CreateFile("LockScreenImage"))
                                {
                                    ms.CopyTo(storageStream);
                                }
                                //var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                                var uri = new Uri("ms-appdata:///Local/" + "LockScreenImage", UriKind.Absolute);
                                Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                                MessageBox.Show(AppResources.SetSuccess);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        
        void viewCommentButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                App.commentImagePassed = currentImage;
                NavigationService.Navigate(new Uri("/ImageCommentPage.xaml", UriKind.Relative));
            }
        }
        

        private void saveMenu_Click(object sender, EventArgs e)
        {
            MessageBoxResult value = MessageBox.Show(AppResources.SaveMessage, "", MessageBoxButton.OKCancel);
            if (value == MessageBoxResult.OK)
            {
                if (imageCollection[imageIndex].bitMap == null)
                {
                    MessageBox.Show(AppResources.SaveFail);
                }
                else
                {
                    WriteableBitmap wb = new WriteableBitmap(imageCollection[imageIndex].bitMap);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveJpeg(ms, imageCollection[imageIndex].bitMap.PixelWidth, imageCollection[imageIndex].bitMap.PixelHeight, 0, 100);
                        MediaLibrary lib = new MediaLibrary();
                        // This is important!!!
                        ms.Seek(0, SeekOrigin.Begin);
                        lib.SavePictureToCameraRoll(Guid.NewGuid().ToString(), ms);
                        MessageBox.Show(AppResources.SaveSuccess);
                    }
                }
            }
        }

        /// <summary>
        /// Load image
        /// </summary>
        /// <returns></returns>
        private async Task loadImage()
        {
            currentImage = imageCollection[imageIndex];
            if (imageCollection[imageIndex].bitMap != null)
            {
                images[pivotIndex].Source = imageCollection[imageIndex].bitMap;
            }
            else
            {
                progressBars[pivotIndex].Visibility = System.Windows.Visibility.Visible;
                Downloader downloader = new Downloader(imageCollection[imageIndex].largeUrl);
                try
                {
                    byte[] byteArray = await downloader.downloadByteArray();
                    MemoryStream ms = new MemoryStream(byteArray);
                    imageCollection[imageIndex].bitMap = new BitmapImage();
                    imageCollection[imageIndex].bitMap.CreateOptions = BitmapCreateOptions.None;
                    imageCollection[imageIndex].bitMap.SetSource(ms);
                    images[pivotIndex].Source = imageCollection[imageIndex].bitMap;
                }
                catch (Exception)
                {
                    imageCollection[imageIndex].bitMap = null;
                }
                progressBars[pivotIndex].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}