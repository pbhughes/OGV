using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Panopto.RemoteRecorder;
using Microsoft.Practices.Prism.Regions;
using System.Threading;
using System.Windows;


namespace Infrastructure.Models
{
    public delegate void LoginEventHandler(object sender, EventArgs e);

    public class User : IUser, INotifyPropertyChanged
    {
        private IUnityContainer _container;
        private ISession _session;

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }


        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand
        {
            get { return _loginCommand; }
            set { _loginCommand = value; }
        }

        private bool CanLogin()
        {
            return true;
        }

        private async void OnLogin()
        {
            try
            {
                IsBusy = true;

                RemoteRecorderManagementClient client = new RemoteRecorderManagementClient();
                Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo authInfo = new Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo()
                {
                    Password = this.Password,
                    UserKey = this.UserID
                };

                ListRecordersResponse recorderResults = await client.ListRecordersAsync(authInfo, new Infrastructure.Panopto.RemoteRecorder.Pagination(), RecorderSortField.Name);
                RemoteRecorder[] recorders = recorderResults.PagedResults;
                _session.RecorderID = recorders[0].Id;

                OnRaiseLoginEvent();

                OnPropertyChanged("RemoteRecorders");
            }
            catch (Exception ex)
            {
                Message = ex.Message;        
            }
            finally
            {
                IsBusy = false;
            }
            
        }

        private string _userID;
        public string UserID
        {
            get { return _userID; }
            set { _userID = value; OnPropertyChanged("UserID"); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        public event LoginEventHandler RaiseLoginEvent;

        private void OnRaiseLoginEvent()
        {
            if (RaiseLoginEvent != null)
                RaiseLoginEvent(this, new EventArgs());
        }
        public User(ISession session)
        {
            _loginCommand = new DelegateCommand(OnLogin, CanLogin);
            _session = session;

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
