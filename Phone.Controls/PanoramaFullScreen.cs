using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;

namespace Phone.Controls
{
    public class PanoramaFullScreen : Panorama
    {
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            availableSize.Width += 48;
            return base.MeasureOverride(availableSize);
        }
    }
}
