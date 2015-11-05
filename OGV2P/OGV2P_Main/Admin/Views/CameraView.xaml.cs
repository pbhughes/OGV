using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using System.Timers;
using System;
using System.Windows.Media;
using AxRTMPActiveX;
using RTMPActiveX;
using System.Diagnostics;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.IO;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using Forms = System.Windows.Forms;
using System.Windows.Interop;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl, INavigationAware, IRegionMemberLifetime, INotifyPropertyChanged
    {
        private const string RECORDING_IN_PROGRESS = "Recording in progress, please stop recording before changing devices";
        ISession _sessionService;
        private System.Timers.Timer cpuReadingTimer;
        private PerformanceCounter cpuCounter;
        private Timer _vuMeterTimer;
        private Forms.NativeWindow _window = new Forms.NativeWindow();

        LinearGradientBrush _yellow = 
            new LinearGradientBrush(Colors.Green, Colors.Yellow, 
                new Point(0, 1), new Point(1, 0));

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public bool KeepAlive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private List<string> _cameras;
        public List<string> Cameras
        {
            get
            {
                return _cameras;
            }

            set
            {
                _cameras = value;
                OnPropertyChanged("Cameras");
            }
        }

        private List<string> _microphones;
        public List<string> Microphones
        {
            get
            {
                return _microphones;
            }

            set
            {
                _microphones = value;
                OnPropertyChanged("Microphones");
            }
        }



        private void UpdateVUMeter(int sampleVolume)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                vuMeter.Value = (double)sampleVolume;
            });
        }

       


        public CameraView()
        {
           
        }

        public CameraView(IDevices devices, ISession sessionService)
        {
            InitializeComponent();

            try
            {
                this.DataContext = this;
                _sessionService = sessionService;
                axRControl.License = "nlic:1.2:LiveEnc:3.0:LvApp=1,LivePlg=1,MSDK=4,MPEG2DEC=1,MPEG2ENC=1,PS=1,TS=1,H264DEC=1,H264ENC=1,H264ENCQS=1,MP4=4,RTMPsrc=1,RtmpMsg=1,RTMPs=1,RTSP=1,RTSPsrc=1,UDP=1,UDPsrc=1,HLS=1,WMS=1,WMV=1,RTMPm=4,RTMPx=3,Resz=1,RSrv=1,VMix2=1,3DRemix=1,ScCap=1,AuCap=1,AEC=1,Demo=1,Ic=1,NoMsg=1,Tm=1800,T1=600,NoIc=1:win,win64,osx:20151030,20160111::0:0:nanocosmosdemo-292490-3:ncpt:f6044ea043c479af5911e60502f1a334";
                axRControl.InitEncoder();

                _cameras = new List<string>();
                _microphones = new List<string>();

                //initialize the window to listen for usb devices to be added
                OGV2P.Admin.Helpers.UsbNotification.RegisterUsbDeviceNotification(_window.Handle);

                //set the user id / password
                //axRControl.SetConfig("Auth", "barkley:hughes");

                var settings = ConfigurationSettings.AppSettings;

                // Video Source Device (0...n)
                axRControl.VideoSource = 0;

                // Device/Camera Resolution
                axRControl.VideoWidth = 320;
                axRControl.VideoHeight = 240;
                axRControl.VideoFrameRate = 25;

                // Video Encoder Bitrate (Bits/s)
                axRControl.VideoBitrate = 500000;

                axRControl.TextOverlayText = "Hello World!";
                //axRTMPActiveX1.TextOverlayText = "c:\\temp\\icon.png";
                axRControl.VideoEffect = 3;

                // nanoStream Event Handlers
                axRControl.OnEvent += new AxRTMPActiveX.IRTMPActiveXEvents_OnEventEventHandler(axRControl_OnEvent);
                axRControl.OnStop += new AxRTMPActiveX.IRTMPActiveXEvents_OnStopEventHandler(axRControl_OnStop);

                // Video/Audio Devices
                int n;
                AddVideoSources();
                cboCameras.SelectedItem = axRControl.GetVideoSource(0);

                AddAudioSources();
                cboMicrophones.SelectedItem = axRControl.GetAudioSource(0);

                long num = axRControl.GetNumberOfResolutions(0);
                axRControl.VideoWidth = int.Parse(settings["PreviewVideoWidth"]);
                axRControl.VideoHeight = int.Parse(settings["PreviewVideoHeight"]);
                winFrmHost.Width = axRControl.VideoWidth;
                winFrmHost.Height = axRControl.VideoHeight;


                // initialize performance counter
                cpuReadingTimer = new Timer();
                cpuReadingTimer.Interval = 1000;
                cpuReadingTimer.Elapsed += cpuReadingTimer_Elapsed;
                cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                cpuReadingTimer.Start();

                //set local video folder
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(path, OGV2P.Admin.Properties.Settings.Default.LocalVideoFolder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Guid guid = Guid.NewGuid();
                path = Path.Combine(path, string.Format("{0}.mp4", guid.ToString()));
                txtLocalFile.Text = path;
                axRControl.DestinationURL2 = path;

                //set the publishing point url
                axRControl.DestinationURL = @"rtmp://devob2.opengovideo.com:1935/ogv2/myStream";

                //set the preview url
                txtUrl.Text = @"http://mytestserver.com/ogv2playerlive.html";

                //change status
                txtStatus.Text = "Idle";

                //setup the vu meter

                vuMeter.Minimum = double.Parse(settings["VuMeterMinimum"]);
                vuMeter.Maximum = double.Parse(settings["VuMeterMaximum"]);
                _vuMeterTimer = new Timer();
                _vuMeterTimer.Interval = double.Parse(settings["VuMeterInterval"]);
                _vuMeterTimer.Elapsed += _vuMeterTimer_Elapsed;
                _vuMeterTimer.Start();

                //start the preview
                axRControl.StartPreview();




            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        private void AddAudioSources()
        {
            int n = axRControl.NumberOfAudioSources;
            for (int i = 0; i < n; i++)
                Microphones.Add(axRControl.GetAudioSource(i));
        }

        private void AddVideoSources()
        {
            int n = axRControl.NumberOfVideoSources;
            for (int i = 0; i < n; i++)
                Cameras.Add(axRControl.GetVideoSource(i));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                base.OnInitialized(e);
                HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            if( msg == OGV2P.Admin.Helpers.UsbNotification.WmDevicechange && !IsBusy)
            {
                Dispatcher.Invoke(() =>
                {
                    int selectedCameraIndex = cboCameras.SelectedIndex;
                    Cameras.Clear();
                    OnPropertyChanged("Cameras");
                    cboCameras.SelectedItem = null;
                    System.Threading.Thread.Sleep(250);
                    AddVideoSources();
                    if (selectedCameraIndex <= Cameras.Count)
                        cboCameras.SelectedIndex = selectedCameraIndex;
                    else
                        cboCameras.SelectedIndex = 0;

                    int selectedMicIndex = cboMicrophones.SelectedIndex;
                    Microphones.Clear();
                    OnPropertyChanged("Microphones");
                    cboMicrophones.SelectedItem = null;
                    System.Threading.Thread.Sleep(250);
                    AddAudioSources();
                    if (selectedMicIndex <= Microphones.Count)
                        cboMicrophones.SelectedIndex = selectedMicIndex;
                    else
                        cboMicrophones.SelectedIndex = 0;
                });
                

            }
            return IntPtr.Zero;
        }

        private void _vuMeterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(IsBusy)
            {
                Dispatcher.Invoke(() =>
                {
                    int volumeLevel = axRControl.GetAudioLevel(0);
                    UpdateVUMeter(volumeLevel);
                    txtAudioLevel.Text = volumeLevel.ToString();
                });
            }
            
        }

        private void cpuReadingTimer_Elapsed(object p1, object p2)
        {
            // get the CPU reading
            Dispatcher.Invoke(() =>
            {
                float cpuUtilization = cpuCounter.NextValue();
                txtCpuUsage.Text = cpuUtilization + "%";
            });
          
        }

        void axRControl_OnStop(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnStopEvent e)
        {
            txtStatus.Text = "Stopped.";
            MessageBox.Show(e.ToString() + " - " + e.result);
        }

        void axRControl_OnEvent(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnEventEvent e)
        {
            if (e.type == "10")
                txtStatus.Text = "Running...";
            else
                txtStatus.Text = "Event Status " + e.type + " Text: " + e.result;

        }

        private void cmdStartRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //axRControl.DestinationURL = cmbUrl.Text;
                //axRTMPActiveX1.DestinationURL2 = cmbUrl2.Text;

                //axRTMPActiveX1.SetConfig("UseColorConverter", "2");
                //axRTMPActiveX1.StartConnect();
                axRControl.StartBroadcast();
                txtStatus.Text = "Running.";
                IsBusy = true;
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Stopped.";
                MessageBox.Show(ex.Message + "-" + axRControl.LastErrorMessage);
            }
        }

        private void cmdStopRecording_Click(object sender, RoutedEventArgs e)
        {
            axRControl.StopBroadcast();
            txtStatus.Text = "Stopped.";
            IsBusy = false;
            axRControl.StartPreview();

        }

        private void cmdPreviewVideo_CLick(object sender, RoutedEventArgs e)
        {
            Process.Start(txtUrl.Text);
        }

        private void PreviewLocalFileLocation(object sender, RoutedEventArgs e)
        {
            DirectoryInfo root = Directory.GetParent(txtLocalFile.Text);

            Process.Start(root.FullName);
        }

        private void cmdStamp_Click(object sender, RoutedEventArgs e)
        {
            String s = axRControl.GetConfig("StreamTime");
            int milliSeconds = int.Parse(s);
            TimeSpan current = new TimeSpan(0, 0, 0, 0, milliSeconds);
            txtLastStamp.Text = string.Format("{0}:{1}:{2}", current.Hours, current.Minutes, current.Seconds);
            _sessionService.Stamp(current);
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void cboCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsBusy)
            {
                MessageBox.Show(RECORDING_IN_PROGRESS);
                return;
            }

            axRControl.VideoSource = Convert.ToInt32(cboCameras.SelectedIndex);
            axRControl.StartPreview();


        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsBusy)
            {
                MessageBox.Show(RECORDING_IN_PROGRESS);
                return;
            }

            axRControl.AudioSource = Convert.ToInt32(cboMicrophones.SelectedIndex);
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }



        #endregion

       
    }
}