using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Board : IBoard, INotifyPropertyChanged
    {
        private string _city;
        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                _city = value;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        private string _state;
        public string State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }

            set
            {
                _userID = value;
            }
        }



        public bool RequireLogin
        {
            get
            {
                return _requireLogin;
            }

            set
            {
                _requireLogin = value;
                OnPropertyChanged("RequireLogin");
            }
        }

        private string _ftpServer;
        public string FtpServer
        {
            get
            {
                return _ftpServer;
            }

            set
            {
                _ftpServer = value;
                OnPropertyChanged("FtpServer");
            }
        }

        private string _ftpPath;
        public string FtpPath
        {
            get
            {
                return _ftpPath;
            }

            set
            {
                _ftpPath = value;
                OnPropertyChanged("FtpPath");
            }
        }



        private bool _requireLogin;


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
