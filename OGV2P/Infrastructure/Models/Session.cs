using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Deployment.Application;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Infrastructure.Models
{
    public delegate void MeetingNameSetEventHandler(object sender, EventArgs e);

    public delegate void StampedEventHandler(TimeSpan sessionTime);

    public delegate void LoggedOutEventHandler();

    public class Session : ISession, INotifyPropertyChanged
    {

        string _localVideoFile;

        public string LocalVideoFile
        {
            get
            {
                return _localVideoFile;
            }

            set
            {
                _localVideoFile = value;
                OnPropertyChanged("LocalVideoFile");
            }
        }

        private TimeSpan _currentVideoTime;
        public TimeSpan CurrentVideoTime
        {
            get
            {
                return _currentVideoTime;
            }

            set
            {
                _currentVideoTime = value;
                OnPropertyChanged("CurrentVideoTime");
            }
        }

    



        private Guid _recoderID;
        public Guid RecorderID
        {
            get { return _recoderID; }
            set { _recoderID = value; }
        }

        private DateTime _initTime;
        public DateTime InitializationTime
        {
            get
            {
                return _initTime;
            }

            set
            {
                _initTime = value;
            }
        }

        public event MeetingNameSetEventHandler RaiseMeetingNameSet;

        public void OnRaiseMeetingSetEvent()
        {
            if (RaiseMeetingNameSet != null)
                RaiseMeetingNameSet(this, new EventArgs());
        }

        public event StampedEventHandler RaiseStamped;

        public void RaiseStampedEvent(TimeSpan sessionTime)
        {
            if (RaiseStamped != null)
                RaiseStamped(sessionTime);
        }

        void ISession.Stamp()
        {
            RaiseStampedEvent(CurrentVideoTime);
        }

        

        public Session()
        {
            InitializationTime = DateTime.Now;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
       

        private void OnPropertyChanged(string name)
        {
           

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public event LoggedOutEventHandler RaiseLoggedOut;

        private void OnLoggedOut()
        {
            if (RaiseLoggedOut != null)
                RaiseLoggedOut();
        }

        public void LogOut()
        {
            OnLoggedOut();
        }
    }
}
