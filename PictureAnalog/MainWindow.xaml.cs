using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;

namespace PictureAnalog
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    struct ColorDataArg
    {
        public int from;
        public int to;
        public int step;

        public List<List<System.Drawing.Color>> myColorData;
        public Dictionary<int, MyImage> images;
    }

    struct ReadImagesArg
    {
        public List<string> path_jpg;

        
    }

    public partial class MainWindow : Window
    {
        Dictionary<int,MyImage> _images;
        List<int> _similarID;
        MyImage _selectedPic;
        Canvas _lastSelectCanvas;

        int _viewedImageID;
        int _showPicNum;
        double _initCanvasHeight;
        double _initCanvasWidth;

        int _pixelAnalysisStep ;
        int _maxId;
        double _similarLimit;
        bool _loadImageFinished;
        string _selectDir;
        int _bwWorkCompleteNum;
        bool _loadFirstPageImageFinished;

        public MainWindow()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            _images = new Dictionary<int, MyImage>();
            _maxId = -1;
            _pixelAnalysisStep = 5;
            _similarLimit = 90;
            _loadFirstPageImageFinished = false;
            _loadImageFinished = false;
            slider1.Value = _similarLimit;
            _viewedImageID = -1;
            _showPicNum = 4;
            button3.IsEnabled = false;
            _initCanvasHeight = grid1.Height / 2;
            _initCanvasWidth = grid1.Width / 2;
            _bwWorkCompleteNum = 0;

            foreach (Canvas canvas in grid1.Children)
            {
                if (canvas == null) continue;

                canvas.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_LeftMouseUp);
            }

           
        }

       

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择文件夹";
            fbd.RootFolder = System.Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = "D:\\ ";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _loadFirstPageImageFinished = false;
                _selectDir = fbd.SelectedPath;
