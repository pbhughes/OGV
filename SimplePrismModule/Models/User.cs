using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
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

    public class User: INotifyPropertyChanged
    {
        private IUnityContainer _container;

        private IRegionManager _regionManager;

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

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }


        public ICommand LoginCommand { get; private set; }

        private async void OnLogin()
        {
            //Authenticate against the web service async and reject or navigate to
            //Board Selection view
            Message = "Authenticating...";
            IsBusy = true;
            
            string token = await Authenticate(_userName, _password);
            OGV.Infrastructure.Model.Session.Instance.Token = token;

            //down load all the board files
            Message = "Downloading Agenda Files...";
            List<Agenda> files = await DownLoadAgendaFiles(""); //get the url from the authentication token

            //load all the board files
            Message = "Loading Agenda Files...";
            

            IsBusy = false;
            //show the BoardView in the main region
            Uri vv = new Uri(typeof(Views.BoardView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);

            //show the Board NAV View in the NAV region
            Uri nn = new Uri(typeof(Views.BoardNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("NavBarRegion", nn);
        }

        private bool CanLogin()
        {
            return ! OGV.Infrastructure.Model.Session.Recording ;
        }

        public User()
        {

            this.LoginCommand = new DelegateCommand(OnLogin, CanLogin);
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
        }

        public User(IUnityContainer container)
        {

            this.LoginCommand = new DelegateCommand(OnLogin, CanLogin);
            _container = container;
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

        private async Task<List<Agenda>> DownLoadAgendaFiles(string url)
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t = Task.Run(() =>
            {
                for (int i = 0; i < 2; i++)
                {
                    Dispatcher.CurrentDispatcher.Invoke((Action)delegate() { CallTime++; });
                    System.Threading.Thread.Sleep(1000);

                }
            });

            await t;

            return new List<Agenda>();

        }

        private async Task<string> LoadAgendaFiles()
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t = Task.Run(() =>
            {
                for (int i = 0; i < 2; i++)
                {
                    Dispatcher.CurrentDispatcher.Invoke((Action)delegate() { CallTime++; });
                    System.Threading.Thread.Sleep(1000);

                    _container.RegisterInstance<BoardList>(new BoardList(_container));
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
