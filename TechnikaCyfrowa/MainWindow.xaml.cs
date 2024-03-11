﻿using Microsoft.Win32;
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

        public void Negate(ref Bitmap image)
        {
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int byteCount = bmData.Stride * image.Height;
            byte[] pixels = new byte[byteCount];

            System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, pixels, 0, byteCount);

            for (int i = 0; i < byteCount; i += bytesPerPixel)
            {
                pixels[i] = (byte)(255 - pixels[i]); // Blue
                if (bytesPerPixel > 1)
                {
                    pixels[i + 1] = (byte)(255 - pixels[i + 1]); // Green
                    pixels[i + 2] = (byte)(255 - pixels[i + 2]); // Red
                }
                // Asumujemy, że czwarty bajt (jeśli istnieje) to alfa i pozostawiamy go bez zmian
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmData.Scan0, byteCount);
            image.UnlockBits(bmData);
            pixels = null;
            bmData = null;
            GC.Collect();
        }

        private void NegateButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = (Bitmap)App.FocusedWindow.Bitmap.Clone();

            Negate(ref bitmap);
            
            App.FocusedWindow.Bitmap = bitmap;
            GC.Collect();
        }
    }
}
