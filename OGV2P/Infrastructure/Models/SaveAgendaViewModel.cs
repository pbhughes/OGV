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
            Uri ftpUrl = new Uri(string.Format("ftp://{0}/{1}", _user.SelectedBoard.FtpServer, _user.SelectedBoard.FtpPath));
            var client = new FTPclient(_user.SelectedBoard.FtpServer, _user.UserID, _user.Password);

            return client;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> SaveAgenda()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            IsBusy = true;
            Task t = Task.Factory.StartNew(() =>
            {
                
                FTPclient client = GetStorageClient();
                string xml = _meeting.GetAgendaXML();
                client.Upload(_meeting.LocalAgendaFileName);
            });

            try
            {
                t.Wait();
                return true;
            }
            catch (Exception ex)
            {

                if (t.Exception.InnerException != null)
                    throw t.Exception.InnerException;
                else
                    throw t.Exception;
            }
            finally
            {
                IsBusy = false;
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
