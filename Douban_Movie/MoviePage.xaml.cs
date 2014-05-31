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
using System.Windows.Controls.Primitives;
using PanoramaApp2.HtmlParser;
using PanoramaApp2.Utility;
using System.Threading.Tasks;
using PanoramaApp2.Resources;
using System.Collections.ObjectModel;

namespace PanoramaApp2
{
    public partial class MoviePage : PhoneApplicationPage
    {
        private MovieJsonParser movieParser = null;
        private Popup searchPopup;
        private bool shortReviewLoaded;
        private bool reviewLoaded;
        private bool imageLoaded;
        private bool movieLoaded;
        private Movie movie = null;
        private ShortReviewHtmlParser shortReviewParser = null;
        private ReviewParser reviewParser = null;
        private ImageHtmlParser imageParser = null;

        public MoviePage()
        {
            InitializeComponent();
            searchPopup = new Popup();
            shortReviewLoaded = false;
            reviewLoaded = false;
            imageLoaded = false;
            movieLoaded = false;

            // Pull to refresh handle
            var shortReviewPullDector = new WP8PullToRefreshDetector();
            shortReviewPullDector.Bind(shortReviewSelector);
            shortReviewPullDector.Compression += shortReviewDector_Compress;
            var reviewPullDector = new WP8PullToRefreshDetector();
            reviewPullDector.Bind(reviewLongListSelector);
            reviewPullDector.Compression += reviewDector_Compress;
            var imagePullDector = new WP8PullToRefreshDetector();
            imagePullDector.Bind(imageSelector);
            imagePullDector.Compression += imageDector_Compress;

            movie = App.moviePassed;
            if (movie != null)
            { 
                movieParser = new MovieJsonParser(movie);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
                    //await loadMovie();
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

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (movieParser != null)
                {
                    movieParser.cancelDownload();
                }
                if (shortReviewParser != null)
                {
                    shortReviewParser.cancelDownload();
                }
                if (reviewParser != null)
                {
                    reviewParser.cancelDownload();
                }
                if (imageParser != null)
                {
                    imageParser.cancelDownload();
                }
            }
            base.OnNavigatedFrom(e);
        }

