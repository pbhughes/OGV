using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using forms = System.Windows.Forms;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    /// 
    [RegionMemberLifetime(KeepAlive = true)]
    public partial class CameraView : UserControl, INotifyPropertyChanged,  IRegionMemberLifetime
    {
        private const int FILE_SOURCE = 101;
        private const string RECORDING_IN_PROGRESS = "Recording in progress, please stop recording before changing devices";
        private const string RES640X480 = "640x480";
        private const string RES320X240 = "320x240";
        private System.Timers.Timer _vuMeterTimer;
        private ManagementEventWatcher usbWatcher = new ManagementEventWatcher();
        private NameValueCollection _settings;
        private IRegionManager _regionManager;
        private IUser _user;
        private AxRTMPActiveX.AxRTMPActiveX axRControl;
        private string PREFERED_DEVICE_FILE = "preferedDevices.xml";
        private List<string> _resolutions = new List<string>();
        private LinearGradientBrush _yellow =
        new LinearGradientBrush(Colors.Green, Colors.Yellow,
            new Point(0, 1), new Point(1, 0));

        public DelegateCommand NotificationCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        private ISession _sessionService;

        private IMeeting _meeting;

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

        private bool _connecting;

        public bool Connecting
        {
            get
            {
                return _connecting;
            }

            set
            {
                _connecting = value;
                OnPropertyChanged("Connecting");
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

        private string _overlay = string.Empty;

        public string Overlay
        {
            get
            {
                return _overlay;
            }

            set
            {
                _overlay = value;
                OnPropertyChanged("Overlay");
            }
        }

        private bool _showOverlay;

        public bool ShowOverlay
        {
            get
            {
                return _showOverlay;
            }

            set
            {
                _showOverlay = value;
                OnPropertyChanged("ShowOverlay");
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

        private IUnityContainer _container;

        private void UpdateVUMeter(int sampleVolume)
        {
            VuMeterReading = sampleVolume;
        }

        public CameraView(IRegionManager regionManager, IDevices devices, IUser user, IUnityContainer container)
        {
            _container = container;

            try
            {
                InitializeComponent();

                
                //get the application settings
#pragma warning disable CS0618 // Type or member is obsolete
                _settings = ConfigurationSettings.AppSettings;
#pragma warning restore CS0618 // Type or member is obsolete

                axRControl = new AxRTMPActiveX.AxRTMPActiveX();

                //axRControl.Width = int.Parse(_settings["PreviewVideoWidth"]);
                //axRControl.Height = int.Parse(_settings["PreviewVideoHeight"]);

                this.DataContext = this;
                NotificationRequest = new InteractionRequest<INotification>();
                NotificationCommand = new DelegateCommand(OnNofity, CanNotify);


                _sessionService = _container.Resolve<ISession>();
                _meeting = _container.Resolve<IMeeting>();
                _meeting.RaiseMeetingSetEvent += Meeting_SetEvent;
                _regionManager = regionManager;
                _user = user;

                _meeting.IsBusy = false;
                cmdStopRecording.IsEnabled = false;

                //initialize the window to listen for USB devices to be added
                var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");
                usbWatcher.EventArrived += UsbWatcher_EventArrived;
                usbWatcher.Query = query;
                usbWatcher.Start();

                //setup the vu meter
                vuMeter.Minimum = double.Parse(_settings["VuMeterMinimum"]);
                vuMeter.Maximum = double.Parse(_settings["VuMeterMaximum"]);
                _vuMeterTimer = new System.Timers.Timer();
                _vuMeterTimer.Interval = double.Parse(_settings["VuMeterInterval"]);
                _vuMeterTimer.Elapsed += _vuMeterTimer_Elapsed;
                _vuMeterTimer.Start();

                axRControl.Dock = System.Windows.Forms.DockStyle.Fill;

                winFormHost.Child.Controls.Add(axRControl);

                this.Name = "CameraView";
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException() != null)
                    ex.GetBaseException().WriteToLogFile();

                ex.WriteToLogFile();
                throw;
            }
        }

        private void _sessionService_RaiseStopRecording(object sender, EventArgs e)
        {
            try
            {
                var er = new RoutedEventArgs();
                cmdStopRecording_Click(sender, er);
            }
            catch (Exception ex)
            {
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
                 new Notification { Content = "Notification Message", Title = "Hey Your Notified" },
                 n => { InteractionResultMessage = "The user was notified"; });
        }

        private async void Meeting_SetEvent(object sender, EventArgs e)
        {
            try
            {

                await Task.Run(() => {
                    axRControl.DestinationURL = _meeting.PublishingPoint;
                    
                    axRControl.StartPreview();
                });

               
            }
           
            catch (Exception ex)
            {
                _meeting.IsBusy = false;
                cmdStopRecording.IsEnabled = true;
                cmdStartRecording.IsEnabled = false;
                ex.WriteToLogFile();
                MessageBox.Show(string.Format("Unable to connect to {0} here is the error {1}", axRControl.DestinationURL, axRControl.LastErrorMessage));
            }
            finally
            {
                
            }
        }

        private void InitRTMPControl()
        {
            axRControl.InitEncoder();
            axRControl.SetConfig("UseSampleGrabber", "2");
            axRControl.AudioBitrate = 64000;

            //set the user id / password
            axRControl.SetConfig("Auth", string.Format("{0}:{1}", _user.SelectedBoard.UserID, _user.SelectedBoard.Password));

            axRControl.VideoEffect = 3;

            // Video/Audio Devices / resolution
            string audio = string.Empty, video = string.Empty, resolution = string.Empty;
            string[] lastUsedDevices = ReadDefaultDeviceCache(ref audio, ref video, ref resolution);

           
            // nanoStream Event Handlers
            axRControl.OnEvent += new AxRTMPActiveX.IRTMPActiveXEvents_OnEventEventHandler(axRControl_OnEvent);
            axRControl.OnStop += new AxRTMPActiveX.IRTMPActiveXEvents_OnStopEventHandler(axRControl_OnStop);

            AddVideoSources(null);
            if(video != string.Empty)
            {
                cboCameras.BeginInit();
                cboCameras.SelectedItem = video;
                axRControl.VideoSource = FindVideoSource(video);
                cboCameras.EndInit();
            }
            else
            {
                cboCameras.SelectedItem = axRControl.GetVideoSource(0);
                axRControl.VideoSource = 0;
            }

            AddAudioSources();
            if (audio != string.Empty)
            {
                cboMicrophones.SelectedItem = audio;
                axRControl.AudioSource = FindAudioSource(audio);
            }
            else
            {
                cboMicrophones.SelectedItem = axRControl.GetAudioSource(0);
                axRControl.AudioSource = 0;
            }

            SetupResolutions();

            //assign the previous resolution
            if (resolution != string.Empty)
                cboResolutions.SelectedItem = resolution;

            axRControl.DestinationURL = _meeting.ClientPathLiveStream;

            //reconnect settings
            axRControl.ReconnectAttempts = 3;
            axRControl.ReconnectDelay = 2000;
        }

        private void SetupResolutions()
        {
            // Device/Camera Resolution
            //clear existing items if they exist
            if (cboResolutions.Items.Count > 0)
                cboResolutions.Items.Clear();

            //acquire resolutions and frame rates
            int numberOfResolutions = axRControl.GetNumberOfResolutions();
            for (int i = 0; i < numberOfResolutions; i++)
            {
                string currentResolution = axRControl.GetResolution(i);
                cboResolutions.Items.Add(currentResolution);
            }


            //LOOK FOR KEY RESOLUTION 640X480 AND 320X340
            int indexOf640x480 = cboResolutions.Items.IndexOf(RES640X480);
            int indexOf320x240 = cboResolutions.Items.IndexOf(RES320X240);
           


            if (cboResolutions.Items.Count > 0)
            {
                //IF YOU HAVE 640X480 USE IT
                if (indexOf640x480 > -1)
                    cboResolutions.SelectedItem = RES640X480;
                else
                {
                    //YOU DIDNT HAVE 640X480 SO TRY TO USE 320X240
                    if (indexOf320x240 > -1)
                        cboResolutions.SelectedItem = "320x240";
                    else
                        //NEITHER KEY RESOLTUIONS EXIST US THE FIRT ONE
                        //IN THE LIST
                        cboResolutions.SelectedIndex = 0;
                }

            }
            else
                throw new Exception("Unable to determine device resolution, restart and try again. If a resolution cannot be obtained call tech support.");
        }

        private void UsbWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!_meeting.IsBusy)
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

        private void AddVideoSources(string previousSelection)
        {
            cboCameras.SelectionChanged -= cboSource_SelectedChanged;
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

            if (!string.IsNullOrEmpty(previousSelection))
            {
                cboCameras.SelectedItem = previousSelection;
            }
            cboCameras.SelectionChanged += cboSource_SelectedChanged;
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
                    System.Diagnostics.Debug.WriteLine(string.Format("Session Service Init Cameraview Time: {0}", _sessionService.InitializationTime.ToShortTimeString()));
                    TimerStamp = current;
                    UpdateVUMeter(volumeLevel);
                });
            }
        }

        private void axRControl_OnStop(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnStopEvent e)
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

        private void axRControl_OnEvent(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnEventEvent e)
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

        private async void cmdStartRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connecting = true;
                _meeting.IsLive = true;

                

                if (expDropDown.IsExpanded)
                {
                    expDropDown.IsExpanded = false;
                }

                if (cboFrameRates.SelectedItem == null)
                    axRControl.VideoFrameRate = 30;
                else
                {
                    double frameRate = double.Parse(cboFrameRates.SelectedItem.ToString());
                }

               

                StartRecording();

            }

            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                Connecting = false;
            }
           
        }

        private async void TryToConnect()
        {
            Task t = Task.Run(() =>
            {
               axRControl.StartConnect();
            });

            await t;
        }

        private void StartRecording()
        {
            try
            {

                // Video Encoder Bitrate (Bits/s)
                axRControl.VideoBitrate = GetVideoBitRate();
                axRControl.VideoHeight = GetVideoHeight();
                axRControl.VideoWidth = GetVideoWidth();

                Meeting = _container.Resolve<IMeeting>();

                //font cache a file source only hardware
                if (axRControl.VideoSource != FILE_SOURCE)
                    WriteDefaultDeviceCache();

                //set local video folder
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(path, OGV2P.Admin.Properties.Settings.Default.LocalVideoFolder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (string.IsNullOrEmpty(_meeting.MeetingName))
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                        $"Please choose an agenda file to continue.",
                        "No agenda file loaded",
                        MessageBoxButton.OK);
                    return;
                }

                if (string.IsNullOrEmpty(_meeting.LocalFile))
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
                _meeting.IsBusy = false;
                cmdStopRecording.IsEnabled = true;
                cmdStartRecording.IsEnabled = false;
                ex.WriteToLogFile();
                MessageBox.Show("Error trying to record be sure to choose a valid agenda file" + "-" + axRControl.LastErrorMessage);
            }
            finally
            {
                Connecting = false;
            }
        }

        private string[] ReadDefaultDeviceCache(ref string audio, ref string video, ref string resolution)
        {
            try
            {
                
                if (File.Exists(PREFERED_DEVICE_FILE))
                {
                    string xml = File.ReadAllText(PREFERED_DEVICE_FILE);
                    XDocument xDoc = XDocument.Parse(xml);
                    video = xDoc.Element("devices").Element("videodevice").Value;
                    audio = xDoc.Element("devices").Element("audiodevice").Value;
                    if (xDoc.Element("devices").Element("resolution") != null)
                        resolution = xDoc.Element("devices").Element("resolution").Value;
                    else
                        resolution = "640x480";

                    string[] result = new string[] { video, audio, resolution };

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

                string videoCacheDevice = (cboCameras.SelectedItem == null) ? cboCameras.Items[0].ToString() : cboCameras.SelectedItem.ToString();
                string audioCacheDevice = (cboMicrophones.SelectedItem == null) ? cboMicrophones.Items[0].ToString() : cboMicrophones.SelectedItem.ToString();
                string resolution = cboResolutions.SelectedItem.ToString();
                XDocument xDoc = new XDocument();
                XElement root = new XElement("devices", null);
                XElement videoElement = new XElement("videodevice", videoCacheDevice);
                XElement audioElement = new XElement("audiodevice", audioCacheDevice);
                XElement resolutionElement = new XElement("resolution", resolution);
                root.Add(videoElement);
                root.Add(audioElement);
                root.Add(resolutionElement);
                xDoc.Add(root);
                File.WriteAllText(PREFERED_DEVICE_FILE, xDoc.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Tried to write the default device cache file and failed. " +
                    "Please verify the selected camera and microphone before recording."));
            }
        }

        private void cmdStopRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_meeting.IsLive)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                        "If you are streaming live the stream will stop and the counter will be reset",
                        "Do you want to stop streaming?",
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        axRControl.StopBroadcast();
                        _vuMeterTimer.Stop();
                        cmdStartRecording.IsEnabled = true;
                        cmdStopRecording.IsEnabled = false;
                        _sessionService.SignalStopRecording(this, new EventArgs());

                        if (_meeting.IsLive)
                        {
                            Meeting.IsBusy = false;
                            VuMeterReading = 0;
                            TimerStamp = TimeSpan.Zero;
                            Meeting.RightStatus = "";
                        }
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        axRControl.StopBroadcast();
                        _vuMeterTimer.Stop();
                        cmdStartRecording.IsEnabled = true;
                        cmdStopRecording.IsEnabled = false;
                        Meeting.IsBusy = false;
                    });
                }

                Meeting.LeftStatus = "Idle";
                _meeting.IsLive = true;
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
                    axRControl.VideoEffect = 0;
                    System.Windows.Forms.OpenFileDialog dg = new System.Windows.Forms.OpenFileDialog();
                    dg.DefaultExt = ".mp4";
                    dg.Filter = "Video Files (*.mp4)|*.mp4|WMV Files (*.wmv)|*.wmv|MOV Files (*.mov)|*.mov|MPG Files (*.mpg)|*.mpg|All (*.*)|*.*";
                    forms.DialogResult result = dg.ShowDialog();

                    if (result == forms.DialogResult.OK)
                    {
                        axRControl.VideoSource = FILE_SOURCE;
                        cmdRecordLabel.Content = "Play";
                        cmdStopRecording.Content = "Pause";
                        if (File.Exists(dg.FileName))
                        {
                            _meeting.IsLive = true;
                            axRControl.FileSourceFilename = dg.FileName;
                            txtLocalFileSource.Text = dg.FileName;
                            FileInfo fi = new FileInfo(dg.FileName);
                            cboCameras.Items.Add(fi.Name);
                            cboCameras.SelectionChanged -= cboSource_SelectedChanged;
                            cboCameras.SelectedItem = fi.Name;
                            cboCameras.SelectionChanged += cboSource_SelectedChanged;
                        }
                        else
                        {
                            Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("Unable to load file {0}", dg.FileName));
                        }
                    }
                }
                else
                {
                    _meeting.IsLive = false;
                    cmdRecordLabel.Content = "REC";
                    cmdStopRecording.Content = "Stop";
                    axRControl.DestinationURL2 = _meeting.LocalFile;
                    axRControl.VideoSource = Convert.ToInt32(cboCameras.SelectedIndex);
                    AddVideoSources(cboCameras.SelectedItem.ToString());
                    cboCameras.SelectionChanged -= cboSource_SelectedChanged;

                    SetupResolutions();
                    cboCameras.SelectionChanged += cboSource_SelectedChanged;
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

        #endregion INotifyPropertyChanged

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

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (axRControl.InvokeRequired)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    DisplayOverLay();
                }));
            }
            else
            {
                DisplayOverLay();
            }
        }

        private void DisplayOverLay()
        {
            if (!_meeting.IsBusy)
            {
                if (!ShowOverlay)
                    axRControl.TextOverlayText = string.Empty;
                else
                {
                    //set the overlay position

                    axRControl.VideoEffect = 3;
                    axRControl.SetConfig("FontSize", "12");
                    axRControl.SetConfig("OverlayBackgroundColor", "0x00FFFFFF");
                    if (string.IsNullOrEmpty(Overlay))
                    {
                        axRControl.TextOverlayText = 
                            (Meeting.MeetingDate == DateTime.MinValue) ? 
                            DateTime.Now.ToShortDateString() : Meeting.MeetingDate.ToShortDateString();
                    }
                    else
                    {
                        axRControl.TextOverlayText = string.Format("{0}-{1}", Overlay,
                        (Meeting.MeetingDate == DateTime.MinValue) ? 
                            DateTime.Now.ToShortDateString() : Meeting.MeetingDate.ToShortDateString());
                    }
                }

                if (!_meeting.IsLive)
                    axRControl.StartPreview();
            }
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_meeting.IsBusy && ShowOverlay)
            {
                DisplayOverLay();
            }
        }

        private void cboResolutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cboColorSpaces.Items.Clear();

            int width, height;
            GetResolution(out width, out height);
            int numberOfColorSpaces = axRControl.GetNumberOfColorSpaces(width, height);
            for (int i = 0; i < numberOfColorSpaces; i++)
            {
                string colorSpace = axRControl.GetColorSpace(i);
                cboColorSpaces.Items.Add(colorSpace);
            }

            if (cboColorSpaces.Items.Count > 0)
                cboColorSpaces.SelectedIndex = 0;
        }

        private void cboColorSpaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboColorSpaces.SelectedItem == null)
                return;

            cboFrameRates.Items.Clear();

            string colorSpace = cboColorSpaces.SelectedItem.ToString();
            int width, height;
            GetResolution(out width, out height);
            int numberOfFrameRates = axRControl.GetNumberOfFramerates(width, height, colorSpace);
            for (int i = 0; i < numberOfFrameRates; i++)
            {
                double frameRate = axRControl.GetFramerate(i);
                cboFrameRates.Items.Add(frameRate);
            }

            if (cboFrameRates.Items.Count > 0)
                cboFrameRates.SelectedIndex = 0;
        }

        private string GetResolution(out int width, out int height)
        {
            width = 0; height = 0;
            string resolution = string.Empty;
            if (cboResolutions.SelectedItem != null)
            {
                resolution = cboResolutions.SelectedItem.ToString();
                string[] splits = resolution.Split('x');
                width = int.Parse(splits[0]);
                height = int.Parse(splits[1]);
            }

            

            return resolution;
        }

        private int GetVideoHeight()
        {
            int width, height;
            GetResolution(out width, out height);
            return height;
        }

        private int GetVideoWidth()
        {
            int width, height;
            GetResolution(out width, out height);
            return width;
        }

        private int GetVideoBitRate()
        {
            int width, height;
            string resolution = GetResolution(out width, out height);
            if (width < 720)
                return 500 * 1024;   //500 KB/s
            else
                return 1 * (1024 * 1024); //1MB/s
        }

    

 

        private void AdvancedExpanderClick(object sender, RoutedEventArgs e)
        {
            advancedHeader.IsExpanded = !advancedHeader.IsExpanded;
        }
    }
}