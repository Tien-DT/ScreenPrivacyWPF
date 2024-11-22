using OpenCvSharp;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using OpenCvSharp;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace ScreenPrivacy
{
    /// <summary>
    /// Author: Tien Trong
    /// Email: trongtiensp007@gmail.com
    /// </summary>
    public partial class PreviewWindow : System.Windows.Window
    {
        private VideoCapture _capture;
        private CascadeClassifier _faceCascade;
        public PreviewWindow()
        {
            InitializeComponent();
            _capture = new VideoCapture(0);
            if (!_capture.IsOpened())
            {
                MessageBox.Show("Cannot access the webcam. Please check and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string faceCascadePath = System.IO.Path.Combine(appDirectory, "haarcascade_frontalface_default.xml");
            _faceCascade = new CascadeClassifier(faceCascadePath);
            StartWebcamPreview();
        }
        private void StartWebcamPreview()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30); 
            timer.Tick += (s, e) =>
            {
                using var frame = new Mat();
                _capture.Read(frame);
                if (frame.Empty()) return;
                var gray = frame.CvtColor(ColorConversionCodes.BGR2GRAY);
                var faces = _faceCascade.DetectMultiScale(gray, 1.1, 4, HaarDetectionTypes.ScaleImage);
                foreach (var face in faces)
                {
                    Cv2.Rectangle(frame, face, new Scalar(0, 255, 0), 2); 
                }
                PreviewImage.Source = ConvertMatToWriteableBitmap(frame);
            };
            timer.Start();
        }

        private WriteableBitmap ConvertMatToWriteableBitmap(Mat mat)
        {
            if (mat.Empty())
                return null;
            int width = mat.Width;
            int height = mat.Height;
            int stride = width * mat.Channels();
            byte[] buffer = new byte[height * stride];
            Marshal.Copy(mat.Data, buffer, 0, buffer.Length);
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), buffer, stride, 0);

            return writeableBitmap;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _capture?.Release();
            base.OnClosed(e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow._isPreviewing = false;
        }
    }
}
