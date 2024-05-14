using System;
using System.Collections.Generic;
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

namespace PrzetwrzanieObrazow.FunctionWindows
{
    /// <summary>
    /// Logika interakcji dla klasy ResizeHistogramWindow.xaml
    /// </summary>
    public partial class ResizeHistogramWindow : Window
    {
        public int p1, p2, q3, q4;

        public ResizeHistogramWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            this.p1 = int.Parse(this.p1TextBox.Text);
            this.p2 = int.Parse(this.p2TextBox.Text);
            this.q3 = int.Parse(this.p3TextBox.Text);
            this.q4 = int.Parse(this.p4TextBox.Text);
        }
    }
}
