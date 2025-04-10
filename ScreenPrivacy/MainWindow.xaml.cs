﻿using Hardcodet.Wpf.TaskbarNotification;
using OpenCvSharp;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ScreenPrivacy
{
    /// <summary>
    /// Author: Tien Trong
    /// Email: trongtiensp007@gmail.com
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private VideoCapture _capture;
        private Thread _cameraThread;
        private bool _isFaceDetectionRunning;
        private TaskbarIcon _notifyIcon;
        private CascadeClassifier _faceCascade;
        public static bool _isPreviewing { get; set; } = false;
        [DllImport("user32.dll")]
        private static extern void LockWorkStation();

        public static int timeOut { get; set; } = 3;
        public MainWindow()
        {
            InitializeComponent();
            _notifyIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon("app.ico"),
                ToolTipText = "Screen Privacy App",
                Visibility = Visibility.Visible
            };
            _notifyIcon.TrayMouseDoubleClick += NotifyIcon_TrayMouseDoubleClick;
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string faceCascadePath = System.IO.Path.Combine(appDirectory, "haarcascade_frontalface_default.xml");
            _faceCascade = new CascadeClassifier(faceCascadePath);

        }

        private void StartServiceButton_Click(object sender, RoutedEventArgs e)
        {
            _capture = new VideoCapture(0);
            if (!_capture.IsOpened())
            {
                MessageBox.Show("Can not access to Webcam, Please Check Webcam and Try Again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _isFaceDetectionRunning = true;
            StartServiceButton.IsEnabled = false;
            StopServiceButton.IsEnabled = true;

            _cameraThread = new Thread(FaceDetection);
            _cameraThread.IsBackground = true;
            _cameraThread.Start();
        }

        private void StopServiceButton_Click(object sender, RoutedEventArgs e)
        {
            _isFaceDetectionRunning = false;
            _cameraThread?.Join();
            _capture?.Release();

            StartServiceButton.IsEnabled = true;
            StopServiceButton.IsEnabled = false;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            StopServiceButton_Click(sender, e); 
            _notifyIcon?.Dispose();
            Application.Current.Shutdown();
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void FaceDetection()
        {
            DateTime lastFaceDetectedTime = DateTime.Now;
            while (_isFaceDetectionRunning)
            {
                using var frame = new Mat();
                _capture.Read(frame);
                if (frame.Empty()) continue;
                var gray = frame.CvtColor(ColorConversionCodes.BGR2GRAY);
                var faces = _faceCascade.DetectMultiScale(gray, 1.1, 4, HaarDetectionTypes.ScaleImage);
                if (faces.Length > 0)
                {
                    lastFaceDetectedTime = DateTime.Now;
                }
                else
                {
                    if ((DateTime.Now - lastFaceDetectedTime).TotalSeconds >= timeOut)
                    {
                        LockWorkStation();
                    }
                }
                Thread.Sleep(100);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isFaceDetectionRunning)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                StopServiceButton_Click(null, null); 
                _notifyIcon?.Dispose();
                base.OnClosing(e);
            }
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPreviewing)
            {
                _isPreviewing = false;
                _capture.Release(); 
            }
            else
            {
                _isPreviewing = true;
                var previewWindow = new PreviewWindow();
                previewWindow.Show();
            }
        }

        private void SetTimeOut_Click(object sender, RoutedEventArgs e)
        {
            var setTimeOutWindow = new SetTimeOutWindow();
            setTimeOutWindow.Show();
        }
    }
}