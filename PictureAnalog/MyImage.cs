using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PictureAnalog
{
    public class MyImage
    {
        string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        double _width;
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        double _height;
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}
