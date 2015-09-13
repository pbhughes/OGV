using DirectX.Capture;
using DShowNET.Device;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using NAudio;
using NAudio.CoreAudioApi;
using System.Timers;
using System;
using NAudio.Wave;
using System.Windows.Media;


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
        WaveIn  _audioDevice;
        LinearGradientBrush _yellow = 
            new LinearGradientBrush(Colors.Green, Colors.Yellow, 
                new Point(0, 1), new Point(1, 0));
       

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
                vuMeter.Minimum = 0;
                vuMeter.Maximum = 100;
                vuMeter.Foreground = _yellow;

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

                int waveInDevices = WaveIn.DeviceCount;
                for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
                { 
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                    Console.WriteLine("Device {0}: {1}, {2} channels",
                        waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
                    if (deviceInfo.ProductName.ToLower().Contains("splitcam"))
                    {
                        if (_audioDevice == null)
                        {
                            _audioDevice = new WaveIn();
                        }
                        _audioDevice.DeviceNumber = 0;//waveInDevice;
                        _audioDevice.DataAvailable += _audioDevice_DataAvailable;
                
                        _audioDevice.StartRecording();
                    }

                }
                
                _capture.PreviewWindow = videoPanel;
                _capture.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("Unable to switch to devices  video: {0} audio: {1}", _video.Name, _audio.Name));
            }
        }

        void _audioDevice_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                UpdateVUMeter(Math.Abs(sample32 * 10000));
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

    }
}