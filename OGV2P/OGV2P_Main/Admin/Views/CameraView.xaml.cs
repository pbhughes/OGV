using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using System.Timers;
using System;
using System.Windows.Media;
using Microsoft.Practices.Prism.Regions;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.Management;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using System.Xml.Linq;
using forms = System.Windows.Forms;
using Infrastructure.Extensions;


namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl,  INotifyPropertyChanged
    {
        private const int FILE_SOURCE = 101;
        private const string RECORDING_IN_PROGRESS = "Recording in progress, please stop recording before changing devices";
        private Timer _vuMeterTimer;
        private ManagementEventWatcher usbWatcher = new ManagementEventWatcher();
        NameValueCollection _settings;
        private IRegionManager _regionManager;
        private IUser _user;
        private  AxRTMPActiveX.AxRTMPActiveX axRControl;
        private string PREFERED_DEVICE_FILE = "preferedDevices.xml";

        LinearGradientBrush _yellow =
        new LinearGradientBrush(Colors.Green, Colors.Yellow,
            new Point(0, 1), new Point(1, 0));

        public DelegateCommand NotificationCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

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

      

        public bool KeepAlive
        {
            get
            {
                return false;
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

        private TimeSpan _timerStamp;
        public TimeSpan TimerStamp
        {
            get
            {
                return _timerStamp;
            }
            set
            {
                _timerStamp = value;
                OnPropertyChanged("TimerStamp");
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

        public string InteractionResultMessage { get; private set; }

        private void UpdateVUMeter(int sampleVolume)
        {
            VuMeterReading = sampleVolume;
        }

        public CameraView(IRegionManager regionManager, IDevices devices, ISession sessionService, IMeeting meeting, IUser user)
        {
            

            try
            {
                InitializeComponent();


                //get the application settings
#pragma warning disable CS0618 // Type or member is obsolete
                _settings = ConfigurationSettings.AppSettings;
#pragma warning restore CS0618 // Type or member is obsolete

                axRControl = new AxRTMPActiveX.AxRTMPActiveX();


                axRControl.Width = int.Parse(_settings["PreviewVideoWidth"]);
                axRControl.Height = int.Parse(_settings["PreviewVideoHeight"]);
                
                

                this.DataContext = this;
                NotificationRequest = new InteractionRequest<INotification>();
                NotificationCommand = new DelegateCommand(OnNofity, CanNotify);

                _sessionService = sessionService;
                _meeting = meeting;
                _meeting.RaiseMeetingSetEvent += Meeting_SetEvent;
                _regionManager = regionManager;
                _user = user;
             

                _meeting.IsBusy = false;

                //initialize the window to listen for USB devices to be added
                var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");
                usbWatcher.EventArrived += UsbWatcher_EventArrived;
                usbWatcher.Query = query;
                usbWatcher.Start();

               

                //setup the vu meter
                vuMeter.Minimum = double.Parse(_settings["VuMeterMinimum"]);
                vuMeter.Maximum = double.Parse(_settings["VuMeterMaximum"]);
                _vuMeterTimer = new Timer();
                _vuMeterTimer.Interval = double.Parse(_settings["VuMeterInterval"]);
                _vuMeterTimer.Elapsed += _vuMeterTimer_Elapsed;
                _vuMeterTimer.Start();

                axRControl.Dock = System.Windows.Forms.DockStyle.Fill;
                winFormHost.Child.Controls.Add(axRControl);



            }
            catch (Exception ex)
            {
                if (ex.GetBaseException() != null)
                    ex.GetBaseException().WriteToLogFile();


                ex.WriteToLogFile();
                throw;
            }


        }

        private bool CanNotify()
        {
            return true;
        }

        private void OnNofity()
        {
            this.NotificationRequest.Raise(
                 new Notification { Content = "Notification Message", Title = "Hey Your Notified"},
                 n => { InteractionResultMessage = "The user was notified"; });
        }

        private void Meeting_SetEvent(object sender, EventArgs e)
        {
            try
            {
                axRControl.DestinationURL = _meeting.PublishingPoint;

                //axRControl.StartConnect();
                axRControl.StartPreview();
            }
            catch(AccessViolationException )
            {
                ;//ignore
            }
            catch (Exception )
            {

                throw;
            }
           
        }

        private void InitRTMPControl()
        {

            axRControl.InitEncoder();
            axRControl.SetConfig("UseSampleGrabber", "2");
            axRControl.AudioBitrate = 64000;
           

            //set the user id / password
            axRControl.SetConfig("Auth", string.Format("{0}:{1}",_user.SelectedBoard.UserID, _user.SelectedBoard.Password));

            // Device/Camera Resolution
            axRControl.VideoWidth = 800;
            axRControl.VideoHeight = 600;
            axRControl.VideoFrameRate = 30;

            // Video Encoder Bitrate (Bits/s)
            axRControl.VideoBitrate = 500000;


            axRControl.VideoEffect = 3;

            // nanoStream Event Handlers
            axRControl.OnEvent += new AxRTMPActiveX.IRTMPActiveXEvents_OnEventEventHandler(axRControl_OnEvent);
            axRControl.OnStop += new AxRTMPActiveX.IRTMPActiveXEvents_OnStopEventHandler(axRControl_OnStop);
            

            // Video/Audio Devices
            string[] lastUsedDevices = ReadDefaultDeviceCache();

           

            AddVideoSources();
            if(lastUsedDevices != null)
            {
                cboCameras.BeginInit();
                cboCameras.SelectedItem = lastUsedDevices[0];
                axRControl.VideoSource = FindVideoSource(lastUsedDevices[0]);
                cboCameras.EndInit();
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

            //winFormHost.Width = axRControl.VideoWidth;
            //winFormHost.Height = axRControl.VideoHeight;
            


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
                if (! _meeting.IsBusy)
                {
                    
                    //InitRTMPControl();

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
                ex.WriteToLogFile();
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

            cboCameras.Items.Add("Choose a File Source....");

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
                ex.WriteToLogFile();
                MessageBox.Show("Unable to find the last audio device used {0}", deviceName);
            }

            return result;
        }

        private void _vuMeterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_meeting.IsBusy)
            {
                Dispatcher.Invoke(() =>
                {
                    int volumeLevel = 0;
                    if (axRControl != null)
                        volumeLevel = axRControl.GetAudioLevel(0);

                    string s = axRControl.GetConfig("StreamTime");
                    int milliSeconds = int.Parse(s);
                    TimeSpan current = new TimeSpan(0, 0, 0, 0, milliSeconds);
                 
                    _sessionService.CurrentVideoTime = current;
                    TimerStamp = current;
                    UpdateVUMeter(volumeLevel);
                });
            }

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

            Message = "Streaming stopped for an unknown reason";

            System.Diagnostics.Debug.WriteLine(e.result);
        }


        void axRControl_OnEvent(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnEventEvent e)
        {

            int result = 0;
            bool parsed = int.TryParse(e.type, out result);

            if (parsed)
            {
                if (result == 10)
#pragma warning disable CS0642 // Possible mistaken empty statement
                    ;
#pragma warning restore CS0642 // Possible mistaken empty statement
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
                //set the meeting title in the view
                axRControl.TextOverlayText = (_meeting.MeetingName == null)? "" : _meeting.MeetingName;

                //font cache a file source only hardware
                if (axRControl.VideoSource != FILE_SOURCE)
                    WriteDefaultDeviceCache();

                //set local video folder
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(path, OGV2P.Admin.Properties.Settings.Default.LocalVideoFolder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

               if(string.IsNullOrEmpty(_meeting.MeetingName))
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                        $"No agenda file has been chosen this will record locally only and the recording can be found at {_meeting.DefaultVideoDirectory()}",
                        "Do you want to include an agenda so you can stream live?", 
                        MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                        return;

                }

                if(string.IsNullOrEmpty(_meeting.LocalFile))
                {
                    _meeting.LocalFile = string.Format("{0}_{1}_{2}.mp4", _user.SelectedBoard.Name, DateTime.Now.ToFileNameComponent(), _meeting.MeetingName);
                }

                path = Path.Combine(path, _meeting.LocalFile);
                axRControl.DestinationURL2 = path;
                _meeting.LocalFile = path;
                _meeting.IsBusy = true;
                axRControl.StartBroadcast();
                _vuMeterTimer.Start();
                _meeting.IsBusy = true;
                _meeting.LeftStatus = string.Format("Streaming to: {0}", _meeting?.PublishingPoint ?? "No stream selected");
                _meeting.RightStatus = _meeting.LandingPage;
                cmdStartRecording.IsEnabled = false;
                cmdStopRecording.IsEnabled = true;
            }
            catch (Exception ex)
            {
                cmdStartRecording.IsEnabled = true;
                cmdStopRecording.IsEnabled = false;
                ex.WriteToLogFile();
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
                    "Please verify the selected camera and microphone before recording.  {0}", ex.Message));
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
                    "Please verify the selected camera and microphone before recording.  {0}", ex.Message));
            }
            
        }

        private void cmdStopRecording_Click(object sender, RoutedEventArgs e)
        {

           
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                       "If you are streaming live the stream will stop and the counter will be reset",
                       "Do you want to stop streaming?",
                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                _vuMeterTimer.Stop();
                cmdStartRecording.IsEnabled = true;
                cmdStopRecording.IsEnabled = false;
                axRControl.StopBroadcast();
                Meeting.IsBusy = false;
                VuMeterReading = 0;
               
                TimerStamp = TimeSpan.Zero;
                Meeting.LeftStatus = "Idle";
                Meeting.RightStatus = "";
                axRControl.StartPreview();
            }
            catch (Exception ex)
            {
                cmdStartRecording.IsEnabled = false;
                cmdStopRecording.IsEnabled = true;

                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error stopping the preview", MessageBoxButton.OK);
            }
           
        }

        private void cboSource_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_meeting.IsBusy)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(RECORDING_IN_PROGRESS);
                    return;
                }

                if (cboCameras.SelectedItem.ToString().Contains("Choose a File Source...."))
                {
                    System.Windows.Forms.OpenFileDialog dg = new System.Windows.Forms.OpenFileDialog();
                    dg.DefaultExt = ".mp4";
                    dg.Filter = "Video Files (*.mp4)|*.mp4|WMV Files (*.wmv)|*.wmv|MOV Files (*.mov)|*.mov|MPG Files (*.mpg)|*.mpg|All (*.*)|*.*";
                    forms.DialogResult result = dg.ShowDialog();
                    cmdRecordLabel.Content = "Play";
                    if (result == forms.DialogResult.OK)
                    {
                        axRControl.VideoSource = FILE_SOURCE;
                        axRControl.DestinationURL2 = string.Empty;
                        if (File.Exists(dg.FileName))
                        {
                            axRControl.FileSourceFilename = dg.FileName;
                            axRControl.TextOverlayText = "";
                            axRControl.StartPreview();

                        }
                        else
                        {
                            Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("Unable to load file {0}", dg.FileName));
                        }

                    }
                }
                else
                {
                    cmdRecordLabel.Content = "REC";
                    axRControl.DestinationURL2 = _meeting.LocalFile;
                    axRControl.VideoSource = Convert.ToInt32(cboCameras.SelectedIndex);
                    axRControl.StartPreview();
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Level 1 occurred: {0} sub error {1}", ex.Message, axRControl.LastErrorMessage);
                Xceed.Wpf.Toolkit.MessageBox.Show(msg, "Preview Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           


        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_meeting.IsBusy)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(RECORDING_IN_PROGRESS);
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
            axRControl.License = @"nlic:1.2:LiveEnc:3.0:LvApp=1,LivePlg=1,H264DEC=1,H264ENC=1,MP4=1,RtmpMsg=1,RTMPx=3,Resz=1,RSrv=1,ScCap=1,NoMsg=1,Ap1=GOV2P.Main.exe,max=10,Ic=0:win:20151230,20161214::0:0:clerkbase-555215-1:ncpt:ce608864c444270ff79e5d65e5c92682";

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

       
        private void ToggleSwtich_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_meeting.IsBusy)
                    return;

                axRControl.StartPreview();
            }
            catch (Exception ex)
            {

                ex.WriteToLogFile();
            }
           
        }
    }
}