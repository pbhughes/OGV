using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Infrastructure.Interfaces;
using System.Windows.Input;
using System.Timers;

namespace Infrastructure.Models
{
    public class Devices : Infrastructure.Interfaces.IDevices, INotifyPropertyChanged
    {
        private IUser _user;

        private ISession _sessionService;

        public ISession SessionService
        {
            get { return _sessionService; }
            set { _sessionService = value; OnPropertyChanged("SessionService"); }
        }

        public List<string> Cameras { get; set; }

        public List<string> Microphones { get; set; }


        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }


        private Guid _currentSessionGuid;
        public Guid CurrentSessionGuid
        {
            get { return _currentSessionGuid; }
            set { _currentSessionGuid = value; OnPropertyChanged("CurrentSessionGuid"); }
        }


        
        

        private DelegateCommand _getFolders;
        public DelegateCommand GetFolders
        {
            get { return _getFolders; }
            set { _getFolders = value; }
        }

        private bool CanGetFolders()
        {
            return true;
        }


         

        private bool CanGetRecorders()
        {
            return true;
        }

       
        void _sessionService_RaiseMeetingNameSet(object sender, EventArgs e)
        {
        }

        public Devices(ISession sessionService, IUser user)
        {
            _sessionService = sessionService;
            _user = user;


            Cameras = new List<string>();
            Microphones = new List<string>();

          

            _sessionService.RaiseMeetingNameSet += _sessionService_RaiseMeetingNameSet;
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
