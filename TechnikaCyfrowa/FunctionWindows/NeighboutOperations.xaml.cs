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
                case "Laplacian": Laplacian(); break;
                case "Canny": Canny(); break;
                case "Laplacian1": Laplacian1(1); break;
                case "Laplacian2": Laplacian1(2); break;
                case "Laplacian3": Laplacian1(3); break;
                case "Prewitta1": Prewitt(1); break;
                case "Prewitta2": Prewitt(2); break;
                case "Prewitta3": Prewitt(3); break;
                case "Prewitta4": Prewitt(4); break;
                case "Prewitta5": Prewitt(5); break;
                case "Prewitta6": Prewitt(6); break;
                case "Prewitta7": Prewitt(7); break;
                case "Prewitta8": Prewitt(8); break;
                case "Uniwersal": break;
            }

            this.Close();
        }

        private void Prewitt(int prewitIndex)
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());
            //    {-1, 0, 1 }, {-1, 0, 1 }, {-1, 0, 1 } ,  // H     Poziomy             1
            //    { -1, -1, -1}, {0, 0, 0 }, {1, 1, 1 },   // V     Pionowy             2
            //    { 0, 1, 1 }, {-1, 0, 1 }, {-1, -1, 0 },  // NE    Prawo-Góra          3
            //    { -1, 0, 1 }, {-1, 0, 1 }, {0, -1, 1 },  // SE    Prawo-Dół           4
            //    { 1, 1, 0 }, {1, 0, -1 }, {0, -1, -1 },  // NW    Lewo-Góra           5
            //    { 0, -1, -1 }, {1, 0, -1 }, {1, 1, 0 },  // SW    Lewo-Dół            6
            //    { 1, 0, -1 }, {1, 0, -1 }, {1, 0, -1 },  // IH    Odwrócony poziomy   7
            //    { 0, -1, 0 }, {-1, 0, -1 }, {0, 1, 0 }    // IV   Odwrócony pionowy   8
            Matrix<double> matrix = prewitIndex switch
            {
                1 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                         {-1, 0, 1 }, {-1, 0, 1 }, {-1, 0, 1 } }
                },
                2 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { -1, -1, -1}, {0, 0, 0 }, {1, 1, 1 } }
                },
                3 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { 0, 1, 1 }, {-1, 0, 1 }, {-1, -1, 0 } }
                },
                4 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { -1, 0, 1 }, {-1, 0, 1 }, {0, -1, 1 } }
                },
                5 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { 1, 1, 0 }, {1, 0, -1 }, {0, -1, -1 } }
                },
                6 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                         { 0, -1, -1 }, {1, 0, -1 }, {1, 1, 0 } }
                },
                7 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                         { 1, 0, -1 }, {1, 0, -1 }, {1, 0, -1 } }
                },
                8 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { 0, -1, 0 }, {-1, 0, -1 }, {0, 1, 0 }   }
                },
            };

            var NewMat = App.FocusedWindow.Mat.Clone();
            CvInvoke.Filter2D(NewMat, NewMat, matrix, new System.Drawing.Point(-1, -1), 0, borderType);
            App.FocusedWindow.Mat = NewMat;
        }

        private void Laplacian1(int laplacianIndex)
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());
            Matrix<double> matrix = laplacianIndex switch
            {
                1 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { 0, -1, 0 },
                        { -1, 5, -1 },
                        { 0, -1, 0 } }
                },

                2 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { -1, -1, -1 },
                        { -1, 9, -1 },
                        { -1, -1, -1 } }
                },

                3 => new Matrix<double>(3, 3)
                {
                    Data = new double[3, 3] {
                        { 1, -2, 1 },
                        { -2, 5, -2 },
                        { 1, -2, 1 } }
                },
            };
            
            var NewMat = App.FocusedWindow.Mat.Clone();
            CvInvoke.Filter2D(NewMat, NewMat, matrix, new System.Drawing.Point(-1,-1), 0, borderType);
            App.FocusedWindow.Mat = NewMat;
        }

        private void Canny()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            double threshold1 = 0, threshold2 = 0;

            var input = new InputWindow();
            if (input.ShowDialog() == true)
                threshold1 = input.inputValue;
            input = new InputWindow();
            if (input.ShowDialog() == true)
                threshold2 = input.inputValue;

            CvInvoke.Canny(NewMat, NewMat, threshold1, threshold2);

            App.FocusedWindow.Mat = NewMat;
        }

        private void Laplacian()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            CvInvoke.Laplacian(NewMat, NewMat, DepthType.Cv64F, 1, 1, 0, borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void Blur()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            CvInvoke.Blur(NewMat, NewMat, new System.Drawing.Size(3,3), new System.Drawing.Point(-1, -1), borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void GausianBlur()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            CvInvoke.GaussianBlur(NewMat, NewMat, new System.Drawing.Size(3, 3), 1.5f, 1.5f, borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void SobelX()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            CvInvoke.Sobel(NewMat, NewMat, DepthType.Cv64F, 1, 0, 3, 1, 0, borderType);

            App.FocusedWindow.Mat = NewMat;
        }

        public void SobelY()
        {
            var borderType = Enum.Parse<BorderType>(this.BorderTypeComboBox.SelectedItem.ToString());

            var NewMat = App.FocusedWindow.Mat.Clone();

            CvInvoke.Sobel(NewMat, NewMat, DepthType.Cv64F, 0, 1, 3, 1, 0, borderType);

            App.FocusedWindow.Mat = NewMat;
        }
    }
}
