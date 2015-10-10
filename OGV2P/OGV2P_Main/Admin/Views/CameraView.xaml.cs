using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using System.Timers;
using System;
using System.Windows.Media;


namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        private Timer _timer;
        ISession _sessionService;

        LinearGradientBrush _yellow = 
            new LinearGradientBrush(Colors.Green, Colors.Yellow, 
                new Point(0, 1), new Point(1, 0));
       

        private IDevices _devices;

        public IDevices Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }

     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            try
            {
                vuMeter.Minimum = 0;
                vuMeter.Maximum = 100;
                vuMeter.Foreground = _yellow;


                _sessionService.LocalVideoFile = "";
          
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to switch to devices  video: {0} audio: {1}", "Video Name", "audio Name"));
            }
        }

     
        private void UpdateVUMeter(float sampleVolume)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                vuMeter.Value = (double)sampleVolume;
                //System.Diagnostics.Debug.WriteLine(string.Format("Volume {0} ", vuMeter.Value));
            });
        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCapture();
        }


        public CameraView()
        {
           
        }

        public CameraView(IDevices devices, ISession sessionService)
        {
            InitializeComponent();
            _devices = devices;
            _sessionService = sessionService;
            DataContext = Devices;
        }


    }
}