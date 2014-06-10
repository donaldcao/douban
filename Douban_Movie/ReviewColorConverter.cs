using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PanoramaApp2
{
    public class ReviewColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Review review = (Review)value;
                if (review.background == Settings.Backgrounds.BLUE)
                {
                    return "#3063A5";
                }
                if (review.background == Settings.Backgrounds.BLACK)
                {
                    return "#4D3E1B";
                }
            }
            return "#3063A5";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }

    }
}
