using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanoramaApp2.Resources;
using System.Windows.Media;
using System.ComponentModel;

namespace PanoramaApp2
{
    public class Settings : INotifyPropertyChanged
    {
        private Settings()
        {
        }

        private static Settings _instance;
        public static Settings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }

        public enum Languages {SCHINESE, TCHINESE, ENGLISH};
        private string[] _locales = { "zh-CN", "en-US", "zh-TW" };
        public string[] locales {
            get {
                return _locales;
            }
            set {
                if (_locales == value) 
                {
                    return;
                }
                _locales = value;
                NotifyPropertyChanged("locales");
            }
        }

        private string[] _languages = { "简体中文", "English", "繁體中文" };
        public string[] languages {
            get {
                return _languages;
            }
            set {
                if (_languages == value) 
                {
                    return;
                }
                _languages = value;
                NotifyPropertyChanged("languages");
            }
        }

        private string _languageSetting = "language";
        public string languageSetting 
        {
            get 
            {
                return _languageSetting;
            }
            set 
            {
                if (_languageSetting == value) {
                    return;
                }
                _languageSetting = value;
                NotifyPropertyChanged("languageSetting");
            }
        }

        public enum Backgrounds { BLUE, BLACK };
        private string[] _backgrounds = { AppResources.Blue, AppResources.Black };
        public string[] backgrounds
        {
            get
            {
                return _backgrounds;
            }
            set
            {
                if (_backgrounds == value)
                {
                    return;
                }
                _backgrounds = value;
                NotifyPropertyChanged("backgrounds");
            }
        }
        private string _backgroundSetting = "background";
        public string backgroundSetting
        {
            get
            {
                return _backgroundSetting;
            }
            set
            {
                if (backgroundSetting == value)
                {
                    return;
                }
                _backgroundSetting = value;
                NotifyPropertyChanged("backgroundSetting");
            }
        }
        private string _blueString = "Blue";
        public string blueString
        {
            get
            {
                return _blueString;
            }
            set
            {
                if (_blueString == value)
                {
                    return;
                }
                _blueString = value;
                NotifyPropertyChanged("blueString");
            }
        }

        private string _blackString = "Black";
        public string blackString
        {
            get
            {
                return _blackString;
            }
            set
            {
                if (_blackString == value)
                {
                    return;
                }
                _blackString = value;
                NotifyPropertyChanged("blackString");
            }
        }

        private Backgrounds _defaultBackground = Backgrounds.BLUE;
        public Backgrounds defaultBackground
        {
            get
            {
                return _defaultBackground;
            }
            set
            {
                if (_defaultBackground == value)
                {
                    return;
                }
                _defaultBackground = value;
                NotifyPropertyChanged("defaultBackground");
            }
        }

        private Languages _language;
        public Languages language
        {
            get
            {
                return _language;
            }
            set
            {
                if (_language == value)
                {
                    return;
                }
                _language = value;
                NotifyPropertyChanged("language");
            }
        }

        private Backgrounds _background;
        public Backgrounds background
        {
            get
            {
                return _background;
            }
            set
            {
                if (_background == value)
                {
                    return;
                }
                _background = value;
                NotifyPropertyChanged("background");
            }
        }

        private SolidColorBrush _bannerBrush;
        public SolidColorBrush bannerBrush
        {
            get
            {
                return _bannerBrush;
            }
            set
            {
                if (_bannerBrush == value)
                {
                    return;
                }
                _bannerBrush = value;
                NotifyPropertyChanged("bannerBrush");
            }
        }

        private SolidColorBrush _backgroundBrush;
        public SolidColorBrush backgroundBrush
        {
            get
            {
                return _backgroundBrush;
            }
            set
            {
                if (_backgroundBrush == value)
                {
                    return;
                }
                _backgroundBrush = value;
                NotifyPropertyChanged("backgroundBrush");
            }
        }

        public void updateLanguage()
        {
            backgrounds[0] = AppResources.Blue;
            backgrounds[1] = AppResources.Black;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
