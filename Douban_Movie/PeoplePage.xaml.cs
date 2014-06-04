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
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using PanoramaApp2.Resources;
using PanoramaApp2.Utility;

namespace PanoramaApp2
{
    public partial class PeoplePage : PhoneApplicationPage
    {
        private People people = null;
        private bool movieLoaded = false;
        private bool imageLoaded = false;
        private bool peopleLoaded = false;
        private Popup searchPopup;
        private PeopleHtmlParser peopleParser = null;
        private PeopleMovieHtmlParser peopleMovieParser = null;
        private PeopleImageHtmlParser peopleImageParser = null;
        private bool imageNewLoad = false;
        private bool movieNewLoad = false;

        public PeoplePage()
        {
            InitializeComponent();
            searchPopup = new Popup();
            people = App.peoplePassed;
            if (people != null)
            {
                People p = Cache.getPeople(people.id);
                if (p != null)
                {
                    people = p;
                }
                peopleParser = new PeopleHtmlParser(people);
            }

            var moviePullDector = new WP8PullToRefreshDetector();
            moviePullDector.Bind(movieSelector);
            moviePullDector.Compression += movieDector_Compress;
            var imagePullDector = new WP8PullToRefreshDetector();
            imagePullDector.Bind(imageSelector);
            imagePullDector.Compression += imageDector_Compress;

            imageSelector.ItemRealized += image_ItemRealized;
            movieSelector.ItemRealized += movie_ItemRealized;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.fromTombStone)
            {
                NavigationService.Navigate(new Uri("/MainPage.xmal", UriKind.Relative));
            }
            else
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    /*
                    if (peopleParser != null)
                    {
                        //loadPeople();
                    }
                    */
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (peopleMovieParser != null)
                {
                    peopleMovieParser.cancelDownload();
                }
                if (peopleImageParser != null)
                {
                    peopleImageParser.cancelDownload();
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
        /// Image item realized handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void image_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (imageNewLoad)
            {
                imageSelector.ScrollTo(e.Container.Content);
                imageNewLoad = false;
            }
        }

        /// <summary>
        /// Image item realized handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void movie_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (movieNewLoad)
            {
                movieSelector.ScrollTo(e.Container.Content);
                movieNewLoad = false;
            }
        }

