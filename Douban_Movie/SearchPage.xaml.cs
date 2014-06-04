using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PanoramaApp2.JsonParser;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using PanoramaApp2.Resources;

namespace PanoramaApp2
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private SearchJsonParser parser = null;
        private Popup searchPopup;

        public SearchPage()
        {
            InitializeComponent();
            searchPopup = new Popup();
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
                    string msg = "";
                    if (NavigationContext.QueryString.TryGetValue("msg", out msg))
                    {
                        parser = new SearchJsonParser(msg);
                        await search();
                    }
                }
            }
        }

        public async Task search()
        {
            bool fromDormant = false;
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = System.Windows.Visibility.Visible;
            try
            {
                await parser.search();
                searchLongListSelector.ItemsSource = parser.movieCollection;
                resultNumber.Text = parser.resultNumber + " ";
                progressBar.Visibility = System.Windows.Visibility.Collapsed;

            }
            catch (TaskCanceledException)
            {
                if (App.isFromDormant)
                {
                    fromDormant = true;
                }
                else
                {
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
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
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
                }
            }

            if (fromDormant)
            {
                App.isFromDormant = false;
                await search();
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

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PopupInput input = new PopupInput();
            input.Width = Application.Current.Host.Content.ActualWidth;
            input.Height = Application.Current.Host.Content.ActualHeight;
            searchPopup.Child = input;
            searchPopup.IsOpen = true;
            input.inputBox.Focus();
        }

        private void searchLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchLongListSelector != null && searchLongListSelector.SelectedItem != null)
            {
                Movie m = (Movie)searchLongListSelector.SelectedItem;
                if (m != null && m.id != string.Empty)
                {
                    App.moviePassed = m;
                    NavigationService.Navigate(new Uri("/MoviePage.xaml", UriKind.Relative));
                }
                searchLongListSelector.SelectedItem = null;
            }
        }
    }
}