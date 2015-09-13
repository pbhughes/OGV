using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using p = Infrastructure.Panopto.Session;
using System.ComponentModel;

namespace Infrastructure.Models
{
    public delegate void MeetingNameSetEventHandler(object sender, EventArgs e);

    public class Session : ISession, INotifyPropertyChanged
    {
        string _meetingName;

        public string MeetingName
        {
            get { return _meetingName; }
            set { 
                _meetingName = value; 
                OnPropertyChanged("MeetingName");
                OnRaiseMeetingSetEvent();

            }
        }

        private p.Session _currentSession;
        public p.Session CurrentSession
        {
            get { return _currentSession; }
            set { _currentSession = value; OnPropertyChanged("CurrentSession"); }
        }

        private Guid _recoderID;
        public Guid RecorderID
        {
            get { return _recoderID; }
            set { _recoderID = value; }
        }

        public event MeetingNameSetEventHandler RaiseMeetingNameSet;

        public void OnRaiseMeetingSetEvent()
        {
            if (RaiseMeetingNameSet != null)
                RaiseMeetingNameSet(this, new EventArgs());
        }
        public Session()
        {

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
