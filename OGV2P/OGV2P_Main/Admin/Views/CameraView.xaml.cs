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
using System.Windows.Interop;
using System.Management.Instrumentation;
using System.Management;
using System.Collections.Specialized;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl, INavigationAware, IRegionMemberLifetime, INotifyPropertyChanged
    {
        private const string RECORDING_IN_PROGRESS = "Recording in progress, please stop recording before changing devices";
        private System.Timers.Timer cpuReadingTimer;
        private PerformanceCounter cpuCounter;
        private Timer _vuMeterTimer;
        private ManagementEventWatcher usbWatcher = new ManagementEventWatcher();
        NameValueCollection _settings;
        private AxRTMPActiveX.AxRTMPActiveX axRControl;
        LinearGradientBrush _yellow =
        new LinearGradientBrush(Colors.Green, Colors.Yellow,
            new Point(0, 1), new Point(1, 0));

        ISession _sessionService;
        public ISession SessionService
        {
            get
            {
                return _sessionService;
            }

            set
            {
                _sessionService = value;
                OnPropertyChanged("SessionService");
            }
        }
        IMeeting _meeting;
        public IMeeting Meeting
        {
            get
            {
                return _meeting;
            }

            set
            {
                _meeting = value;
                OnPropertyChanged("Meeting");
            }
        }

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

        

        private void UpdateVUMeter(int sampleVolume)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                vuMeter.Value = (double)sampleVolume;
            });
        }
    


        public CameraView(IDevices devices, ISession sessionService, IMeeting meeting)
        {
            InitializeComponent();

            try
            {
                this.DataContext = this;
                _sessionService = sessionService;
                _meeting = meeting;

     
                //get the application settings
                _settings = ConfigurationSettings.AppSettings;

                //initialize the window to listen for usb devices to be added
                var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");
                usbWatcher.EventArrived += UsbWatcher_EventArrived;
                usbWatcher.Query = query;
                usbWatcher.Start();

                // initialize performance counter
                cpuReadingTimer = new Timer();
                cpuReadingTimer.Interval = 1000;
                cpuReadingTimer.Elapsed += cpuReadingTimer_Elapsed;
                cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                cpuReadingTimer.Start();

                //set preview URL
                //set the preview url
                txtUrl.Text = @"http://mytestserver.com/ogv2playerlive.html";


                //change status
                txtStatus.Text = "Idle";

                //setup the vu meter
                vuMeter.Minimum = double.Parse(_settings["VuMeterMinimum"]);
                vuMeter.Maximum = double.Parse(_settings["VuMeterMaximum"]);
                _vuMeterTimer = new Timer();
                _vuMeterTimer.Interval = double.Parse(_settings["VuMeterInterval"]);
                _vuMeterTimer.Elapsed += _vuMeterTimer_Elapsed;
                _vuMeterTimer.Start();


                axRControl = new AxRTMPActiveX.AxRTMPActiveX();
                winFrmHost.Child = axRControl;


            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        private void InitRTMPControl()
        {
            
            axRControl.License = "nlic:1.2:LiveEnc:3.0:LvApp=1,LivePlg=1,MSDK=4,MPEG2DEC=1,MPEG2ENC=1,PS=1,TS=1,H264DEC=1,H264ENC=1,H264ENCQS=1,MP4=4,RTMPsrc=1,RtmpMsg=1,RTMPs=1,RTSP=1,RTSPsrc=1,UDP=1,UDPsrc=1,HLS=1,WMS=1,WMV=1,RTMPm=4,RTMPx=3,Resz=1,RSrv=1,VMix2=1,3DRemix=1,ScCap=1,AuCap=1,AEC=1,Demo=1,Ic=1,NoMsg=1,Tm=1800,T1=600,NoIc=1:win,win64,osx:20151030,20160111::0:0:nanocosmosdemo-292490-3:ncpt:f6044ea043c479af5911e60502f1a334";
            axRControl.InitEncoder();

            //set the user id / password
            axRControl.SetConfig("Auth", "barkley:hughes");

            // Video Source Device (0...n)
            axRControl.VideoSource = 0;

            // Device/Camera Resolution
            axRControl.VideoWidth = 640;
            axRControl.VideoHeight = 480;
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
            AddVideoSources();
            cboCameras.SelectedItem = axRControl.GetVideoSource(0);

            AddAudioSources();
            cboMicrophones.SelectedItem = axRControl.GetAudioSource(0);

            long num = axRControl.GetNumberOfResolutions(0);
            axRControl.VideoWidth = int.Parse(_settings["PreviewVideoWidth"]);
            axRControl.VideoHeight = int.Parse(_settings["PreviewVideoHeight"]);
            winFrmHost.Width = axRControl.VideoWidth;
            winFrmHost.Height = axRControl.VideoHeight;

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
            //axRControl.DestinationURL = @"rtmp://devob2.opengovideo.com:1935/RI_SouthKingstown_Live/LicenseBoard";
            axRControl.DestinationURL = _meeting.ClientPathLiveStream;

          
        }

        private void UsbWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                if( !IsBusy)
                {
                    System.Threading.Thread.Sleep(2000);
                    InitRTMPControl();

                }
            });
        }

        private void AddAudioSources()
        {
            int n = axRControl.NumberOfAudioSources;
            cboMicrophones.Items.Clear();
            for (int i = 0; i < n; i++)
            {
                string source = axRControl.GetAudioSource(i);
                if ( ! cboMicrophones.Items.Contains(source))
                {
                    cboMicrophones.Items.Add(source);
                }
                  
            }
                
        }

        private void AddVideoSources()
        {
           
            int n = axRControl.NumberOfVideoSources;
            cboCameras.Items.Clear();
            for (int i = 0; i < n; i++)
            {
                string source = axRControl.GetVideoSource(i);
                if (! cboCameras.Items.Contains(source))
                {
                    cboCameras.Items.Add(source);
                }
            }
               
        }

    

     
        private void _vuMeterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(IsBusy)
            {
                Dispatcher.Invoke(() =>
                {
                    int volumeLevel = 0;
                    if (axRControl != null)
                        volumeLevel = axRControl.GetAudioLevel(0);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //INIT AXRTMP Control
            InitRTMPControl();
        }

        private void cmdPreview_Click(object sender, RoutedEventArgs e)
        {
            //start the preview
            axRControl.StartPreview();
        }
    }
}