//                 BackgroundWorker bw = new BackgroundWorker();
//                 bw.DoWork += new DoWorkEventHandler(BackgroundLoadImage);
//                 bw.RunWorkerAsync();
// 
//                 while (!_loadFirstPageImageFinished) ;
                _viewedImageID = 0;
                LoadImagesData();
                ShowImages(_viewedImageID, _showPicNum);
            }

        }

        private void BackgroundLoadImage(object sender, DoWorkEventArgs e)
        {
            LoadImagesData();
        }

        private void LoadImagesData()
        {

            var tmpImage_jpg = Directory.EnumerateFiles(_selectDir, "*.jpg");

            List<string> path_jpg = new List<string>();
            foreach (string currentFile in tmpImage_jpg)
            {
                path_jpg.Add(currentFile);
            }

            int count = 0;
            foreach (string currentFile in path_jpg)
            {
                MyImage image = new MyImage();
                image.Path = currentFile;
                Uri uri = new Uri(currentFile);
                BitmapImage bi=null;
                try
                {
                    bi = new BitmapImage(uri);

                }
                catch (System.Exception ex)
                {
                    continue;
                }         
                image.Width = bi.Width;
                image.Height = bi.Height;
                image.ID = ++_maxId;
                _images[image.ID] = image;

                count++;
                if (count == 4)
                {
                    _loadFirstPageImageFinished = true;
                }
            }


            _loadFirstPageImageFinished = true;
            _loadImageFinished = true;
        }

        private void BeginBackComputation(int MyID)
        {
            _bwWorkCompleteNum = 0;

            List<int> splitIndex = new List<int>();
            splitIndex.Add((int)(_images.Count / 3.0));
            splitIndex.Add((int)(_images.Count * 2 / 3.0));

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(_images[MyID].Path);
            List<List<System.Drawing.Color>> myColorData = FindSimilarImage.GetPixels(bitmap, _pixelAnalysisStep);

            ColorDataArg arg1 = new ColorDataArg();
            arg1.from = 0;
            arg1.to = splitIndex[0];
            arg1.step = _pixelAnalysisStep;
            arg1.images = _images;
            arg1.myColorData = myColorData;
            BackgroundWorker bw1 = new BackgroundWorker();
            bw1.DoWork += new DoWorkEventHandler(Work_GenerateColorData);
            bw1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Work_GenerateSimilarPic_Complete);
            bw1.RunWorkerAsync(arg1);

            ColorDataArg arg2 = new ColorDataArg();
            arg2.from = splitIndex[0]+1;
            arg2.to = splitIndex[1];
            arg2.step = _pixelAnalysisStep;
            arg2.images = _images;
            arg2.myColorData = myColorData;
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(Work_GenerateColorData);
            bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Work_GenerateSimilarPic_Complete);
            bw2.RunWorkerAsync(arg2);

            ColorDataArg arg3 = new ColorDataArg();
            arg3.from = splitIndex[1] + 1;
            arg3.to = _images.Count-1;
            arg3.step = _pixelAnalysisStep;
            arg3.images = _images;
            arg3.myColorData = myColorData;
            BackgroundWorker bw3 = new BackgroundWorker();
            bw3.DoWork += new DoWorkEventHandler(Work_GenerateColorData);
            bw3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Work_GenerateSimilarPic_Complete);
            bw3.RunWorkerAsync(arg3);
        }

        private void BeginBackComputation_test(int MyID)
        {
            _bwWorkCompleteNum = 0;

            List<int> splitIndex = new List<int>();
            splitIndex.Add((int)(_images.Count / 3.0));
            splitIndex.Add((int)(_images.Count * 2 / 3.0));

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(_images[MyID].Path);
            List<List<System.Drawing.Color>> myColorData = FindSimilarImage.GetPixels(bitmap, _pixelAnalysisStep);

            ColorDataArg arg1 = new ColorDataArg();
            arg1.from = 0;
            arg1.to = _images.Count - 1;
            arg1.step = _pixelAnalysisStep;
            arg1.images = _images;
            arg1.myColorData = myColorData;
            BackgroundWorker bw1 = new BackgroundWorker();
            bw1.DoWork += new DoWorkEventHandler(Work_GenerateColorData);
            bw1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Work_GenerateSimilarPic_Complete);
            bw1.RunWorkerAsync(arg1);

            button1.IsEnabled = true;
            button2.IsEnabled = true;
        }

        private void Work_GenerateColorData(object sender, DoWorkEventArgs e)
        {
            ColorDataArg arg = (ColorDataArg)e.Argument;

            e.Result = FindSimilarImage.GenerateSimilarPicIteration(arg.myColorData, arg.images, arg.from, arg.to, _similarLimit,_pixelAnalysisStep);
            
        }

        private void Work_GenerateSimilarPic_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                List<int> res = (List<int>)e.Result;
                foreach (int id in res)
                {
                    _similarID.Add(id);

                }

                if (res == null) return;

                foreach (int id in res)
                {
                    MyImage image = _images[id];

                    Canvas canvas = new Canvas();
                    canvas.Tag = image.ID;
                    canvas.Background = GetImageBrush(image.Path);
                    canvas.Height = image.Height / 10;
                    canvas.Width = image.Width / 10;
                    canvas.SetValue(WrapPanel.MarginProperty, new Thickness(10));
                    wrapPanel2.Children.Add(canvas);
                }

                if (_bwWorkCompleteNum != 2)
                {
                    _bwWorkCompleteNum++;
                }
                else
                {
                    button1.IsEnabled = true;
                    button2.IsEnabled = true;
                    MessageBox.Show("寻找结束！");
                }
            }
        }

   

        private void Canvas_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_lastSelectCanvas != null)
            {
                _lastSelectCanvas.Effect = null;
            }

            Canvas canvas = sender as Canvas;
            DropShadowEffect shadow = new DropShadowEffect();
            shadow.BlurRadius = 20;
            canvas.Effect = shadow;
            _lastSelectCanvas = canvas;

            int id = (int)canvas.Tag;

            foreach (KeyValuePair<int,MyImage > image in _images)
            {
                if (image.Value.ID == id)
                {
                    _selectedPic = image.Value;
                    return;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPic == null) return;

            if (!_loadImageFinished)
            {
                string message="图片未载入完，如果一定要寻找的话软件可能崩溃，也可能找不全图片。确定要继续？";
                if (MessageBoxResult.OK == MessageBox.Show(message, "警告", MessageBoxButton.OKCancel))
                {
                    button1.IsEnabled = false;
                    button2.IsEnabled = false;
                    _similarID = new List<int>();
                    wrapPanel2.Children.Clear();
                    BeginBackComputation(_selectedPic.ID);
                }
                else
                {
                    return;
                }
            }
            else
            {
                button1.IsEnabled = false;
                button2.IsEnabled = false;
                _similarID = new List<int>();
                wrapPanel2.Children.Clear();
                BeginBackComputation(_selectedPic.ID);
            }

           
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = e.NewValue;
            _similarLimit = val;
        }

        private ImageBrush GetImageBrush(string currentFile)
        {
            BitmapImage bi = new BitmapImage(new Uri(currentFile));
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bi;

            return brush;
        }

        private void ShowImages(int startID,int picNum)
        {
            foreach (Canvas canvas in grid1.Children)
            {
                if (canvas != null)
                {
                    canvas.Background = null;
                }
            }


            List<int> imageIDs = new List<int>();
            for (int i = startID; i < (_viewedImageID + picNum); ++i)
            {
                if (_images.ContainsKey(i))
                {
                    imageIDs.Add(i);

                    
                }
            }

            for (int i = 0; i < imageIDs.Count;++i )
            {
                MyImage image = _images[imageIDs[i]];
                Canvas canvas = grid1.Children[i] as Canvas;
                canvas.Tag = image.ID;

                double canvas_height = _initCanvasHeight;
                double canva_width = _initCanvasWidth;
                if (image.Height > image.Width)
                {
                    canvas.Width = canvas_height / image.Height * image.Width;
                    canvas.Height = canvas_height;
                }
                else
                {
                    canvas.Height = canva_width / image.Width * image.Height;
                    canvas.Width = canva_width;
                }

                canvas.Background = GetImageBrush(image.Path);
                canvas.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                canvas.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if ((_viewedImageID - _showPicNum) < 0) return;

            _viewedImageID -=_showPicNum;
            ShowImages(_viewedImageID, _showPicNum);
            if ((_viewedImageID - _showPicNum) < 0)
            {
                button3.IsEnabled = false;
            }
            else
            {
                button3.IsEnabled = true;
            }

            if ((_viewedImageID + _showPicNum) > (_images.Count - 1))
            {
                button4.IsEnabled = false;
            }
            else
            {
                button4.IsEnabled = true;
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if ((_viewedImageID + _showPicNum) > (_images.Count - 1)) return;

            _viewedImageID += _showPicNum;
            ShowImages(_viewedImageID, _showPicNum);
            if ((_viewedImageID + _showPicNum) > (_images.Count - 1))
            {
                button4.IsEnabled = false;
            }
            else
            {
                button4.IsEnabled = true;
            }

            if ((_viewedImageID - _showPicNum) < 0)
            {
                button3.IsEnabled = false;
            }
            else
            {
                button3.IsEnabled = true;
            }
        }
    }

}
