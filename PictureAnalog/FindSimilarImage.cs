using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureAnalog
{
    public class FindSimilarImage
    {
        static int _initStep = 20;
        static int _moveStep = 5;

        static public List<int> GenerateSimilarPicIteration(List<List<System.Drawing.Color>> myImageColorData, Dictionary<int, MyImage> images, int from, int to, double uplimit, int minStep)
        {
            List<int> res = new List<int>();
            List<MyImage> imagesForComputation = new List<MyImage>();
            for (int i = from; i <= to;++i )
            {
                imagesForComputation.Add(images[i]);
            }

            int curStep = _initStep;
            while (curStep >= minStep)
            {
                List<int> similarID = GenerateSimilarPic(myImageColorData, imagesForComputation, uplimit, curStep);
                if (similarID.Count == 0)
                {
                    break;
                }

                imagesForComputation.Clear();
                res.Clear();
                foreach (int id in similarID)
                {
                    imagesForComputation.Add(images[id]);
                    res.Add(id);
                }

                if (curStep == minStep)
                {
                    break;
                }
                else if (curStep - _moveStep <= minStep)
                {
                    curStep = minStep;
                }
                else
                {
                    curStep -= _moveStep;
                }
                
            }

            return res;
        }

        static public List<int> GenerateSimilarPic(List<List<System.Drawing.Color>> myImageColorData, List<MyImage> images, double uplimit, int step)
        {
            List<int> res = new List<int>();

            for (int i = 0; i < images.Count; i++)
            {
                if (Similar(myImageColorData, images[i], uplimit, step))
                {
                    res.Add(images[i].ID);
                }
            }

            return res;
        }

        static private bool Similar(List<List<System.Drawing.Color>> left_colorData, MyImage right, double uplimit, int step)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(right.Path);
            List<List<System.Drawing.Color>> right_colorData = GetPixels(bitmap, step);
            double dis = ComputeDisBetweentColorsList(left_colorData, right_colorData);

            if (dis < 0) return false;
            if (dis < uplimit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private double ComputeDisBetweentColorsList(List<List<System.Drawing.Color>> left, List<List<System.Drawing.Color>> right)
        {
            if (left == null || right == null) return -1;
            if (left.Count == 0 || left.Count == 0) return -1;
//             if (left.Count != right.Count) return -1;
//             if (left[0].Count != right[0].Count) return -1;

            int height_left = left.Count;
            int width_left = left[0].Count;
            int height_right = right.Count;
            int width_right = right[0].Count;
            int minHeight, minWidth;
            double height_interval, width_interval;
            if (height_left >= height_right)
            {
                height_interval = (double)height_left / height_right;
                minHeight = height_right;
            }
            else
            {
                height_interval = (double)height_right / height_left;
                minHeight = height_left;
            }
            if (width_left >= width_right)
            {
                width_interval = (double)width_left / width_right;
                minWidth = width_right;
            }
            else
            {
                width_interval = (double)width_right / width_left;
                minWidth = width_left;
            }

            double curHeight = 0;
            double res = 0;
            for (int i = 0; i < minHeight; ++i)
            {
                double  curWidth = 0;
                for (int j = 0; j < minWidth; j++)
                {
                    if (height_left >= height_right && width_left >= width_right)
                    {
                        res += ComputeDisBetweentColors(left[(int)curHeight][(int)curWidth], right[i][j]);
                    }
                    else if (height_left >= height_right && width_left < width_right)
                    {
                        res += ComputeDisBetweentColors(left[(int)curHeight][j], right[i][(int)curWidth]);
                    }
                    else if (height_left < height_right && width_left < width_right)
                    {
                        res += ComputeDisBetweentColors(left[i][j], right[(int)curHeight][(int)curWidth]);
                    }
                    else
                    {
                        res += ComputeDisBetweentColors(left[i][(int)curWidth], right[(int)curHeight][j]);
                    }


                    curWidth += width_interval;
                }

                curHeight += height_interval;
            }

            return res / (minHeight * minWidth);
        }

        static private double ComputeDisBetweentColors(System.Drawing.Color left, System.Drawing.Color right)
        {
            double a_l = left.A;
            double r_l = left.R;
            double g_l = left.G;
            double b_l = left.B;

            double a_r = right.A;
            double r_r = right.R;
            double g_r = right.G;
            double b_r = right.B;

            double dis = Math.Pow(a_l - a_r, 2) + Math.Pow(r_l - r_r, 2) + Math.Pow(g_l - g_r, 2) + Math.Pow(b_l - b_r, 2);

            return Math.Sqrt(dis);
        }

        static public List<List<System.Drawing.Color>> GetPixels(System.Drawing.Bitmap map, int step)
        {
            List<List<System.Drawing.Color>> res = new List<List<System.Drawing.Color>>();

            try
            {
                for (int i = 0; i < map.Height; i += step)
                {
                    List<System.Drawing.Color> tmp = new List<System.Drawing.Color>();
                    for (int j = 0; j < map.Width; j += step)
                    {
                        tmp.Add(map.GetPixel(j, i));
                    }
                    res.Add(tmp);
                }
            }
            catch (System.Exception ex)
            {
                string error = ex.Message;
            }


            return res;
        }
    }
}
