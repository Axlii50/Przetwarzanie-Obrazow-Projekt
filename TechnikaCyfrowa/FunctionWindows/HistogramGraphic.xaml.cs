using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Reg;
using Emgu.CV.Structure;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace PrzetwrzanieObrazow.FunctionWindows
{
    /// <summary>
    /// Logika interakcji dla klasy Histogram.xaml
    /// </summary>
    public partial class HistogramGraphic : Window
    {
        Mat mat { get; set; }

        public HistogramGraphic(Mat mat)
        {
            InitializeComponent();

            this.mat = mat;

            calculateHistogram();
        }

        public async Task calculateHistogram()
        {
            var data = CountPixelValues(this.mat);

            this.plot.Series.Clear();
            
            this.plot.Series.Add(new ColumnSeries()
            {
                ColumnPadding = 0,
                MaxColumnWidth = double.PositiveInfinity,
                Values = new ChartValues<int>(data),
                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255))
            });
        }

        public static int[] CountPixelValues(Mat image)
        {
            // Upewniamy się, że obraz jest w skali szarości
            if (image.NumberOfChannels != 1)
                MessageBox.Show("Obraz nie jest w skali szarocieniowej");

            // Inicjalizacja tablicy do przechowywania liczników dla każdej możliwej wartości piksela
            int[] pixelCounts = new int[256];

            // Konwersja Mat na obraz w skali szarości
            Image<Gray, byte> grayImage = image.ToImage<Gray, byte>();

            // Iteracja przez wszystkie piksele obrazu
            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
                    // Odczytanie wartości piksela
                    byte pixelValue = grayImage.Data[y, x, 0];

                    // Inkrementacja odpowiedniego licznika
                    pixelCounts[pixelValue]++;
                }
            }

            return pixelCounts;
        }

        public async Task calculateHistogram(Mat mat)
        {
            this.mat = mat;
            calculateHistogram();
        }
    }
}
