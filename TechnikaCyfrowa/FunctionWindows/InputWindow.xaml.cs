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
    /// Logika interakcji dla klasy InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public int inputValue = -1;
        
        public InputWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO doadnie zabezpieczenia
            inputValue = int.Parse(this.LayersInput.Text);
            this.DialogResult = true;
        }
    }
}
