using LiveCharts.Wpf;
using LiveCharts;
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
using Color = System.Drawing.Color;

namespace PrzetwrzanieObrazow.FunctionWindows
{
    /// <summary>
    /// Logika interakcji dla klasy HistogramTable.xaml
    /// </summary>
    public partial class HistogramTable : Window
    {
        public class DataItem
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
        }

        Bitmap Bitmap { get; set; }

        public HistogramTable(Bitmap bitmap)
        {
            InitializeComponent();
            Bitmap = bitmap;
        }

        public async Task calculateHistogram()
        {
            this.DataGrid.Items.Clear();

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


            this.DataGrid.Items.Add(new DataItem() { Column1 = "trwes", Column2 = "teststs" });
            histo = null;
        }

        public async Task calculateHistogram(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
            calculateHistogram();
        }
    }
}
