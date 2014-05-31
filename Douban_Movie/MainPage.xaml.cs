using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using PanoramaApp2.JsonParser;
using PanoramaApp2.HtmlParser;
using PanoramaApp2.Resources;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using PanoramaApp2.Utility;

namespace PanoramaApp2
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Popup popup;
        private Popup searchPopup;
        public ApplicationBarMenuItem settingMenu;
        public ApplicationBarMenuItem rateMenu;
        public ApplicationBarMenuItem aboutMenu;
        private bool hotLoaded;
        private bool latestLoaded;
        private bool top250Loaded;
        private bool usboxLoaded;
        private bool commentLoaded;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            App.mainPage = this;

            var top250PullDector = new WP8PullToRefreshDetector();
            top250PullDector.Bind(top250LongListSelector);
            top250PullDector.Compression += top250Dector_Compress;
            var hotReviewPullDector = new WP8PullToRefreshDetector();
            hotReviewPullDector.Bind(hotReviewLongListSelector);
            hotReviewPullDector.Compression += hotReviewDector_Compress;
            // Get hot movie
            popup = new Popup();
            searchPopup = new Popup();
        }

        /// <summary>
        /// Top 250 pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void top250Dector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (Top250HtmlParser.hasMore)
                {
                    await loadTopPivotItem();
                }
            }
        }

        /// <summary>
        /// Top 250 pull to refresh handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void hotReviewDector_Compress(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Bottom)
            {
                if (HotReviewHtmlParser.hasMore)
                {
                    await loadReviewPivotItem();
                }
            }
        }
        /// <summary>
        /// Create application bar
        /// </summary>
        private void createApplicationBar() {
            ApplicationBar = new ApplicationBar();
            
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 1;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            settingMenu = new ApplicationBarMenuItem(AppResources.SettingMenu);
            settingMenu.Click += settingMenu_Click;
            ApplicationBar.MenuItems.Add(settingMenu);
            
            rateMenu = new ApplicationBarMenuItem(AppResources.RateMenu);
            rateMenu.Click += rateMenu_Click;
            ApplicationBar.MenuItems.Add(rateMenu);

            aboutMenu = new ApplicationBarMenuItem(AppResources.AboutMenu);
            aboutMenu.Click += aboutMenu_Click;
            ApplicationBar.MenuItems.Add(aboutMenu);
        }

        void aboutMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        void rateMenu_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void settingMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void showPopup()
        {
            
            PopupSplash splash = new PopupSplash();
            splash.Height = Application.Current.Host.Content.ActualHeight;
            splash.Width = Application.Current.Host.Content.ActualWidth;
            popup.Child = splash;
            popup.IsOpen = true;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.fromTombStone)
            {
                App.fromTombStone = false;
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
                showPopup();
            }
            else
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    showPopup();
                    //HotMovieHtmlParser.parseHottMovie();
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

        private void hotLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotLongListSelector != null && hotLongListSelector.SelectedItem != null)
            {
                Movie movie = (Movie)hotLongListSelector.SelectedItem;
                App.moviePassed = movie;
                NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                hotLongListSelector.SelectedItem = null;
            }
        }

        private void usboxLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (usboxLongListSelector != null && usboxLongListSelector.SelectedItem != null)
            {
                Movie movie = (Movie)usboxLongListSelector.SelectedItem;
                App.moviePassed = movie;
                NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                usboxLongListSelector.SelectedItem = null;
            }
        }

        private void top250LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (top250LongListSelector != null && top250LongListSelector.SelectedItem != null)
            {
                Movie movie = (Movie)top250LongListSelector.SelectedItem;
                App.moviePassed = movie;
                NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                top250LongListSelector.SelectedItem = null;
            }
        }

        private void latestListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (latestListSelector != null && latestListSelector.SelectedItem != null)
            {
                Movie movie = (Movie)latestListSelector.SelectedItem;
                App.moviePassed = movie;
                NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                latestListSelector.SelectedItem = null;
            }
        }

        private void hotReviewLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotReviewLongListSelector != null && hotReviewLongListSelector.SelectedItem != null)
            {
                Review review = (Review)hotReviewLongListSelector.SelectedItem;
                App.reviewPassed = review;
                NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
                hotReviewLongListSelector.SelectedItem = null;
            }
        }

        private async Task loadLatestPivotItem()
        {
            // Get latest
            bool fromDormant = false;
            UpcomingProgressBar.IsIndeterminate = true;
            UpcomingProgressBar.Visibility = System.Windows.Visibility.Visible;
            try
            {
                latestListSelector.ItemsSource = await LatestHtmlParser.getLatestMovie();
                UpcomingProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    UpcomingProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    latestLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
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
                    UpcomingProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    latestLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadLatestPivotItem();
            }
        }

        private async Task loadTopPivotItem()
        {
            // Get top 250 movie
            bool fromDormant = false;
            TopProgressBar.IsIndeterminate = true;
            TopProgressBar.Visibility = System.Windows.Visibility.Visible;

            try
            {
                await Top250HtmlParser.getTop250();
                top250LongListSelector.ItemsSource = Top250HtmlParser.observableMovieList;
                TopProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    TopProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    top250Loaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
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
                    TopProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    top250Loaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadTopPivotItem();
            }
        }

        private async Task loadUSBoxPivotItem() 
        {
            // Get us box
            bool fromDormant = false;
            USBoxProgressBar.IsIndeterminate = true;
            USBoxProgressBar.Visibility = System.Windows.Visibility.Visible;

            try
            {
                usboxLongListSelector.ItemsSource = await USBoxJsonParser.getUSMovie();
                USBoxProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    USBoxProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    usboxLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
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
                    USBoxProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    usboxLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadUSBoxPivotItem();
            }
        }

        private async Task loadReviewPivotItem()
        {
            // Get hot review
            bool fromDormant = false;
            HotReviewProgressBar.IsIndeterminate = true;
            HotReviewProgressBar.Visibility = System.Windows.Visibility.Visible;

            try
            {
                await HotReviewHtmlParser.getHotReview();
                hotReviewLongListSelector.ItemsSource = HotReviewHtmlParser.reviewCollection;
                HotReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    HotReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    commentLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
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
                    HotReviewProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    commentLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadReviewPivotItem();
            }

        }

        /// <summary>
        /// Load hot movie
        /// </summary>
        /// <returns></returns>
        private async Task loadHotMovie()
        {
            bool fromDormant = false;
            try
            {
                hotLongListSelector.ItemsSource = await HotMovieHtmlParser.getHotMovie();
                popup.IsOpen = false;
                createApplicationBar();
            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    popup.IsOpen = false;
                    hotLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
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
                    popup.IsOpen = false;
                    hotLoaded = false;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }
            if (fromDormant)
            {
                App.isFromDormant = false;
                await loadHotMovie();
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((Pivot)sender).SelectedIndex;
            if (index == 0)
            {
                if (hotLoaded == false)
                {
                    hotLoaded = true;
                    await loadHotMovie();
                    createApplicationBar();
                }
            }

            else if (index == 1)
            {
                if (latestLoaded == false)
                {
                    latestLoaded = true;
                    await loadLatestPivotItem();
                }
            }
            else if (index == 2)
            {
                if (top250Loaded == false)
                {
                    top250Loaded = true;
                    await loadTopPivotItem();
                }
            }
            else if (index == 3)
            {
                if (usboxLoaded == false)
                {
                    usboxLoaded = true;
                    await loadUSBoxPivotItem();
                }
            }
            else if (index == 4)
            {
                if (commentLoaded == false)
                {
                    commentLoaded = true;
                    await loadReviewPivotItem();
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
    }
}