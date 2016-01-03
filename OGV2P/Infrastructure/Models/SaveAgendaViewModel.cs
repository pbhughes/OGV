using Infrastructure.AgendaService;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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

        private StorageService GetStorageClient()
        {
            var client = new AgendaService.StorageService();
            client.Timeout = 10000;
            return client;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> SaveAgenda()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            IsBusy = true;
            Task t = Task.Factory.StartNew(() =>
            {
                
                StorageService client = GetStorageClient();
                client.PreAuthenticate = true;
                client.Timeout = 5000;
                string auth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(_user.UserID + ":" + _user.Password));
                NetworkCredential netCred = new NetworkCredential(_user.UserID, _user.Password);
                string xml = _meeting.GetAgendaXML();
                client.SaveAgendaFile(_user.UserID, _user.Password,
                                                      _user.SelectedBoard.City,_user.SelectedBoard.State, _user.SelectedBoard.Name,
                                                      _meeting.MeetingName + ".oga", xml, DateTime.Now);
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
