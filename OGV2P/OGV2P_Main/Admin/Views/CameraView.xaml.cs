using DirectX.Capture;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using NAudio;
using NAudio.CoreAudioApi;
using System.Timers;
using System;


namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        private Timer _timer;
        Filter _video = null;
        Filter _audio = null;
        Capture _capture = null;

        private IDevices _devices;

        public IDevices Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }

        public CameraView(IDevices devices)
        {
            InitializeComponent();
            _devices = devices;
            DataContext = Devices;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            try
            {
                if (_capture != null)
                {
                    _capture.Stop();
                    _capture.Dispose();
                }

                Filters filters = new Filters();



                foreach (Filter v in filters.VideoInputDevices)
                {
                    if (v.Name == cboCameras.SelectedValue.ToString())
                    {
                        _video = v;
                        break;
                    }

                }

                foreach (Filter c in filters.AudioInputDevices)
                {
                    if (c.Name == cboMicrophones.SelectedValue.ToString())
                    {
                        _audio = c;
                        break;
                    }

                }

                _capture = new Capture(
                    _video,
                    _audio
                );

                //_timer = new Timer(250);
                //_timer.Elapsed += _timer_Elapsed;
                //_timer.Start();

                _capture.PreviewWindow = videoPanel;

                _capture.Start();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(string.Format("Unable to switch to devices  video: {0} audio: {1}", _video.Name, _audio.Name ));
            }
            
           
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            

            this.Dispatcher.InvokeAsync( () =>
            {
                //UpdateVUMeter(_audio);
              
            });
           
        }

        private void UpdateVUMeter(Filter audio)
        {
            MMDeviceEnumerator de = new MMDeviceEnumerator();
            MMDevice nDevice = null;
            if (de.HasDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                nDevice = de.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            vuMeter.Value = (double)nDevice.AudioEndpointVolume.MasterVolumeLevel;
            System.Diagnostics.Debug.WriteLine(string.Format("Volue {0} ", vuMeter.Value));
        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCapture();
        }


        public CameraView()
        {
           
        }

    }
}