        ///////////////////////////////////////////////////////// Pull to refresh event handler///////////////////////////////////////////////
        /// <summary>
        /// Short review pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void shortReviewDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (shortReviewParser != null && movie != null)
                {
                    await loadMoreShortReview();
                }
            }
        }

        /// <summary>
        /// Review pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void reviewDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (reviewParser != null && movie != null)
                {
                    await loadMoreReview();
                }
            }
        }

        /// <summary>
        /// Image pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void imageDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (imageParser != null && movie != null)
                {
                    await loadMoreImage();
                }
            }
        }

        ///////////////////////////////////////////////////////// Loading functions for tab /////////////////////////////////////////////////
        /// <summary>
        /// Load movie information page
        /// </summary>
        private async Task loadMovie()
        {
            bool fromDormant = false;
            MovieProgressBar.IsIndeterminate = true;
            MovieProgressBar.Visibility = System.Windows.Visibility.Visible;
            try
            {
                Tuple<Movie, List<People>> tuple = await movieParser.getMovieByID();
                movie = tuple.Item1;
                List<People> peoples = tuple.Item2;
                movieGrid.DataContext = movie;
                trailer.Content = "预告片";
                trailer.NavigateUri = new Uri(Movie.movieLinkHeader + movie.id + "/trailer", UriKind.Absolute);
                theater.Content = "选座购票";
                theater.NavigateUri = new Uri(Movie.movieLinkHeader + movie.id + "/cinema", UriKind.Absolute);
                slash.Text = " / ";
                fixedName.Text = "人评分";
                peopleSelector.ItemsSource = peoples;
                MovieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    MovieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    // Not canceled by user, must be a network issue
                    if (!movieParser.isCanceled())
                    {
                        movieLoaded = false;
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
                    MovieProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    movieLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }

            // From dormant state, reload
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadMovie();
            }
        }

        /// <summary>
        /// Load short review
        /// </summary>
        /// <returns></returns>
        private async Task loadShortReview() 
        {
            if (movie != null)
            {
                bool fromDormant = false;
                shortReviewParser = new ShortReviewHtmlParser(movie);
                ShortReviewProgressBar.IsIndeterminate = true;
                ShortReviewProgressBar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    await shortReviewParser.getShortReview();
                    shortReviewSelector.ItemsSource = shortReviewParser.shortReviewCollection;
                    ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                } 
                catch (TaskCanceledException) 
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!shortReviewParser.isCanceled())
                        {
                            shortReviewLoaded = false;
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
                        ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        shortReviewLoaded = false;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadShortReview();
                }
            }
        }

        /// <summary>
        /// Load review
        /// </summary>
        /// <returns></returns>
        private async Task loadReview()
        {
            if (movie != null)
            {
                bool fromDormant = false;
                reviewParser = new ReviewParser(movie);
                ReviewProgressBar.IsIndeterminate = true;
                ReviewProgressBar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    await reviewParser.getReview();
                    reviewLongListSelector.ItemsSource = reviewParser.reviewCollection;
                    ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!reviewParser.isCanceled())
                        {
                            reviewLoaded = false;
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
                        reviewLoaded = false;
                        ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadReview();
                }
            }
        }

        /// <summary>
        /// Load image
        /// </summary>
        private async Task loadImage()
        {
            if (movie != null)
            {
                imageParser = new ImageHtmlParser(movie);
                ImageProgressBar.IsIndeterminate = true;
                ImageProgressBar.Visibility = System.Windows.Visibility.Visible;
                bool fromDormant = false;
                try
                {
                    await imageParser.getImage();
                    imageSelector.ItemsSource = imageParser.imageCollection;
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
                        if (!imageParser.isCanceled())
                        {
                            imageLoaded = false;
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
                        imageLoaded = false;
                        ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadImage();
                }
            }
        }

        private async Task loadMoreImage()
        {
            if (imageParser.hasMore)
            {
                bool fromDormant = false;
                ImageProgressBar.IsIndeterminate = true;
                ImageProgressBar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    await imageParser.loadMore();
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
                        if (!imageParser.isCanceled())
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
                        ImageProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreImage();
                }
            }
        }

        /// <summary>
        /// Load more review
        /// </summary>
        /// <returns></returns>
        private async Task loadMoreReview()
        {
            if (reviewParser.hasMoreReview)
            {
                bool fromDormant = false;
                ReviewProgressBar.IsIndeterminate = true;
                ReviewProgressBar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    await reviewParser.loadMore();
                    ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!reviewParser.isCanceled())
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
                        ReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreReview();
                }
            }
        }

        /// <summary>
        /// Load more short review
        /// </summary>
        /// <returns></returns>
        private async Task loadMoreShortReview()
        {
            if (shortReviewParser.hasMoreReview)
            {
                bool fromDormant = false;
                ShortReviewProgressBar.IsIndeterminate = true;
                ShortReviewProgressBar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    await shortReviewParser.loadMore();
                    ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        if (!shortReviewParser.isCanceled())
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
                        ShortReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                        MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                    }
                }

                if (fromDormant)
                {
                    App.isFromDormant = false;
                    await loadMoreShortReview();
                }
            }
        }

        /// <summary>
        /// Privot change event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((Pivot)sender).SelectedIndex;

            if (index == 0 || index == 1)
            {
                if (movieLoaded == false)
                {
                    movieLoaded = true;
                    await loadMovie();
                }
            }
            if (index == 2)
            {
                if (shortReviewLoaded == false)
                {
                    shortReviewLoaded = true;
                    await loadShortReview();
                }
            }
            if (index == 3)
            {
                if (reviewLoaded == false)
                {
                    reviewLoaded = true;
                    await loadReview();
                }
            }
            if (index == 4)
            {
                if (imageLoaded == false)
                {
                    imageLoaded = true;
                    await loadImage();
                }
            }
        }
        
        private void reviewLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (reviewLongListSelector != null && reviewLongListSelector.SelectedItem != null) {
                Review review = (Review)reviewLongListSelector.SelectedItem;
                if (review != null)
                {
                    App.reviewPassed = review;
                    NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
                }
                reviewLongListSelector.SelectedItem = null;
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

        private void imageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageSelector != null && imageSelector.SelectedItem != null)
            {
                MovieImage image = (MovieImage)imageSelector.SelectedItem;
                if (image != null)
                {
                    App.imagePassed = image;
                    App.imageCollectionPassed = imageParser.imageCollection;
                    NavigationService.Navigate(new Uri("/ImagePage.xaml", UriKind.Relative));
                }
                imageSelector.SelectedItem = null;
            }
        }

        private void peopleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (peopleSelector != null && peopleSelector.SelectedItem != null)
            {
                People people = (People)peopleSelector.SelectedItem;
                if (people != null && people.id != null && people.id != "")
                {
                    App.peoplePassed = people;
                    NavigationService.Navigate(new Uri("/PeoplePage.xaml", UriKind.Relative));
                }
                peopleSelector.SelectedItem = null;
            }
        }
    }
}