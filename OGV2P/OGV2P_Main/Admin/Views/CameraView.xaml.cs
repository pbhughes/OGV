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
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Prism.Commands;
using forms = System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;

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
        private IRegionManager _regionManager;
        private IUser _user;
        private AxRTMPActiveX.AxRTMPActiveX axRControl;
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        private string PREFERED_DEVICE_FILE = "preferedDevices.xml";

        LinearGradientBrush _yellow =
        new LinearGradientBrush(Colors.Green, Colors.Yellow,
            new Point(0, 1), new Point(1, 0));

        public DelegateCommand NotificationCommand { get; set; }

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

        private int _bandwidthCheckInterval;
        public int BandwidthCheckInterval
        {
            get
            {
                return _bandwidthCheckInterval;
            }

            set
            {
                _bandwidthCheckInterval = value;
                OnPropertyChanged("BandwithCheckInterval");
            }
        }

        private bool _isCheckingBandwidth;
        public bool IsCheckingBandwidth
        {
            get
            {
                return _isCheckingBandwidth;
            }

            set
            {
                _isCheckingBandwidth = value;
                OnPropertyChanged("IsCheckingBandwidth");
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

        private double _vuMeterReading;

        public double VuMeterReading
        {
            get
            {
                return _vuMeterReading;
            }

            set
            {
                _vuMeterReading = value;
                OnPropertyChanged("VuMeterReading");
            }
        }

        private int _hours;
        public int Hours
        {
            get
            {
                return _hours;
            }

            set
            {
                _hours = value;
                OnPropertyChanged("Hours");
                OnPropertyChanged("TimerStamp");
            }
        }

        private int _minutes;
        public int Minutes
        {
            get
            {
                return _minutes;
            }

            set
            {
                _minutes = value;
                OnPropertyChanged("Minutes");
            }
        }

        private int _seconds;
        public int Seconds
        {
            get
            {
                return _seconds;
            }

            set
            {
                _seconds = value;
                OnPropertyChanged("Seconds");
            }
        }

        public string TimerStamp
        {
            get
            {
                return string.Format("{0}:{1}:{2}", _hours, _minutes, _seconds);
            }

           
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private void UpdateVUMeter(int sampleVolume)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                VuMeterReading = (double)sampleVolume;
            });
        }

        public CameraView(IRegionManager regionManager, IDevices devices, ISession sessionService, IMeeting meeting, IUser user)
        {
            InitializeComponent();

            try
            {
                this.DataContext = this;
                NotificationRequest = new InteractionRequest<INotification>();
                NotificationCommand = new DelegateCommand(() =>
                {
                    NotificationRequest.Raise(new Notification { Title = "Settings", Content = "Notfication Set" }, null);
                });

                _sessionService = sessionService;
                _meeting = meeting;
                _meeting.RaiseMeetingSetEvent += Meeting_SetEvent;
                _regionManager = regionManager;
                _user = user;
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

        private void Meeting_SetEvent(object sender, EventArgs e)
        {
            try
            {
                axRControl.DestinationURL = _meeting.PublishingPoint;

                //axRControl.StartConnect();
                axRControl.StartPreview();
            }
            catch(AccessViolationException aex)
            {
                ;//ignore
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
            axRControl.SetConfig("Auth", string.Format("{0}:{1}",_user.SelectedBoard.UserID, _user.SelectedBoard.Password));

          

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
            string[] lastUsedDevices = ReadDefaultDeviceCache();

           

            AddVideoSources();
            if(lastUsedDevices != null)
            {
                cboCameras.SelectedItem = lastUsedDevices[0];
                axRControl.VideoSource = FindVideoSource(lastUsedDevices[0]); ;
            }
            else
            {
                cboCameras.SelectedItem = axRControl.GetVideoSource(0);
                axRControl.VideoSource = 0;
            }
            

            AddAudioSources();
            if (lastUsedDevices != null)
            {
                cboMicrophones.SelectedItem = lastUsedDevices[1];
                axRControl.AudioSource = FindAudioSource(lastUsedDevices[1]);

            }
            else
            {
                cboMicrophones.SelectedItem = axRControl.GetAudioSource(0);
                axRControl.AudioSource = 0;
            }
           

            long num = axRControl.GetNumberOfResolutions(0);
            axRControl.VideoWidth = int.Parse(_settings["PreviewVideoWidth"]);
            axRControl.VideoHeight = int.Parse(_settings["PreviewVideoHeight"]);
            winFrmHost.Width = axRControl.VideoWidth;
            winFrmHost.Height = axRControl.VideoHeight;

            


            //set the publishing point url
            //axRControl.DestinationURL = @"rtmp://devob2.opengovideo.com:1935/RI_SouthKingstown_Live/LicenseBoard";
            axRControl.DestinationURL = _meeting.ClientPathLiveStream;

            //reconnect settings
            axRControl.ReconnectAttempts = 3;
            axRControl.ReconnectDelay = 2000;




        }

        private void UsbWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!IsBusy)
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
                if (!cboMicrophones.Items.Contains(source))
                {
                    cboMicrophones.Items.Add(source);
                }

            }

        }

        private int FindAudioSource(string deviceName)
        {
            int result = 0;

            try
            {
                int n = axRControl.NumberOfAudioSources;
                for (int i = 0; i < n; i++)
                {
                    string source = axRControl.GetAudioSource(i);
                    if (source == deviceName)
                        return i;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Unable to find the last audio device used {0}", deviceName);
            }

            return result;
        }

        private void AddVideoSources()
        {

            int n = axRControl.NumberOfVideoSources;
            cboCameras.Items.Clear();
            for (int i = 0; i < n; i++)
            {
                string source = axRControl.GetVideoSource(i);
                if (!cboCameras.Items.Contains(source))
                {
                    cboCameras.Items.Add(source);
                }
            }

        }

        private int FindVideoSource(string deviceName)
        {
            int result = 0;

            try
            {
                int n = axRControl.NumberOfVideoSources;
                for (int i = 0; i < n; i++)
                {
                    string source = axRControl.GetVideoSource(i);
                    if (source == deviceName)
                        return i;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Unable to find the last audio device used {0}", deviceName);
            }

            return result;
        }

        private void _vuMeterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsBusy)
            {
                Dispatcher.Invoke(() =>
                {
                    int volumeLevel = 0;
                    if (axRControl != null)
                        volumeLevel = axRControl.GetAudioLevel(0);

                    string s = axRControl.GetConfig("StreamTime");
                    int milliSeconds = int.Parse(s);
                    TimeSpan current = new TimeSpan(0, 0, 0, 0, milliSeconds);
                    Hours = (int)current.TotalHours;
                    Minutes = (int)current.TotalMinutes;
                    Seconds = (int)current.TotalSeconds;
                    _sessionService.CurrentVideoTime = current;
                    UpdateVUMeter(volumeLevel);
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
            int result = 0;
            bool parsed = int.TryParse(e.result, out result);
            if (parsed)
            {
                if (result < 64)
                {

                    switch (result)
                    {
                        case 1:
                            Message = "Streaming Error";
                            break;
                        case 2:
                            Message = "Connection Lost";
                            break;
                        case 3:
                            Message = "No input data, timeout";
                            break;
                        case 4:
                            Message = "License expired";
                            break;
                        default:
                            Message = "Stopped";
                            break;
                            
                    }
                    return;
                }
                else
                {
                    Message = string.Format("Connection lost, reconnecting");
                    return;
                }
            }

            Message = "Streaming stopped for an unkown reason";

            System.Diagnostics.Debug.WriteLine(e.result);
        }

        void axRControl_OnEvent(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnEventEvent e)
        {

            int result = 0;
            bool parsed = int.TryParse(e.type, out result);

            if (parsed)
            {
                if (result == 10)
                    ;
                if (result == 11)
                {
                    RTMPStatus status = Newtonsoft.Json.JsonConvert.DeserializeObject<RTMPStatus>(e.result);
                    Message = status.ConnectionStatus;
                }

            }
            System.Diagnostics.Debug.WriteLine(e.result);

        }

        private void cmdStartRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WriteDefaultDeviceCache();

                //set local video folder
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(path, OGV2P.Admin.Properties.Settings.Default.LocalVideoFolder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, _meeting.LocalFile);
                axRControl.DestinationURL2 = path;
                _meeting.LocalFile = path;
                _meeting.IsBusy = true;
                axRControl.StartBroadcast();
                _vuMeterTimer.Start();
                IsBusy = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to record be sure to choose a valid agenda file" + "-" + axRControl.LastErrorMessage);
            }
        }

        private string[] ReadDefaultDeviceCache()
        {
            try
            {
                string video = string.Empty;
                string audio = string.Empty;
                if (File.Exists(PREFERED_DEVICE_FILE))
                {
                    string xml = File.ReadAllText(PREFERED_DEVICE_FILE);
                    XDocument xDoc = XDocument.Parse(xml);
                    video = xDoc.Element("devices").Element("videodevice").Value;
                    audio = xDoc.Element("devices").Element("audiodevice").Value;

                    string[] result = new string[] { video, audio };

                    return result;
                }

                
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("Tried to read the default device cache file and failed. " +
                    "Please verify the selected camera and micrphone before recording.  {0}", ex.Message));
            }

            return null;

        }

        private void WriteDefaultDeviceCache()
        {
            try
            {
                //remember the chosen audio and video devices
                string videoCacheDevice = cboCameras.SelectedItem.ToString();
                string audioCacheDevice = cboMicrophones.SelectedItem.ToString();
                XDocument xDoc = new XDocument();
                XElement root = new XElement("devices", null);
                XElement videoElement = new XElement("videodevice", videoCacheDevice);
                XElement audioElement = new XElement("audiodevice", audioCacheDevice);
                root.Add(videoElement);
                root.Add(audioElement);
                xDoc.Add(root);
                File.WriteAllText(PREFERED_DEVICE_FILE, xDoc.ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("Tried to write the default device cache file and failed. " +
                    "Please verify the selected camera and micrphone before recording.  {0}", ex.Message));
            }
            
        }

        private void cmdStopRecording_Click(object sender, RoutedEventArgs e)
        {
            axRControl.StopBroadcast();
            IsBusy = false;
            VuMeterReading = 0;
            _vuMeterTimer.Stop();
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            axRControl.StartPreview();
            txtTimer.Text = string.Empty;
        }

        private void cmdStamp_Click(object sender, RoutedEventArgs e)
        {

            _sessionService.Stamp();

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

        private void cmdStartBandwidthCheck_Click(object sender, RoutedEventArgs e)
        {
            IsCheckingBandwidth = true;
            Task checkBandwith = new Task(() =>
           {
               int keepgoing = 0;
               axRControl.StartBandwidthChecker();
               while (keepgoing <= 9)
               {
                   
                   System.Threading.Thread.Sleep(250);
                   keepgoing++;
                   BandwidthCheckInterval = keepgoing;
               }


               Dispatcher.Invoke(() =>
               {
                   int bandwidth = axRControl.StopBandwidthChecker();
                   IsCheckingBandwidth = false;
                   axRControl.StartPreview();
               });
           });
            checkBandwith.Start();

        }

     

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (winFrmHost.Visibility == Visibility.Visible)
                {
                   

                    winFrmHost.Visibility = Visibility.Hidden;
                    var settingsView = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("SettingsView");
                    _regionManager.Regions[Infrastructure.Models.Regions.Middle].Activate(settingsView);

                }
                else
                {
                    winFrmHost.Visibility = Visibility.Visible;
                    var settingsView = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("SettingsView");
                    _regionManager.Regions[Infrastructure.Models.Regions.Middle].Deactivate(settingsView);

             
                }
                    
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
           
        }
    }
}