using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using OGV.Cache;

namespace OGV.Streaming.Models
{
    public abstract class EncodingSourceBase : INotifyPropertyChanged
    {
        #region Fields, Events, and Delegates
        CacheManager cm = new CacheManager();

        protected LiveDeviceSource _liveSource;
        public LiveDeviceSource FeedSource
        {
            get
            {
                return _liveSource;
            }
        }

        public string SourceTypeString { get; set; }

        public List<Preset> _presets = null;
        public List<Preset> Presets
        {
            get
            {
                return _presets;
            }
            set
            {
                _presets = value;
                OnPropertyChanged("Presets");
            }
        }

        protected Preset SelectedPreset { get; set; }

        protected LiveJob _job = null;
        public LiveJob Job
        {
            get { return _job; }
            set { _job = value; }
        }

        private bool preConnectComplete = false;

        private string _messageContainer = string.Empty;
        public string Message
        {
            get
            {
                return _messageContainer;
            }
            set
            {
                _messageContainer = value;
                OnPropertyChanged("Message");
            }
        }

        private string _state = string.Empty;
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        protected System.Timers.Timer timerFrameTrack = null;

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

        private bool _streamLive = false;
        public bool StreamLive
        {
            get
            {
                return _streamLive;
            }
            set
            {
                _streamLive = value;
                OnPropertyChanged("StreamLive");
            }
        }

        private string _sessionName = "Video";
        public string SessionName
        {
            get
            {
                return _sessionName;
            }
            set
            {
                _sessionName = value;
                OnPropertyChanged("SessionName");
            }
        }

        private string _publishingPoint;
        public string PublishingPoint
        {
            get { return _publishingPoint; }
            set { _publishingPoint = value; }
        }

        #endregion

        #region Constructors
        public EncodingSourceBase()
        {
            Presets = cm.ReadPresetsFromFile();
            SelectedPreset = Presets[0];
        }

        #endregion

        #region Methods and Functions

        public abstract void PreconnectPublishingPoint();

        protected string GenerateEventID()
        {
            string retval = string.Empty;

            retval = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", DateTime.Now.Year, DateTime.Now.Day, DateTime.Now.Month,
                                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            return retval;
        }

        protected void RaiseOnPropertyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        #endregion

        #region Event Handlers

        public void job_Status(object sender, EncodeStatusEventArgs e)
        {
            switch (e.Status)
            {
                case EncodeStatus.PublishingPointError:
                    State = Message = e.Message;
                    preConnectComplete = true;
                    break;
                case EncodeStatus.PublishingPointOpened:
                    if (e.Status == EncodeStatus.PublishingPointOpened)
                        State = Message = "Connected";
                    preConnectComplete = true;
                    break;
                default:
                    Message = e.Message;
                    break;
            }
        }

        void EEService_StateRequestHandler(string State)
        {

        }

        /// <summary>
        /// Used to track session statistics.  Key point is it does not
        /// directly manipulate the UI it changes variable that are data
        /// bound to the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerFrameTrack_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //report the sample statistics
            NumberOfDroppedSamples = _job.NumberOfDroppedSamples;
            NumberOfSamples = _job.NumberOfEncodedSamples;

            //report the time
            SessionTime = string.Format("{0} : {1} : {2}",
                (DateTime.Now - _startTime).Hours,
                (DateTime.Now - _startTime).Minutes,
                (DateTime.Now - _startTime).Seconds);

            if (_numberOfDroppedSamples > 100)
            {
                MessageBox.Show(string.Format("The number of dropped samples is excessive.  Dropped Samples: {0} " +
                    "- This is an indication of poo network performance. Live streaming will be stopped." +
                    " Disable live streaming and turn on archiving.", NumberOfDroppedSamples));
                timerFrameTrack.Stop();
                _job.StopEncoding();

            }
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
