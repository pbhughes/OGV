using System;
using System.Collections.Generic;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;
using OGV.Cache;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Expression.Encoder.Devices;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using OGV.Infrastructure.Interfaces;
using System.IO;
using OGV.Infrastructure.Extensions;

namespace OGV.Streaming.Models
{

    public delegate void LoadCompleteDelegate(object sender, EventArgs e);

    public class LiveEncodingSource : EncodingSourceBase, IEncoderInterface, INotifyPropertyChanged
    {
        #region Fields - Properties - Events

        public IList<eeDevices.EncoderDevice> VideoDevices { get; set; }

        public eeDevices.EncoderDevice VideoDevice { get; set; }

        public IList<eeDevices.EncoderDevice> AudioDevices { get; set; }

        public eeDevices.EncoderDevice AudioDevice { get; set; }

        public bool UseLocalArchive { get; set; }

        public string ArchiveFileName { get; set; }

        private string settingsCacheFileName = "live_settings.xml";

        public event LoadCompleteDelegate LoadCompletedEvent;

        public DelegateCommand RecordCommand { get; set; }

        public DelegateCommand StopCommand { get; set; }

        protected System.Timers.Timer _timerFrameTrack = null;

        private long _numberOfSamples = 0;
        public long NumberOfSamples
        {
            set
            {
                _numberOfSamples = value;
                OnPropertyChanged("NumberOfSamples");
            }
            get
            {
                return _numberOfSamples;
            }
        }

        private long _numberOfDroppedSamples = 0;
        public long NumberOfDroppedSamples
        {
            set
            {
                _numberOfDroppedSamples = value;
                OnPropertyChanged("NumberOfDroppedSamples");
            }
            get
            {
                return _numberOfDroppedSamples;
            }
        }

        private string _sessionTime = "00 : 00 : 00";
        public string SessionTime
        {
            get
            {
                return _sessionTime;
            }
            set
            {
                _sessionTime = value;
                OnPropertyChanged("SessionTime");
            }
        }

        protected DateTime _startTime;

        protected TimeSpan _totalRecordTime = new TimeSpan(0, 0, 0);


        IUserViewModel _user;
        #endregion

        #region Constructors

        public LiveEncodingSource(IUserViewModel user)
            : base()
        {
            _user = user;
            _user.BoardList.AgendaSelectedEvent += BoardList_AgendaSelectedEvent;
            VideoDevices = eeDevices.EncoderDevices.FindDevices(eeDevices.EncoderDeviceType.Video);
            AudioDevices = eeDevices.EncoderDevices.FindDevices(eeDevices.EncoderDeviceType.Audio);

            SetDefaultDevices();

            RecordCommand = new DelegateCommand(OnRecord, CanRecord);
            StopCommand = new DelegateCommand(OnStop, CanStop);


            ActivateSource(VideoDevice, AudioDevice);
           
           
        }

