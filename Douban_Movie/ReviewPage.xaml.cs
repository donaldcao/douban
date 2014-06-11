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
    public partial class ReviewPage : PhoneApplicationPage
    {
        private Review review;
        private ReviewHtmlParser reviewParser;
        private Popup searchPopup;
        private bool commentNewLoad = false;

        public ReviewPage()
        {
            InitializeComponent();
            searchPopup = new Popup();
            review = App.reviewPassed;
            if (review != null)
            {
                reviewParser = new ReviewHtmlParser(review);
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
                if (reviewParser != null)
                {
                    reviewParser.cancelDownload();
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
                    if (reviewParser != null)
                    {
                        await loadReview();
                    }
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
                if (reviewParser != null && review != null)
                {
                    await loadMoreComment();
                }
            }
        }

        private async Task loadMoreComment()
        {
            if (!reviewParser.hasMoreComments)
            {
                noCommentTextBlock.Visibility = System.Windows.Visibility.Visible;
                await Task.Delay(2000);
                noCommentTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else {
                bool fromDormant = false;
                ReviewCommentProgressBar.IsIndeterminate = true;
                ReviewCommentProgressBar.Visibility = System.Windows.Visibility.Visible;
                commentNewLoad = true;

                try
                {
                    await reviewParser.loadMore();
                    ReviewCommentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch (TaskCanceledException)
                {
                    if (App.isFromDormant)
                    {
                        fromDormant = true;
                    }
                    else
                    {
                        ReviewCommentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
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
                        ReviewCommentProgressBar.Visibility = System.Windows.Visibility.Collapsed;
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
        /// Load review
        /// </summary>
        /// <returns></returns>
        private async Task loadReview()
        {
            bool fromDormant = false;
            ReviewProgressBar.IsIndeterminate = true;
            ReviewProgressBar.Visibility = System.Windows.Visibility.Visible;
            try
            {
                await reviewParser.getReview();
                border.Visibility = Visibility.Visible;
                movieText.Visibility = Visibility.Visible;
                reviewStackPanel.DataContext = reviewParser.review;
                commentSelector.ItemsSource = reviewParser.commentCollection;
                if (commentSelector.ItemsSource.Count == 0)
                {
                    commentTextBlock.Visibility = System.Windows.Visibility.Visible;
                }
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
                await loadReview();
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