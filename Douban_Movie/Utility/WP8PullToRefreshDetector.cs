using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;

namespace PanoramaApp2.Utility
{
    class WP8PullToRefreshDetector
    {
        LongListSelector listbox;

        bool viewportChanged = false;
        bool isMoving = false;
        double manipulationStart = 0;
        double manipulationEnd = 0;
        private ViewportControl _viewport = null;

        public bool Bound { get; private set; }

        public void Bind(LongListSelector listbox)
        {
            Bound = true;
            this.listbox = listbox;
            listbox.ManipulationStateChanged += listbox_ManipulationStateChanged;
            listbox.MouseMove += listbox_MouseMove;
            listbox.ItemRealized += OnViewportChanged;
            listbox.ItemUnrealized += OnViewportChanged;
            listbox.Loaded += listbox_Loaded;
        }

        public void Unbind()
        {
            Bound = false;

            if (listbox != null)
            {
                listbox.ManipulationStateChanged -= listbox_ManipulationStateChanged;
                listbox.MouseMove -= listbox_MouseMove;
                listbox.ItemRealized -= OnViewportChanged;
                listbox.ItemUnrealized -= OnViewportChanged;
            }
        }

        private void listbox_Loaded(object sender, RoutedEventArgs e)
        {
            this._viewport = FindVisualChild<ViewportControl>(listbox);
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        void OnViewportChanged(object sender, Microsoft.Phone.Controls.ItemRealizationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Realized");
            viewportChanged = true;
        }

        void listbox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pos = e.GetPosition(null);

            if (!isMoving)
            {
                System.Diagnostics.Debug.WriteLine("not moving, position is " + pos.Y);
                manipulationStart = pos.Y;
            }
            else 
            {
                System.Diagnostics.Debug.WriteLine("Moving, position is " + pos.Y);
                manipulationEnd = pos.Y;
            }

            isMoving = true;
        }

        void listbox_ManipulationStateChanged(object sender, EventArgs e)
        {
            if (listbox.ManipulationState == ManipulationState.Idle)
            {
                isMoving = false;
                viewportChanged = false;
            }
            else if (listbox.ManipulationState == ManipulationState.Manipulating)
            {
                viewportChanged = false;
            }
            else if (listbox.ManipulationState == ManipulationState.Animating)
            {
                var total = manipulationStart - manipulationEnd;

                if (!viewportChanged && Compression != null)
                {
                    System.Diagnostics.Debug.WriteLine("bound top " + _viewport.Bounds.Top);
                    System.Diagnostics.Debug.WriteLine("bound bottom " + _viewport.Bounds.Bottom);
                    System.Diagnostics.Debug.WriteLine("viewport top " + _viewport.Viewport.Top);
                    System.Diagnostics.Debug.WriteLine("viewport bottom " + _viewport.Viewport.Bottom);
                    if (total < 0 && (_viewport.Viewport.Top == _viewport.Bounds.Top))
                        Compression(this, new CompressionEventArgs(CompressionType.Top));
                    else if (total > 0 && (_viewport.Bounds.Bottom == _viewport.Viewport.Bottom)) // Explicitly exclude total == 0 case
                        Compression(this, new CompressionEventArgs(CompressionType.Bottom));
                }
            }
        }

        public event OnCompression Compression;
    }

    public class CompressionEventArgs : EventArgs
    {
        public CompressionType Type { get; protected set; }

        public CompressionEventArgs(CompressionType type)
        {
            Type = type;
        }
    }

    public enum CompressionType { Top, Bottom, Left, Right };

    public delegate void OnCompression(object sender, CompressionEventArgs e);
}
