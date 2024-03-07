using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PrzetwrzanieObrazow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<ImageObject> Images = new List<ImageObject>();
        public static event EventHandler<ImageChangedEventArgs> ImageChanged;

        private static ImageObject currentImage;

        public static ImageObject CurrentImage
        {
            get { return currentImage; }
            set
            {
                if (value != currentImage)
                {
                    currentImage = value;
                    OnImageChanged(new ImageChangedEventArgs(value));
                }
            }
        }

        protected static void OnImageChanged(ImageChangedEventArgs e)
        {
            ImageChanged?.Invoke(null, e);
        }
    }
}