        /// <summary>
        /// Movie pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void movieDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (peopleMovieParser != null)
                {
                    await loadMoreMovie();
                }
            }
        }

        /// <summary>
        /// Movie pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void imageDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (peopleImageParser != null)
                {
                    await loadMoreImage();
                }
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((Pivot)sender).SelectedIndex;

            if (index == 0)
            {
                if (peopleLoaded == false)
                {
                    peopleLoaded = true;
                    await loadPeople();
                }
            }
            if (index == 1)
            {
                if (movieLoaded == false)
                {
                    movieLoaded = true;
                    await loadMovie();
                }
            }
            if (index == 2)
            {
                if (imageLoaded == false)
                {
                    imageLoaded = true;
                    await loadImage();
                }
            }
        }

        /// <summary>
        /// Load people 
        /// </summary>
        private async Task loadPeople()
        {
            PeopleProgressBar.IsIndeterminate = true;
            PeopleProgressBar.Visibility = System.Windows.Visibility.Visible;
            bool fromDormant = false;
            try
            {
                PeopleGrid.DataContext = await peopleParser.getPeople();
                if (people.gender != "")
                {
                    genderStackPanel.Visibility = Visibility.Visible;
                }
                if (people.birthday != "")
                {
                    birthdayStackPanel.Visibility = Visibility.Visible;
                }
                if (people.birthplace != "")
                {
                    birthplaceStackPanel.Visibility = Visibility.Visible;
                }
                if (people.constl != "")
                {
                    constStackPanel.Visibility = Visibility.Visible;
                }
                if (people.occupation != "")
                {
                    occupationStackPanel.Visibility = Visibility.Visible;
                }
                PeopleProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    PeopleProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    if (!peopleParser.isCanceled())
                    {
                        peopleLoaded = false;
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
                    PeopleProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    peopleLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }

            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadPeople();
            }
        }

        /// <summary>
        /// Load movie
        /// </summary>
        private async Task loadMovie()
        {
            if (people != null)
            {
                bool fromDormant = false;
                peopleMovieParser = new PeopleMovieHtmlParser(people);
                movieProgressBar.IsIndeterminate = true;
                movieProgressBar.Visibility = System.Windows.Visibility.Visible;
                movieNewLoad = true;

                try
                {
                    await peopleMovieParser.getMovie();
                    movieSelector.ItemsSource = peopleMovieParser.movieCollection;
                    if (movieSelector.ItemsSource.Count == 0)
                    {
                        movieTextBlock.Visibility = System.Windows.Visibility.Visible;
                    }
                    movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!peopleMovieParser.isCanceled())
                        {
                            movieLoaded = false;
                            MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception)
                {
                    movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    movieLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMovie();
                }
            }
        }

        /// <summary>
        /// Load more movie
        /// </summary>
        /// <returns></returns>
        private async Task loadMoreMovie()
        {
            if (!peopleMovieParser.hasMore)
            {
                noMovieTextBlock.Visibility = System.Windows.Visibility.Visible;
                await Task.Delay(2000);
                noMovieTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else {
                bool fromDormant = false;
                movieProgressBar.IsIndeterminate = true;
                movieProgressBar.Visibility = System.Windows.Visibility.Visible;
                movieNewLoad = true;

                try
                {
                    await peopleMovieParser.loadMore();
                    movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!peopleMovieParser.isCanceled())
                        {
                            MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception)
                {
                    movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreMovie();
                }
            }
        }
        /// <summary>
        /// Load image
        /// </summary>
        private async Task loadImage()
        {
            if (people != null)
            {
                bool fromDormant = false;
                peopleImageParser = new PeopleImageHtmlParser(people);
                ImageProgressBar.IsIndeterminate = true;
                ImageProgressBar.Visibility = System.Windows.Visibility.Visible;
                imageNewLoad = true;

                try
                {
                    await peopleImageParser.getImage();
                    imageSelector.ItemsSource = peopleImageParser.imageCollection;
                    if (imageSelector.ItemsSource.Count == 0)
                    {
                        imageTextBlock.Visibility = System.Windows.Visibility.Visible;
                    }
                    ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!peopleImageParser.isCanceled())
                        {
                            imageLoaded = false;
                            MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception)
                {
                    movieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    imageLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadImage();
                }
            }
        }

        /// <summary>
        /// Load more image
        /// </summary>
        /// <returns></returns>
        private async Task loadMoreImage()
        {
            if (!peopleImageParser.hasMore)
            {
                noImageTextBlock.Visibility = System.Windows.Visibility.Visible;
                await Task.Delay(2000);
                noImageTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else {
                bool fromDormant = false;
                ImageProgressBar.IsIndeterminate = true;
                ImageProgressBar.Visibility = System.Windows.Visibility.Visible;
                imageNewLoad = true;

                try
                {
                    await peopleImageParser.loadMore();
                    ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!peopleImageParser.isCanceled())
                        {
                            MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                        }
                    }
                }
                catch (Exception)
                {
                    ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreImage();
                }
            }
        }
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PopupInput input = new PopupInput();
            input.Width = Application.Current.Host.Content.ActualWidth;
            input.Height = Application.Current.Host.Content.ActualHeight;
            searchPopup.Child = input;
            searchPopup.IsOpen = true;
            input.inputBox.Focus();
        }

        private void imageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageSelector != null && imageSelector.SelectedItem != null)
            {
                MovieImage image = (MovieImage)imageSelector.SelectedItem;
                if (image != null)
                {
                    App.imagePassed = image;
                    App.imageCollectionPassed = peopleImageParser.imageCollection;
                    NavigationService.Navigate(new Uri("/ImagePage.xaml", UriKind.Relative));
                }
                imageSelector.SelectedItem = null;
            }
        }

        private void movieSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (movieSelector != null && movieSelector.SelectedItem != null)
            {
                Movie m = (Movie)movieSelector.SelectedItem;
                if (m != null && m.id != string.Empty)
                {
                    App.moviePassed = m;
                    NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                }
                movieSelector.SelectedItem = null;
            }
        }
    }
}