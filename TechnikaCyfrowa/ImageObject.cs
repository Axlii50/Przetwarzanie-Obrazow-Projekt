using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace PrzetwrzanieObrazow
{
    public class ImageObject
    {
        public string Name { get; set; }

        public Emgu.CV.Image<Bgr, byte> Image { get; set; }

        public Bitmap Bitmap { 
            get
            {
                return this.Image.ToBitmap();
            }
            set
            {
                Image = value.ToImage<Bgr, byte>();
            }
        }
    }
}
