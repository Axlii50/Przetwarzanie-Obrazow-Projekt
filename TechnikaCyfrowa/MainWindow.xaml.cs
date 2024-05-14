using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Win32;
using PrzetwrzanieObrazow.FunctionWindows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;


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

            Bitmap originalBitmap = new Bitmap(dialogResult.filePath);//do zmiany wczytywanie

            await Task.Run(() => ConvertToGrayScale(ref originalBitmap));

            OpenNewWindow(originalBitmap, dialogResult.fileName);
        }

        private void ColorScaleLoadButton_Click(object sender, RoutedEventArgs e)
        {
            (string fileName, string filePath) dialogResult = GetFile();

            if (dialogResult.fileName == string.Empty) return;

            OpenNewWindow(new Bitmap(dialogResult.filePath), dialogResult.fileName);
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

        private void ResizeHistogramButton_Click(object sender, RoutedEventArgs e)
        {
            Mat mat = App.FocusedWindow.Gray.Mat;

            ResizeHistogram(ref mat);

            App.FocusedWindow.Mat = mat;
            App.FocusedWindow.OpenHistogramGraphic();
        }

        public void ResizeHistogram(ref Mat image, byte? p1 = null, byte? p2 = null, byte q3 = 0, byte q4 = 255)
        {
            if(p1 is null || p2 is null)
            {
                CvInvoke.MinMaxIdx(image, out double tp1, out double tp2, null, null);
                p1 = (byte)tp1;
                p2 = (byte)tp2;
            }

            int height = image.Height;
            int step = image.Step;

            byte[] imageData = new byte[height * step];
            GCHandle handle = GCHandle.Alloc(imageData, GCHandleType.Pinned);

            try
            {
                IntPtr pointer = image.DataPointer;

                // Kopiowanie danych obrazu do tablicy
                Marshal.Copy(pointer, imageData, 0, imageData.Length);

                // Przetwarzanie danych obrazu
                for (int i = 0; i < imageData.Length; i++)
                {
                    double pixel = imageData[i];
                    pixel = q3 + (pixel - p1.Value) * (q4 - q3) / (p2.Value - p1.Value);
                    imageData[i] = (byte)Math.Max(Math.Min(pixel, 255), 0);
                }

                // Kopiowanie przetworzonych danych z powrotem do niezarządzanej pamięci obrazu
                Marshal.Copy(imageData, 0, pointer, imageData.Length);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        private void RgbToHSVButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<Mat> hsvSplit(Mat mat)
        {
            Mat newMat = new Mat();
            CvInvoke.CvtColor(mat, newMat, ColorConversion.Bgr2Hsv);

            VectorOfMat vector = new VectorOfMat();
            CvInvoke.Split(newMat, vector);
            List<Mat> Channels = new List<Mat>();

            for (int i = 0; i < vector.Size; i++)
            {
                Mat channel = vector[i];
                Mat grayChannel = channel.Clone();

                Channels.Add(grayChannel);
            }

            return Channels;
        }

        private void EqualizeTableButton_Click(object sender, RoutedEventArgs e)
        {
            Mat mat = App.FocusedWindow.Gray.Mat;
            ResizeHistogramWindow resizeHistogramWindow = new ResizeHistogramWindow();
            if(resizeHistogramWindow.ShowDialog() == true )
            {
                ResizeHistogram(ref mat, (byte?)resizeHistogramWindow.p1, (byte?)resizeHistogramWindow.p2, (byte)resizeHistogramWindow.q3, (byte)resizeHistogramWindow.q4);
            }

            App.FocusedWindow.Mat = mat;
            App.FocusedWindow.OpenHistogramGraphic();
        }

        public static Image<Gray, byte> EqualizeImage(Image<Gray, byte> image)
        {
            byte[,,] data = image.Data;
            int width = image.Width;
            int height = image.Height;

            // Krok 1: Obliczenie histogramu
            int[] histogram = new int[256];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    histogram[data[y, x, 0]]++;
                }
            }

            // Krok 2: Obliczenie skumulowanego histogramu (CDF)
            int[] cdf = new int[256];
            cdf[0] = histogram[0];
            for (int i = 1; i < 256; i++)
            {
                cdf[i] = cdf[i - 1] + histogram[i];
            }

            // Normalizacja CDF
            float cdfMin = cdf[0];
            float range = cdf[255] - cdfMin;
            float[] cdfNormalized = new float[256];
            for (int i = 0; i < 256; i++)
            {
                cdfNormalized[i] = (cdf[i] - cdfMin) / range * 255;
            }

            // Krok 3: Mapowanie pikseli do wyrównanych wartości
            Image<Gray, byte> equalizedImage = image.CopyBlank();
            byte[,,] equalizedData = equalizedImage.Data;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    equalizedData[y, x, 0] = Convert.ToByte(cdfNormalized[data[y, x, 0]]);
                }
            }

            return equalizedImage;
        }

        private void PosterizeButton_Click(object sender, RoutedEventArgs e)
        {
            int layers = 0;
            InputWindow posterizeWindowInput = new InputWindow();
            if (posterizeWindowInput.ShowDialog() == true)
                layers = posterizeWindowInput.inputValue;

            if (layers <= 0) return;

            var currentmat = App.FocusedWindow.Mat;

            var newmat = Posterize(currentmat, layers);

            App.FocusedWindow.Mat = newmat;
        }

        public static Mat Posterize(Mat image, int levels)
        {
            const int divisorconst = 256;
            Mat posterizedImage = image.Clone();

            int divisor = divisorconst / levels;

            int step = posterizedImage.Step;
            IntPtr dataPtr = posterizedImage.DataPointer;
            int byteCount = posterizedImage.Cols * step;

            for (int i = 0; i < byteCount; ++i)
            {
                byte byteValue = Marshal.ReadByte(dataPtr, i);
                byte intensity = (byte)(Math.Round(byteValue / (double)divisor) * divisor);

                Marshal.WriteByte(dataPtr, i, intensity);
            }

            return posterizedImage;
        }

        private void NeighBourButton_Click(object sender, RoutedEventArgs e)
        {
            NeighboutOperations neighboutOperations = new NeighboutOperations();
            neighboutOperations.Show();
        }

        private void MorfologicOperations_Click(object sender, RoutedEventArgs e)
        {
            var mat = App.FocusedWindow.Mat;
            MorphologicOperationsWindow window = new MorphologicOperationsWindow();
            if (window.ShowDialog() == true)
            {
                BorderType borderT = window.borderT;
                MorphOp MorphOperation = window.MorphOperation;
                ElementShape EleShape = window.EleShape;

                Mat element = CvInvoke.GetStructuringElement(EleShape, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
                Mat result = new Mat();
                CvInvoke.MorphologyEx(mat, result, MorphOperation, element, new System.Drawing.Point(-1, -1), 1, borderT, new MCvScalar());

                App.FocusedWindow.Mat = result;
            }
        }
    }
}
