using Emgu.CV.Structure;
using Emgu.CV;
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
using System.Windows.Shapes;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace PrzetwrzanieObrazow.FunctionWindows
{
    /// <summary>
    /// Logika interakcji dla klasy ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        const int HEIGHT = 450, WIDTH = 800;
        public event PropertyChangedEventHandler PropertyChanged;
        private Emgu.CV.Image<Bgr, byte> _image;

        public Image<Bgr, byte> Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    Image?.Dispose();
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return this.Image.ToBitmap();
            }
            set
            {
                Image?.Dispose();
                Image = value.ToImage<Bgr, byte>();
                value.Dispose();
                OnPropertyChanged(nameof(Image));
            }
        }

        public ImageWindow(Bitmap bitmap, string title)
        {
            InitializeComponent();

            this.Bitmap = bitmap;
            this.Title = title;

            SetBitmap();
        }

        protected virtual async void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            SetBitmap();

            if (histogramTable != null) await histogramTable.calculateHistogram(this.Bitmap);
            if (histogramGraphic != null) await histogramGraphic.calculateHistogram(this.Bitmap);
        }

        private void SetBitmap()
        {
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                Bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            this.ImageControl.Source = bitmapSource;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            App.FocusedWindow = this;
        }

        HistogramGraphic histogramGraphic = null;
        public void OpenHistogramGraphic()
        {
            histogramGraphic = new HistogramGraphic(Bitmap);
            histogramGraphic.Show();
        }

        //dodanie możliwe czyszczenia tej zmiennej
        HistogramTable histogramTable = null;
        public void OpenHistogramTable()
        {
            histogramTable = new HistogramTable(Bitmap);
            histogramTable.Show();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Bitmap.Dispose();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int SizeChange = 100;
            double proportionWidth = (double)WIDTH / this.Bitmap.Width;
            double proportionHeight = (double)HEIGHT / this.Bitmap.Height;

            if (e.Delta < 0) SizeChange = -SizeChange;
            
            this.Width += SizeChange * proportionWidth;
            this.Height += SizeChange * proportionHeight;
        }
    }
}
