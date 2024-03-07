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
            App.ImageChanged += App_ImageChanged;
        }

        private void App_ImageChanged(object? sender, ImageChangedEventArgs e)
        {
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               e.NewImage.Bitmap.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());

            this.Image.Source = bitmapSource;
        }

        private async void GrayScaleLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = GetFile();

            if (dialogResult.fileName == string.Empty) return;

            Bitmap originalBitmap = new Bitmap(dialogResult.filePath);

            await Task.Run(() => ConvertToGrayScale(ref originalBitmap));

            var ImageObject = new ImageObject()
            {
                Bitmap = originalBitmap,
                Name = "Grey"+dialogResult.fileName
            };

            var imageControl = new ImageControl(ImageObject.Name, ImageObject.Bitmap);

            this.ImageWrapPanel.Children.Add(imageControl);
            App.Images.Add(ImageObject);
        }

        private void ColorScaleLoadButton_Click(object sender, RoutedEventArgs e)
        {
            (string fileName, string filePath) dialogResult = GetFile();

            if (dialogResult.fileName == string.Empty) return;

            var ImageObject = new ImageObject()
            {
                Bitmap = new System.Drawing.Bitmap(dialogResult.filePath),
                Name = dialogResult.fileName
            };

            var imageControl = new ImageControl(dialogResult.fileName, ImageObject.Bitmap);

            this.ImageWrapPanel.Children.Add(imageControl);
            App.Images.Add(ImageObject);
        }
        private void HistogramGraphicButton_Click(object sender, RoutedEventArgs e)
        {
            HistogramGraphic hist = new HistogramGraphic(App.CurrentImage.Bitmap);
            hist.Show();
        }
        private void HistogramTableButton_Click(object sender, RoutedEventArgs e)
        {
            HistogramTable hist = new HistogramTable(App.CurrentImage.Bitmap);
            hist.Show();
        }

        #region Functions

        private void ConvertToGrayScale(ref Bitmap originalBitmap)
        {
            using (Graphics g = Graphics.FromImage(originalBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(
                          new float[][]
                          {
                             [.3f, .3f, .3f, 0, 0],
                             [.59f, .59f, .59f, 0, 0],
                             [.11f, .11f, .11f, 0, 0],
                             [0, 0, 0, 1, 0],
                             [0, 0, 0, 0, 1]
                          });

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(originalBitmap, new System.Drawing.Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                                0, 0, originalBitmap.Width, originalBitmap.Height, GraphicsUnit.Pixel, attributes);
                }
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

                return (fileName, filePath);
            }
            else
            {
                MessageBox.Show("Zdjęcie nie wybrane");
                return (string.Empty, string.Empty);
            }
        }
        #endregion

        
    }
}
