using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzetwrzanieObrazow
{
    public class ImageChangedEventArgs : EventArgs
    {
        public ImageObject NewImage { get; set; }

        public ImageChangedEventArgs(ImageObject newImage)
        {
            NewImage = newImage;
        }
    }
}
