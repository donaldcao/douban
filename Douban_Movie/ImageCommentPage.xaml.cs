using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PanoramaApp2.HtmlParser;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using PanoramaApp2.Resources;
using PanoramaApp2.Utility;

namespace PanoramaApp2
{
    public partial class ImageCommentPage : PhoneApplicationPage
    {
        private Popup searchPopup;
        private MovieImage image;
        private ImageCommentHtmlParser parser;
        bool commentNewLoad = false;

        public ImageCommentPage()
        {
            InitializeComponent();
            searchPopup = new Popup();
            image = App.commentImagePassed;
            if (image != null)
            {
                parser = new ImageCommentHtmlParser(image);
            }

            var commentPullDector = new WP8PullToRefreshDetector();
            commentPullDector.Bind(commentSelector);
            commentPullDector.Compression += commentDector_Compress;

            commentSelector.ItemRealized += comment_ItemRealized;
            LayoutRoot.DataContext = Settings.instance;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (parser != null)
                {
                    parser.cancelDownload();
                }
            }
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
                    await loadComment();
                }
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (searchPopup.IsOpen)
            {
                searchPopup.IsOpen = false;
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        /// <summary>
        /// Comment item realized handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comment_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (commentNewLoad)
            {
                commentSelector.ScrollTo(e.Container.Content);
                commentNewLoad = false;
            }
        }

        ///////////////////////////////////////////////////////// Pull to refresh event handler///////////////////////////////////////////////
        /// <summary>
        /// Short review pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void commentDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (parser != null && image != null)
                {
                    await loadMoreComment();
                }
            }
        }

        private async Task loadComment()
        {
            bool fromDormant = false;
            commentProgressBar.IsIndeterminate = true;
            commentProgressBar.Visibility = System.Windows.Visibility.Visible;
            try
            {
                await parser.getComments();
                commentSelector.ItemsSource = parser.commentCollection;
                if (commentSelector.ItemsSource.Count == 0)
                {
                    commentTextBlock.Visibility = System.Windows.Visibility.Visible;
                }
                commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    if (!parser.isCanceled())
                    {
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }

            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadComment();
            }
        }

        private async Task loadMoreComment()
        {
            if (!parser.hasMoreComments)
            {
                noCommentTextBlock.Visibility = System.Windows.Visibility.Visible;
                await Task.Delay(2000);
                noCommentTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                bool fromDormant = false;
                commentProgressBar.IsIndeterminate = true;
                commentProgressBar.Visibility = System.Windows.Visibility.Visible;
                commentNewLoad = true;

                try
                {
                    await parser.loadMore();
                    commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!parser.isCanceled())
                        {
                            MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        commentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreComment();
                }
            }
        }

        /// <summary>
        /// Open search box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PopupInput input = new PopupInput();
            input.Width = Application.Current.Host.Content.ActualWidth;
            input.Height = Application.Current.Host.Content.ActualHeight;
            searchPopup.Child = input;
            searchPopup.IsOpen = true;
            input.inputBox.Focus();
        }
    }
}