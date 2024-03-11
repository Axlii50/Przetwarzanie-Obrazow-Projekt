using Emgu.CV.Dnn;
using Emgu.CV.Reg;
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
        Bitmap Bitmap { get; set; }

        public HistogramGraphic(Bitmap Bitmap)
        {
            InitializeComponent();

            this.Bitmap = Bitmap;

            calculateHistogram();
        }

        public async Task calculateHistogram()
        {
            Dictionary<byte, int> redHistogram = new Dictionary<byte, int>();
            Dictionary<byte, int> greenHistogram = new Dictionary<byte, int>();
            Dictionary<byte, int> blueHistogram = new Dictionary<byte, int>();

            Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            BitmapData bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * Bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                byte blue = rgbValues[i];
                byte green = rgbValues[i + 1];
                byte red = rgbValues[i + 2];

                // Aktualizacja histogramu dla czerwonego
                if (redHistogram.ContainsKey(red))
                {
                    redHistogram[red]++;
                }
                else
                {
                    redHistogram.Add(red, 1);
                }

                // Aktualizacja histogramu dla zielonego
                if (greenHistogram.ContainsKey(green))
                {
                    greenHistogram[green]++;
                }
                else
                {
                    greenHistogram.Add(green, 1);
                }

                // Aktualizacja histogramu dla niebieskiego
                if (blueHistogram.ContainsKey(blue))
                {
                    blueHistogram[blue]++;
                }
                else
                {
                    blueHistogram.Add(blue, 1);
                }
            }

            Bitmap.UnlockBits(bitmapData);
            rgbValues = null;
            GC.Collect();

            this.plot.Series.Clear();

            this.plot.Series.Add(new ColumnSeries()
            {
                ColumnPadding = 0,
                MaxColumnWidth = double.PositiveInfinity,
                Values = new ChartValues<int>(redHistogram.Select(x => x.Value).ToList()),
                Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            });
            this.plot.Series.Add(new ColumnSeries()
            {
                ColumnPadding = 0,
                MaxColumnWidth = double.PositiveInfinity,
                Values = new ChartValues<int>(greenHistogram.Select(x => x.Value).ToList()),
                Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0))
            }); 
            this.plot.Series.Add(new ColumnSeries()
            {
                ColumnPadding = 0,
                MaxColumnWidth = double.PositiveInfinity,
                Values = new ChartValues<int>(blueHistogram.Select(x => x.Value).ToList()),
                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255))
            });

            redHistogram = null;
            greenHistogram = null;
            blueHistogram = null;
        }

        public async Task calculateHistogram(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
            calculateHistogram();
        }
    }
}
