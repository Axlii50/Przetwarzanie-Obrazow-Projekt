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

    ///TODO zmienie tego na input window dla jednej wartosci
    /// <summary>
    /// Logika interakcji dla klasy PosterizeWindowInput.xaml
    /// </summary>
    public partial class PosterizeWindowInput : Window
    {
        public int layers = -1;
        
        public PosterizeWindowInput()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO doadnie zabezpieczenia
            layers = int.Parse(this.LayersInput.Text);
            this.DialogResult = true;
        }
    }
}
