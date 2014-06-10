using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanoramaApp2.Resources;

namespace PanoramaApp2
{
    public class Settings
    {
        public enum Languages {SCHINESE, TCHINESE, ENGLISH};
        public static string[] locales = { "zh-CN", "en-US", "zh-TW" };
        public static string[] languages = { "简体中文", "English", "繁體中文" };
        public static string languageSetting = "language";
        public static string defaultLocale = locales[0];
        public enum Backgrounds { BLUE, BLACK };
        public static string[] backgrounds = { AppResources.Blue, AppResources.Black };
        public static string backgroundSetting = "background";
        public static string blueString = "Blue";
        public static string blackString = "Black";
        public static Backgrounds defaultBackground = Backgrounds.BLUE;
        public static Languages language { set; get; }
        public static Backgrounds background { set; get; }

        public static void updateLanguage()
        {
            backgrounds[0] = AppResources.Blue;
            backgrounds[1] = AppResources.Black;
        }
    }
}
