using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Globalization;
using System.Threading;
using PanoramaApp2.Resources;
using System.Windows.Markup;
using System.IO.IsolatedStorage;

namespace PanoramaApp2
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private int currentLanguageIndex;
        private int currentBackgroundIndex;

        public SettingPage()
        {
            currentLanguageIndex = -1;
            currentBackgroundIndex = -1;
            InitializeComponent();
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
                    string[] languages = Settings.languages;
                    languagePicker.ItemsSource = languages;
                    currentLanguageIndex = 0;
                    IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                    if (appSettings.Contains(Settings.languageSetting)) 
                    {
                        string languageSetting = (string)appSettings[Settings.languageSetting];
                        currentLanguageIndex = Array.IndexOf(Settings.locales, languageSetting);
                        languagePicker.SelectedIndex = currentLanguageIndex;
                    }
                    languagePicker.SelectedIndex = currentLanguageIndex;

                    Settings.updateLanguage();
                    string[] backgrounds = Settings.backgrounds;
                    System.Diagnostics.Debug.WriteLine(Settings.backgrounds[0]);
                    System.Diagnostics.Debug.WriteLine(AppResources.Black);
                    backgroundPicker.ItemsSource = backgrounds;
                    currentBackgroundIndex = 0;
                    if (appSettings.Contains(Settings.backgroundSetting))
                    {
                        string backgroundSetting = (string)appSettings[Settings.backgroundSetting];
                        if (backgroundSetting == Settings.blueString)
                        {
                            Settings.background = Settings.Backgrounds.BLUE;
                            currentBackgroundIndex = 0;
                        }
                        if (backgroundSetting == Settings.blackString)
                        {
                            Settings.background = Settings.Backgrounds.BLACK;
                            currentBackgroundIndex = 1;
                        }
                    }
                    backgroundPicker.SelectedIndex = currentBackgroundIndex;
                }
            }
        }

        private void backgroundPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((ListPicker)sender).SelectedIndex;
            if (currentBackgroundIndex == -1 || index == currentBackgroundIndex)
            {
                return;
            }
            currentBackgroundIndex = index;
            if (index == 0)
            {
                Settings.background = Settings.Backgrounds.BLUE;
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                appSettings[Settings.backgroundSetting] = Settings.blueString;
                appSettings.Save();
            }
            if (index == 1)
            {
                Settings.background = Settings.Backgrounds.BLACK;
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                appSettings[Settings.backgroundSetting] = Settings.blackString;
                appSettings.Save();
            }
        }

        private void languagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((ListPicker)sender).SelectedIndex;
            if (currentLanguageIndex == -1 || index == currentLanguageIndex)
            {
                return;
            }
            currentLanguageIndex = index;
            setUI(Settings.locales[index]);
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            appSettings[Settings.languageSetting] = Settings.locales[index];
            appSettings.Save();
        }


        private void setUI(string locale)
        {
            // Set this thread's current culture to the culture associated with the selected locale.
            CultureInfo newCulture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;


            // Set the FlowDirection of the RootFrame to match the new culture.
            FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
            App.RootFrame.FlowDirection = flow;

            // Set the Language of the RootFrame to match the new culture.
            App.RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

            App.mainPage.title.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.title.Text = AppResources.ApplicationTitle;

            App.mainPage.hotPivotItem.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.hotPivotItem.Header = AppResources.HotPivot;

            App.mainPage.UpcompingPivotItem.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.UpcompingPivotItem.Header = AppResources.ComingPivot;

            App.mainPage.topPivotItem.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.topPivotItem.Header = AppResources.TopPivot;

            App.mainPage.reviewPivotItem.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.reviewPivotItem.Header = AppResources.HotReviewPivot;

            App.mainPage.boxPivotItem.Language = XmlLanguage.GetLanguage(locale);
            App.mainPage.boxPivotItem.Header = AppResources.USPivot;

            App.mainPage.settingMenu.Text = AppResources.SettingMenu;
            App.mainPage.rateMenu.Text = AppResources.RateMenu;
            App.mainPage.aboutMenu.Text = AppResources.AboutMenu;
        }
    }
}