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
