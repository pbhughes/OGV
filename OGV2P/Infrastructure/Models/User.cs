using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;

namespace Infrastructure.Models
{
    public class User : IUser, INotifyPropertyChanged
    {
        private ISession _session;

        private bool _isReady;

        public bool IsReady
        {
            get { return _isReady; }
            set { _isReady = value; OnPropertyChanged("IsReady"); }
        }

        private bool _isFilterServiceReady;

        public bool IsFilterServiceReady
        {
            get { return _isFilterServiceReady; }
            set { _isFilterServiceReady = value; OnPropertyChanged("IsFilterServiceReady"); }
        }

        private bool _isCameraServiceReady;

        public bool IsCameraServiceReady
        {
            get { return _isCameraServiceReady; }
            set { _isCameraServiceReady = value; OnPropertyChanged("IsCameraServiceReady"); }
        }

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
            return SelectedBoard != null;
        }

        private async void OnLogin()
        {
            try
            {
                IsBusy = true;

                if (SelectedBoard.RequireLogin)
                {
                    if (string.IsNullOrEmpty(UserID))
                    {
                        Message = "Login required enter a valid user ID and password.";
                        return;
                    }

                    if (string.IsNullOrEmpty(Password))
                    {
                        Message = "Login required enter a valid user ID and password.";
                        return;
                    }

                    if (UserID.ToLower() != SelectedBoard.UserID.ToLower())
                    {
                        Message = "Invalid user id or password";
                        throw new UnauthorizedAccessException("Invalid user ID or password.");
                    }

                    if (Password.ToLower() != SelectedBoard.Password.ToLower())
                    {
                        Message = "Invalid user ID or password";
                        throw new UnauthorizedAccessException("Invalid user ID or password");
                    }
                }
                else
                {
                    UserID = SelectedBoard.UserID;
                    Password = SelectedBoard.Password;
                }

                Message = string.Empty;
                OnRaiseLoginEvent();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("An error occured loading the user, see error text: {0}", ex.Message));
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
            set { _userID = value; OnPropertyChanged("UserID"); LoginCommand.RaiseCanExecuteChanged(); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); LoginCommand.RaiseCanExecuteChanged(); }
        }

        private IBoardList _boards;

        public IBoardList Boards
        {
            get
            {
                return _boards;
            }

            set
            {
                _boards = value;
                OnPropertyChanged("Boards");
            }
        }

        private IBoard _selectedBoard;

        public IBoard SelectedBoard
        {
            get
            {
                return _selectedBoard;
            }

            set
            {
                _selectedBoard = value;
                OnPropertyChanged("SelectedBoard");
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public event LoginEventHandler RaiseLoginEvent;

        private void OnRaiseLoginEvent()
        {
            if (RaiseLoginEvent != null)
                RaiseLoginEvent(this, new EventArgs());
        }

        public User(ISession session, IBoardList boards)
        {
            _loginCommand = new DelegateCommand(OnLogin, CanLogin);
            _session = session;
            _boards = boards;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void EvaluateLoginCapability()
        {
            LoginCommand.RaiseCanExecuteChanged();
        }

        #endregion INotifyPropertyChanged
    }
}