        void BoardList_AgendaSelectedEvent(IAgenda selected)
        {
            RecordCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        private bool CanStop()
        {
            return _job.IsCapturing;
        }

        private void OnStop()
        {
            StopEncoding();
            _user.BoardList.SelectedAgenda.IsRecording = false;
            _user.BoardList.LoadAgendaCommand.RaiseCanExecuteChanged();
            _user.BoardList.LogOutCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            RecordCommand.RaiseCanExecuteChanged();
        }

        private bool CanRecord()
        {
            return (!_job.IsCapturing && _user.BoardList.SelectedAgenda != null );
        }

        private void OnRecord()
        {
            try
            {
                if (_job == null)
                {

                    _job = new LiveJob();
                    _job.Status += new EventHandler<EncodeStatusEventArgs>(job_Status);
                    

                }
                // remove all existing output formats it will have
                //to add them new each time
                _job.PublishFormats.Clear();

                //set the start time
                _startTime = DateTime.Now;

                //create the base file structure if it does not exist
                string myVideos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                myVideos = System.IO.Path.Combine(myVideos, "OV Videos");
                if (!System.IO.Directory.Exists(myVideos))
                    System.IO.Directory.CreateDirectory(myVideos);

                //add a folder for today's segments
                string todaysFolder = string.Format("{0}_{1}_{2}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year);
                myVideos = System.IO.Path.Combine(myVideos, todaysFolder);
                if (!System.IO.Directory.Exists(myVideos))
                    System.IO.Directory.CreateDirectory(myVideos);

                //check the video file name
                string ext = "ismv";
                if (_user != null)
                    if (_user.BoardList != null)
                        if (_user.BoardList.SelectedAgenda != null)
                            if (!string.IsNullOrEmpty(_user.BoardList.SelectedAgenda.VideoFilePath))
                            {
                                if (_user.BoardList.SelectedAgenda.VideoFilePath.Contains(".wmv"))
                                {
                                    _user.BoardList.SelectedAgenda.VideoFilePath.Replace(".wmv", "");

                                }
                                SessionName = _user.BoardList.SelectedAgenda.VideoFilePath;
 
                            }
                            else
                            {
                                //the file name is not set
                                SessionName = DateTime.Now.ToShortDateString().Replace("/","_") + "video" + "."+ ext;
                                _user.BoardList.SelectedAgenda.VideoFilePath = SessionName;
                            }
                //add the file archive output format by choosing
                //a segment file name that has not been used today

                string fileName = SessionName;
                int i = 1;
                string fullPath = System.IO.Path.Combine(myVideos, fileName);
                FileInfo fo = new FileInfo(fullPath);
                while (System.IO.File.Exists(fullPath))
                {
                    fileName = string.Format("{0}_{1}{2}",fo.FileNameNoExt(),i++,fo.Extension);
                    fullPath = System.IO.Path.Combine(myVideos, fileName);
                    _user.BoardList.SelectedAgenda.CurrentSegment = fileName;
                    _user.BoardList.SelectedAgenda.VideoFileName = fileName;
                    _user.BoardList.SelectedAgenda.VideoFilePath = fullPath;

                }

                FileArchivePublishFormat archiveFormat = new FileArchivePublishFormat(fullPath);
                
                _job.PublishFormats.Add(archiveFormat);

                if (_liveSource == null)
                {
                    ActivateSource(VideoDevice, AudioDevice);

                }

                //add streaming format
                PushBroadcastPublishFormat pushFormat = new PushBroadcastPublishFormat();
                Preset preset = Preset.SystemLivePresets.Where(p => p.Name.Contains("Low Bandwidth")).First();
                _job.ApplyPreset(preset);
               
                pushFormat.PublishingPoint = new Uri( @"http://ogv2.opengovideo.com/point1.isml");
                _job.PublishFormats.Add(pushFormat);
                _job.StartEncoding();
                _timerFrameTrack.Start();
                StopCommand.RaiseCanExecuteChanged();
                RecordCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region IEncoderInterface

        public override void PreconnectPublishingPoint()
        {
            try
            {
                //reset the publishing point and track each step
                if (StreamLive)
                {

                }
              

                if (_job == null)
                {

                    _job = new LiveJob();
                    _job.Status += new EventHandler<EncodeStatusEventArgs>(job_Status);
                    // remove all existing output formats it will have
                    //to add them new each time
                    _job.PublishFormats.Clear();

                }
                

                if (StreamLive)
                {
                    PushBroadcastPublishFormat pushformat = new PushBroadcastPublishFormat();
                    if( ! string.IsNullOrEmpty(PublishingPoint))
                    {
                        pushformat.PublishingPoint = new Uri(PublishingPoint);
                        pushformat.EventId = GenerateEventID();
                        Job.PublishFormats.Add(pushformat);
                        Job.PreConnectPublishingPoint();

                        int x = 0;
                        if (State != "Connected")
                        {
                            State = "Disconnected";
                            throw new Exception(Message);
                        }
                    }
                   
                }



               

                if (LoadCompletedEvent != null)
                    LoadCompletedEvent(this, new EventArgs());

            }
            catch (Exception ex)
            {

                Exception exf = new Exception(ex.Message, ex);
                throw exf;
            }


        }

        public LiveSource AddRootSource()
        {
            _liveSource = Job.AddDeviceSource(VideoDevice, AudioDevice);
            return _liveSource;
        }

        public void RemoveRootSource()
        {
            if (Job == null)
                return;

            if (Job.DeviceSources.Count > 0)
                Job.RemoveDeviceSource(Job.DeviceSources[0]);
        }

        public void StopEncoding()
        {
            _timerFrameTrack.Stop();
            base.Message = "Shutting down the live stream...";
            _timerFrameTrack.Stop();
            base.Job.StopEncoding();
            base.State = "Disconnected";

        }

        public PreviewWindow SetInputPreviewWindow(System.Drawing.Size windowSize, 
            System.Windows.Forms.Panel pnlInputPreview)
        {

            HandleRef h = new HandleRef(pnlInputPreview, pnlInputPreview.Handle);
            PreviewWindow prev = new PreviewWindow(h);
            prev.SetSize(windowSize);
            return prev;
        }

        /// <summary>
        /// Saves to disk, the users chosen runtime settings and use them on the next run
        /// sort of like default settings.
        /// </summary>
        public void CacheSettings()
        {
            List<string> names = new List<string>();
            List<string> values = new List<string>();

            string audioDevicePath = Job.DeviceSources[0].AudioDevice.DevicePath;
            string videoDevicePath = Job.DeviceSources[0].VideoDevice.DevicePath;
            string presetName = SelectedPreset.Name;

            names.Add("AudioDevice");
            values.Add(audioDevicePath);

            names.Add("VideoDevice");
            values.Add(videoDevicePath);

            names.Add("Preset");
            values.Add(presetName);

            CacheManager cm = new CacheManager();
            cm.WriteCacheSettings(names, values, settingsCacheFileName);

        }


        /// <summary>
        /// Reads and apply the users last run time settings.
        /// </summary>
        public void ReadAndApplySettings()
        {
            try
            {

                //read cache settings
                CacheManager cm = new CacheManager();
                XDocument doc = new XDocument();
                string settingsContent = cm.ReadCacheFile(settingsCacheFileName);
                if (settingsContent == string.Empty)
                    return;

                doc = XDocument.Parse(settingsContent);

                string audioDeviceName = doc.Element("Settings").Element(@"AudioDevice").Value;
                string videoDeviceName = doc.Element("Settings").Element(@"VideoDevice").Value;
                string presetName = doc.Element("Settings").Element(@"Preset").Value;

                //set the video device
                foreach (eeDevices.EncoderDevice x in VideoDevices)
                {
                    if (x.DevicePath == videoDeviceName)
                        VideoDevice = x;
                }

               
                

                //set the video devices
                foreach (eeDevices.EncoderDevice y in AudioDevices)
                {
                    if (y.DevicePath == videoDeviceName)
                    {
                        AudioDevice = y;
                    }
                }

                //apply the preset
                foreach (Preset p in Presets)
                {
                    if (p.Name == presetName)
                        SelectedPreset = p;
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error reading cached settings.", ex);
            }
        }

        #endregion

        #region Event Handlers

        void timerFrameTrack_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //report the sample statistics
            NumberOfDroppedSamples = _job.NumberOfDroppedSamples;
            NumberOfSamples = _job.NumberOfEncodedSamples;
            _totalRecordTime = _totalRecordTime.Add(new TimeSpan(0, 0, 0, 1, 0));


            //set the video time for stamps
            if(_user != null)
                if(_user.BoardList != null)
                    if (_user.BoardList.SelectedAgenda != null)
                    {
                        _user.BoardList.SelectedAgenda.VideoTime = _totalRecordTime;
                    }
            //report the time
            SessionTime = string.Format("{0} : {1} : {2}", _totalRecordTime.Hours, _totalRecordTime.Minutes, _totalRecordTime.Seconds);
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString());
            
        }

        #endregion

        #region Methods and Functions

        /// <summary>
        /// Sets the active devices to the first devices in the list
        /// </summary>
        private void SetDefaultDevices()
        {
            if (VideoDevices.Count > 0)
            {
                //set to the first device that can capture live video
                VideoDevice = VideoDevices.Where(dev => dev.Name =="Integrated Webcam").First();
            }
                

            if (AudioDevices.Count > 0)
                AudioDevice = AudioDevices[0];
        }

        public void ActivateSource( EncoderDevice video, EncoderDevice audio )
        {
            if (_job == null)
                _job = new LiveJob();

            foreach (LiveDeviceSource ds in _job.DeviceSources)
            {
                _job.RemoveDeviceSource(ds);
            }
            
            VideoDevice = video;
            AudioDevice = audio;
            _liveSource = _job.AddDeviceSource(VideoDevice, AudioDevice);

            
            _job.ActivateSource(_liveSource);

            if (_timerFrameTrack == null)
            {
                _timerFrameTrack = new System.Timers.Timer(1000);
                _timerFrameTrack.Elapsed += timerFrameTrack_Elapsed;
            }



        }

        void _job_Status(object sender, EncodeStatusEventArgs e)
        {
            
        }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));

           
        }

        #endregion INotifyPropertyChanged

        
    }
}
