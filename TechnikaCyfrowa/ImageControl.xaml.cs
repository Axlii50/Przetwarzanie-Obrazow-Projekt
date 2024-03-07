using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrzetwrzanieObrazow
{
    /// <summary>
    /// Logika interakcji dla klasy ImageControl.xaml
    /// </summary>
    public partial class ImageControl : UserControl
    {
        public ImageControl(string text, System.Drawing.Bitmap image)
        {
            InitializeComponent();

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               image.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());

            this.Image.Source = bitmapSource;
            this.Text.Content = text;

            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Margin = new Thickness(0,5,0,0);
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var image = App.Images.FirstOrDefault(im => im.Name == this.Text.Content);

            App.CurrentImage = image;
        }
    }
}
