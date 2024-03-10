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
        public event PropertyChangedEventHandler PropertyChanged;
        private Emgu.CV.Image<Bgr, byte> _image;
        public Image<Bgr, byte> Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
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
                Image = value.ToImage<Bgr, byte>();
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        HistogramTable histogramTable = null;
        public void OpenHistogramTable()
        {
            histogramTable = new HistogramTable(Bitmap);
            histogramTable.Show();
        }
    }
}
