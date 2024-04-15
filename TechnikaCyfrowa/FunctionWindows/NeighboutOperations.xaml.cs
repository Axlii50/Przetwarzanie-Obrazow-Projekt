using Emgu.CV;
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
    /// Logika interakcji dla klasy NeighboutOperations.xaml
    /// </summary>
    public partial class NeighboutOperations : Window
    {
        public NeighboutOperations()
        {
            InitializeComponent();

            this.BorderTypeComboBox.Items.Add(BorderType.Isolated);
            this.BorderTypeComboBox.Items.Add(BorderType.Reflect);
            this.BorderTypeComboBox.Items.Add(BorderType.Replicate);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (this.Operation.SelectedItem)
            {
                case "Blur": Blur(); break;
                case "gaussianBlur": GausianBlur(); break;
                case "SobelX": SobelX(); break;
                case "SobelY": SobelY(); break;
                case "Laplacian": break;
                case "Canny": break;
                case "Laplacian1": break;
                case "Laplacian2": break;
                case "Laplacian3": break;
                case "Prewitta1": break;
                case "Prewitta2": break;
                case "Prewitta3": break;
                case "Prewitta4": break;
                case "Prewitta5": break;
                case "Prewitta6": break;
                case "Prewitta7": break;
                case "Prewitta8": break;
                case "Uniwersal": break;
            }

            this.Close();
        }
        
        public void Blur()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat;

            CvInvoke.Blur(NewMat, NewMat, new System.Drawing.Size(3,3), new System.Drawing.Point(-1, -1), borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void GausianBlur()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat;

            CvInvoke.GaussianBlur(NewMat, NewMat, new System.Drawing.Size(3, 3), 1.5f, 1.5f, borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void SobelX()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat;

            CvInvoke.Sobel(NewMat, NewMat, DepthType.Cv64F, 1, 0, 3, 1, 0, borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void SobelY()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat;

            CvInvoke.Sobel(NewMat, NewMat, DepthType.Cv64F, 0, 1, 3, 1, 0, borderType);

            App.FocusedWindow.Mat = NewMat;
        }
    }
}
