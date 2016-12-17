using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using OGV2P.FTP.Utilities;

namespace Infrastructure.Models
{
    public class SaveAgendaViewModel : ISaveAgendaViewModel, INotifyPropertyChanged
    {
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

        private IUser _user;
        public IUser User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
                OnPropertyChanged("User");
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

        private FTPclient GetStorageClient()
        {
            Uri ftpUrl = new Uri(string.Format("ftp://{0}", _user.SelectedBoard.FtpServer));

            var client = new FTPclient(ftpUrl.ToString(), _user.UserID, _user.Password);
            client.CurrentDirectory = string.Format("/{0}", _user.SelectedBoard.FtpPath);
            return client;
        }

        public void SaveAgenda()
        {
            FTPclient client = GetStorageClient();
            bool saved = client.Upload(_meeting.LocalAgendaFileName);
            if (!saved)
            {
                var mxgBox = new Xceed.Wpf.Toolkit.MessageBox();
                mxgBox.Text = string.Format("Agenda {0} saved.", _meeting.MeetingName);
                mxgBox.ShowDialog();
            }
        }


        public SaveAgendaViewModel(IUser user, IMeeting meeting)
        {
            _user = user;
            _meeting = meeting;
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
