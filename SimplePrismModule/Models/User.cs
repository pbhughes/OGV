using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace OGV.Admin.Models
{
    class User: INotifyPropertyChanged
    {
        private int _callTime;

        public int CallTime
        {
            get { return _callTime; }
            set { _callTime = value; OnPropertyChanged("CallTime"); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        private IRegionManager _regionManager;

        public ICommand LoginCommand { get; private set; }

        private async void OnSubmit()
        {
            //Authenticate against the web service async and reject or navigate to
            //Board Selection view
            IsBusy = true;
            
            string token = await Authenticate(_userName, _password);
            OGV.Infrastructure.Model.Session.Instance.Token = token;

            IsBusy = false;
            Uri vv = new Uri(typeof(Views.ChooseBoardView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);
        }

        private bool CanSubmit()
        {
            return ! OGV.Infrastructure.Model.Session.Recording ;
        }

        public User()
        {

            this.LoginCommand = new DelegateCommand(OnSubmit, CanSubmit);
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
        }

        private async Task<string> Authenticate(string userName, string password)
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t =  Task.Run(  () =>
            {
                for (int i = 0; i < 2; i++)
                {
                    Dispatcher.CurrentDispatcher.Invoke((Action)delegate(){ CallTime++; });
                    System.Threading.Thread.Sleep(1000);
                   
                }
            });

            await t;

            return Guid.NewGuid().ToString();

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
