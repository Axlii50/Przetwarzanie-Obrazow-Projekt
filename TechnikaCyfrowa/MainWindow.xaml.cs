using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Win32;
using PrzetwrzanieObrazow.FunctionWindows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

            if (dialogResult.fileName == string.Empty || dialogResult.fileName == null) return;

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

        public void OpenNewWindow(Mat image, string title)
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
            List<Mat> Channels = hsvSplit(App.FocusedWindow.Mat);
            OpenNewWindow(Channels[0], "HsvR");
            OpenNewWindow(Channels[1], "HsvG");
            OpenNewWindow(Channels[2], "HsvB");
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
            /*if(resizeHistogramWindow.ShowDialog() == true )
            {
                ResizeHistogram(ref mat, (byte?)resizeHistogramWindow.p1, (byte?)resizeHistogramWindow.p2, (byte)resizeHistogramWindow.q3, (byte)resizeHistogramWindow.q4);
            }*/
            App.FocusedWindow.Mat = EqualizeImage(mat.ToImage<Gray,byte>()).Mat;

            //App.FocusedWindow.Mat = mat;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var mat = App.FocusedWindow.Mat;

            SaveFileDialog saveFileDialog = new();

            if(saveFileDialog.ShowDialog() == true)
            {
                Bitmap btm = mat.ToBitmap();
               
                btm.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void StretchContrastButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeHistogramWindow resize = new ResizeHistogramWindow();

            if (resize.ShowDialog() == true)
            {
                var mat = App.FocusedWindow.Mat;
                int p1 = resize.p1;
                int p2 = resize.p2;
                int q3 = resize.q3;
                int q4 = resize.q4;

                mat = StretchContrast(mat, p1, p2, q3, q4);
                App.FocusedWindow.Mat = mat;
            }
        }

        public Mat StretchContrast(Mat mat, int p1, int p2, int q3, int q4)
        {
            Image<Gray, byte> image = mat.ToImage<Gray, byte>();

            double minVal = 255;
            double maxVal = 0;

            for (int y = 0; y < image.Rows; y++)
            {
                for (int x = 0; x < image.Cols; x++)
                {
                    byte pixelVal = image.Data[y, x, 0];
                    if (pixelVal >= p1 && pixelVal <= p2)
                    {
                        if (pixelVal < minVal) minVal = pixelVal;
                        if (pixelVal > maxVal) maxVal = pixelVal;
                    }
                }
            }

            for (int y = 0; y < image.Rows; y++)
            {
                for (int x = 0; x < image.Cols; x++)
                {
                    byte pixelVal = image.Data[y, x, 0];
                    if (pixelVal >= p1 && pixelVal <= p2)
                    {
                        double newVal = ((pixelVal - minVal) / (maxVal - minVal)) * (q4 - q3) + q3;
                        byte newByteVal = (byte)Math.Round(newVal);
                        image.Data[y, x, 0] = newByteVal;
                    }
                }
            }

            return image.Mat;
        }

        private void RgbToLabButton_Click(object sender, RoutedEventArgs e)
        {
            List<Mat> Channels = LabSplit(App.FocusedWindow.Mat);
            OpenNewWindow(Channels[0], "LabR");
            OpenNewWindow(Channels[1], "LabG");
            OpenNewWindow(Channels[2], "LabB");
        }

        public List<Mat> LabSplit(Mat mat)
        {
            Mat newMat = new Mat();
            CvInvoke.CvtColor(mat, newMat, ColorConversion.Bgr2Lab);

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

        private void SkeletonizeButton_Click(object sender, RoutedEventArgs e)
        {
             App.FocusedWindow.Mat = Skeletonize(App.FocusedWindow.Mat);
        }

        public Mat Skeletonize(Mat img)
        {
            Mat skel = new Mat(img.Size, DepthType.Cv8U, 1);
            Mat imCopy = img.Clone();

            // Utworzenie kernela
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Cross, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));

            // Pętla obejmująca kroki 2-4
            while (true)
            {
                // Krok 2: Otwarcie morfologiczne
                Mat imOpen = new Mat();
                CvInvoke.MorphologyEx(imCopy, imOpen, MorphOp.Open, element, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                // Krok 3: Odjęcie powyższego wyniku od obrazu oryginalnego
                Mat imTemp = new Mat();
                CvInvoke.Subtract(imCopy, imOpen, imTemp);

                // Krok 4: Erozja morfologiczna
                Mat imEroded = new Mat();
                CvInvoke.Erode(imCopy, imEroded, element, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                // Aktualizacja szkieletu
                CvInvoke.BitwiseOr(skel, imTemp, skel);

                // Aktualizacja obrazu przetwarzanego
                imCopy = imEroded.Clone();

                // Sprawdzenie warunku zakończenia pętli
                if (CvInvoke.CountNonZero(imCopy) == 0)
                    break;
            }

            return skel;
        }

        private void DistanceTransform_Click(object sender, RoutedEventArgs e)
        {
            _ = DistanceTransform(App.FocusedWindow.Mat);
        }

        private Mat DistanceTransform(Mat mat)
        {
            Mat mask = new Mat();
            //CvInvoke.InRange(mat, new ScalarArray(new MCvScalar(0, 0, 0)), new ScalarArray(new MCvScalar(255, 255, 255)), mask);

            CvInvoke.BitwiseNot(mat, mask);

            //// Ustaw piksele na czarne zgodnie z maską
            //mat.SetTo(new MCvScalar(0, 0, 0), mask);

            ////CvInvoke.Imshow("BG black", mask);
            CvInvoke.Imshow("Negateed", mask);

            // Utwórz kernel
            float[,] kernelData2D = new float[,]
            {
                { 1, 1, 1 },
                { 1, -8, 1 },
                { 1, 1, 1 }
            };

            float[] kernelData = new float[kernelData2D.Length];
            Buffer.BlockCopy(kernelData2D, 0, kernelData, 0, kernelData2D.Length * sizeof(float));

            Mat kernel = new Mat(3, 3, DepthType.Cv32F, 1);
            Marshal.Copy(kernelData, 0, kernel.DataPointer, kernelData.Length);

            // Wykonaj filtrowanie Laplace'a
            Mat imgLaplacian = new Mat(mat.Size, mat.Depth, mat.NumberOfChannels);
            CvInvoke.Filter2D(mat, imgLaplacian, kernel, new System.Drawing.Point(-1, -1), 0, BorderType.Default);

            // Przekształć obraz źródłowy na głębię CV_32F
            Mat sharp = new Mat(imgLaplacian.Size, imgLaplacian.Depth, imgLaplacian.NumberOfChannels);
            mat.ConvertTo(sharp, DepthType.Cv32F);

            // Upewnij się, że obrazy mają te same rozmiary
            if (sharp.Size != imgLaplacian.Size)
            {
                CvInvoke.Resize(imgLaplacian, imgLaplacian, sharp.Size);
            }

            // Upewnij się, że obrazy mają tę samą liczbę kanałów
            if (sharp.NumberOfChannels != imgLaplacian.NumberOfChannels)
            {
                CvInvoke.CvtColor(imgLaplacian, imgLaplacian, ColorConversion.Gray2Bgr);
            }

            // Odejmij filtr Laplace'a od źródła
            Mat imgResult = new Mat(imgLaplacian.Size, imgLaplacian.Depth, imgLaplacian.NumberOfChannels);
            CvInvoke.Subtract(sharp, imgLaplacian, imgResult, null, DepthType.Cv32F);

            // Konwersja z powrotem do CV_8U
            imgResult.ConvertTo(imgResult, DepthType.Cv8U);
            imgLaplacian.ConvertTo(imgLaplacian, DepthType.Cv8U);

            // Wyświetl przefiltrowany obraz
            //OpenNewWindow(imgResult, "2");
            CvInvoke.Imshow("New Sharpened Image", imgResult);

            // Zastosuj DistanceTransform do wyniku
            //Mat transformedResult = DistanceTransform(imgResult);

            // OpenNewWindow(transformedResult, "2");

            // Konwertuj obraz na skalę szarości
            Mat bw = new Mat();
            CvInvoke.CvtColor(imgResult, bw, ColorConversion.Bgr2Gray);

            // Wykonaj progowanie (thresholding)
            CvInvoke.Threshold(bw, bw, 40, 255, ThresholdType.Binary | ThresholdType.Otsu);
            CvInvoke.BitwiseNot(bw, bw);

            // Wyświetl binarny obraz
            //OpenNewWindow(bw, "3");
            CvInvoke.Imshow("Binary Image", bw);

            Mat dist = new Mat();
            CvInvoke.DistanceTransform(bw, dist, null, DistType.L2, 3);

            // Normalizacja obrazu transformacji odległościowej
            CvInvoke.Normalize(dist, dist, 0, 1.0, NormType.MinMax);

            // Wyświetlenie obrazu transformacji odległościowej
            CvInvoke.Imshow("Distance Transform Image", dist);

            Mat peaks = new Mat();
            // Progowanie obrazu transformacji odległościowej
            CvInvoke.Threshold(dist, peaks, 0.4, 1.0, ThresholdType.Binary);

            // Dylatacja obrazu
            Mat kernel1 = Mat.Ones(3, 3, DepthType.Cv8U, 1);
            CvInvoke.Dilate(peaks, peaks, kernel1, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0));

            CvInvoke.Imshow("Peaks", peaks);


            Mat dist8u = new Mat();
            dist.ConvertTo(dist8u, DepthType.Cv8U);

            // Find total markers
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(dist8u, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Create the marker image for the watershed algorithm
            Mat markers = Mat.Zeros(dist.Rows, dist.Cols, DepthType.Cv32S, 1);

            // Draw the foreground markers
            for (int i = 0; i < contours.Size; i++)
            {
                CvInvoke.DrawContours(markers, contours, i, new MCvScalar(i + 1), -1);
            }

            // Draw the background marker
            CvInvoke.Circle(markers, new System.Drawing.Point(5, 5), 3, new MCvScalar(255), -1);

            // Convert markers to 8U type for display
            Mat markers8u = new Mat();
            markers.ConvertTo(markers8u, DepthType.Cv8U, 10);

            // Show the markers
            CvInvoke.Imshow("Markers", markers8u);

            // Perform the watershed algorithm
            CvInvoke.Watershed(imgResult, markers);

            // Convert markers to 8U
            Mat mark = new Mat();
            markers.ConvertTo(mark, DepthType.Cv8U);
            CvInvoke.BitwiseNot(mark, mark);

            // Optionally display the marker image at this point
            // CvInvoke.Imshow("Markers_v2", mark);

            // Generate random colors
            List<MCvScalar> colors = new List<MCvScalar>();
            Random rng = new Random();
            for (int i = 0; i < contours.Size; i++)
            {
                int b = rng.Next(0, 256);
                int g = rng.Next(0, 256);
                int r = rng.Next(0, 256);
                colors.Add(new MCvScalar(b, g, r));
            }

            // Create the result image
            Mat dst = new Mat(markers.Size, DepthType.Cv8U, 3);
            dst.SetTo(new MCvScalar(0, 0, 0)); // Initialize with zeros

            // Fill labeled objects with random colors
            for (int i = 0; i < markers.Rows; i++)
            {
                for (int j = 0; j < markers.Cols; j++)
                {
                    int index = GetMarkerValue(markers, i, j);
                    if (index > 0 && index <= contours.Size)
                    {
                        MCvScalar color = colors[index - 1];
                        SetPixelColor(dst, i, j, color);
                    }
                }
            }

            // Visualize the final image
            CvInvoke.Imshow("Final Result", dst);

            return markers8u;
        }

        private int GetMarkerValue(Mat markers, int row, int col)
        {
            // Access the pointer to the data
            IntPtr dataPtr = markers.DataPointer;
            // Calculate the offset
            int offset = row * markers.Cols + col;
            // Read the value from the pointer
            int value = Marshal.ReadInt32(dataPtr, offset * sizeof(int));
            return value;
        }

        private void SetPixelColor(Mat img, int row, int col, MCvScalar color)
        {
            // Access the pointer to the data
            IntPtr dataPtr = img.DataPointer;
            // Calculate the offset
            int offset = (row * img.Cols + col) * 3; // 3 channels for BGR
                                                     // Write the values to the pointer
            Marshal.WriteByte(dataPtr, offset, (byte)color.V0); // Blue channel
            Marshal.WriteByte(dataPtr, offset + 1, (byte)color.V1); // Green channel
            Marshal.WriteByte(dataPtr, offset + 2, (byte)color.V2); // Red channel
        }
    }
}
