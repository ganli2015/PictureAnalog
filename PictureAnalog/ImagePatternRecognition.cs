using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PictureAnalog
{
    public class ImagePatternRecognition
    {
        static public ImagePattern SaveAsPattern(string filename)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(filename);
            ImagePattern pattern = new ImagePattern(FindSimilarImage.GetPixels(bitmap, 5));

            return pattern;
        }
    }
}
