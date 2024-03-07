using Emgu.CV.Dnn;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Color = System.Drawing.Color;

namespace PrzetwrzanieObrazow.FunctionWindows
{
    /// <summary>
    /// Logika interakcji dla klasy Histogram.xaml
    /// </summary>
    public partial class HistogramGraphic : Window
    {
        Bitmap Bitmap { get; set; }

        public HistogramGraphic(Bitmap bitmap)
        {
            InitializeComponent();

            this.Bitmap = bitmap;

            calculateHistogram();
        }

        public void calculateHistogram()
        {
            Dictionary<Color, int> histo = new Dictionary<Color, int>();
            int sum = 0;
            for (int x = 0; x < Bitmap.Width; x++)
            {
                for (int y = 0; y < Bitmap.Height; y++)
                {
                    Color c = Bitmap.GetPixel(x, y);
                    if (histo.ContainsKey(c))
                        histo[c] = histo[c] + 1;
                    else
                        histo.Add(c, 1);

                    sum++;
                }
            }

            var values = new ChartValues<int>();
            values.AddRange(histo.Select(x => x.Value).ToList());

            this.plot.Series.Add(new ColumnSeries()
            {
                ColumnPadding = 0,
                MaxColumnWidth = double.PositiveInfinity,
                Values = values
            }); ;
        }
    }
}
