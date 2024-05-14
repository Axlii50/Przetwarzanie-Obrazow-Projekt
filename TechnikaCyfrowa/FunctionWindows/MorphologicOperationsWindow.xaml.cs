using Emgu.CV.CvEnum;
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
    /// Logika interakcji dla klasy MorphologicOperationsWindow.xaml
    /// </summary>
    public partial class MorphologicOperationsWindow : Window
    {
        public BorderType borderT;
        public MorphOp MorphOperation;
        public ElementShape EleShape;

        public MorphologicOperationsWindow()
        {
            InitializeComponent();
        }

        private void MorfologicalOperationsApplay_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            var test = Enum.Parse<BorderType>("Isolated");
            var test2 = this.borderTypeComboBox.SelectedItem.ToString();

            this.borderT = Enum.Parse<BorderType>(this.borderTypeComboBox.SelectedItem.ToString());
            this.MorphOperation = Enum.Parse<MorphOp>(this.Operation_ComboBox.SelectedItem.ToString());
            this.EleShape = Enum.Parse<ElementShape>(this.Shape_ComboBox.SelectedItem.ToString());
        }
    }
}
