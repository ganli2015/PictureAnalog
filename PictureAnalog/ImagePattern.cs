using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureAnalog
{
    public class ImagePattern
    {
        List<List<System.Drawing.Color>> _colorData;

        public ImagePattern()
        {
            _colorData = new List<List<System.Drawing.Color>>();
        }

        public ImagePattern(List<List<System.Drawing.Color>> colorData)
        {
            _colorData = colorData;
        }
    }
}
