using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Win32;
using PrzetwrzanieObrazow.FunctionWindows;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;


namespace PrzetwrzanieObrazow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GrayScaleLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = GetFile();

            if (dialogResult.fileName == string.Empty) return;

            Bitmap originalBitmap = new Bitmap(dialogResult.filePath);

            await Task.Run(() => ConvertToGrayScale(ref originalBitmap));

            OpenNewWindow(originalBitmap, dialogResult.fileName);
        }

        private void ColorScaleLoadButton_Click(object sender, RoutedEventArgs e)
        {
            (string fileName, string filePath) dialogResult = GetFile();

            if (dialogResult.fileName == string.Empty) return;

            OpenNewWindow(new System.Drawing.Bitmap(dialogResult.filePath), dialogResult.fileName);
        }
        private void HistogramGraphicButton_Click(object sender, RoutedEventArgs e)
        {
            App.FocusedWindow.OpenHistogramGraphic();
        }

        private void HistogramTableButton_Click(object sender, RoutedEventArgs e)
        {
            App.FocusedWindow.OpenHistogramTable();
        }

        #region Functions

        public void OpenNewWindow(Bitmap bitmap, string title)
        {
            ImageWindow imageWindow = new ImageWindow(bitmap, title);
            imageWindow.Show();
        }
        public void OpenNewWindow(Image<Bgr, byte> image, string title)
        {
            ImageWindow imageWindow = new ImageWindow(image, title);
            imageWindow.Show();
        }
        
        public void OpenNewWindow(Image<Gray, byte> image, string title)
        {
            ImageWindow imageWindow = new ImageWindow(image, title);
            imageWindow.Show();
        }

        private void ConvertToGrayScale(ref Bitmap originalBitmap)
        {
            using (Graphics g = Graphics.FromImage(originalBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(
                          [
                             [.3f,  .3f,  .3f,  0, 0],
                             [.59f, .59f, .59f, 0, 0],
                             [.11f, .11f, .11f, 0, 0],
                             [0,    0,    0,    1, 0],
                             [0,    0,    0,    0, 1]
                          ]);

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(originalBitmap, new System.Drawing.Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                                0, 0, originalBitmap.Width, originalBitmap.Height, GraphicsUnit.Pixel, attributes);
                }
                colorMatrix = null;
            }
        }

        private (string fileName, string filePath) GetFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.png;*.gif";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog().Value)
            {
                string fileName = openFileDialog.SafeFileName;
                string filePath = openFileDialog.FileName;
                openFileDialog = null;
                return (fileName, filePath);
            }
            else
            {
                MessageBox.Show("Zdjęcie nie wybrane");
                openFileDialog = null;
                return (string.Empty, string.Empty);
            }
        }
        #endregion

        private void RgbToGreyButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = (Bitmap)App.FocusedWindow.Bitmap.Clone();

            ConvertToGrayScale(ref bitmap);

            App.FocusedWindow.Bitmap = bitmap;
        }

        public void Negate(ref Mat image)
        {
            if (image.NumberOfChannels != 1)
                throw new ArgumentException("Obraz musi być w skali szarości.");

            // Konwersja Mat na obraz w skali szarości
            Image<Gray, byte> grayImage = image.ToImage<Gray, byte>();

            // Iteracja przez wszystkie piksele obrazu
            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
                    // Negacja wartości piksela
                    grayImage.Data[y, x, 0] = (byte)(255 - grayImage.Data[y, x, 0]);
                }
            }
        }

        private void NegateButton_Click(object sender, RoutedEventArgs e)
        {
            Mat mat = App.FocusedWindow.Image.Mat;

            Negate(ref mat);
            
            App.FocusedWindow.Mat = mat;
        }

        private void SplitChannelButton_Click(object sender, RoutedEventArgs e)
        {
            // Rozdziel obraz na tablicę kanałów
            VectorOfMat channels = new VectorOfMat();
            CvInvoke.Split(App.FocusedWindow.Image, channels);

            Image<Gray, byte> blueChannelImage = channels[0].ToImage<Gray, byte>();
            Image<Gray, byte> greenChannelImage = channels[1].ToImage<Gray, byte>();
            Image<Gray, byte> redChannelImage = channels[2].ToImage<Gray, byte>();

            OpenNewWindow(blueChannelImage, App.FocusedWindow.Title + " BlueChannel");
            OpenNewWindow(greenChannelImage, App.FocusedWindow.Title + " BlueChannel");
            OpenNewWindow(redChannelImage, App.FocusedWindow.Title + " BlueChannel");

        }
    }
}
