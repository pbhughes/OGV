﻿using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Commands;
using r = Infrastructure.Panopto.RemoteRecorder;
using s = Infrastructure.Panopto.Session;
using Microsoft.Practices.Prism.Regions;
using System.Threading;
using System.Windows;
using System.ServiceProcess;


namespace Infrastructure.Models
{
    public delegate void LoginEventHandler(object sender, EventArgs e);

    public class User : IUser, INotifyPropertyChanged
    {
        private IUnityContainer _container;
        private ISession _session;


        private s.AuthenticationInfo _sessionAuth;

        public s.AuthenticationInfo SessionAuthInfo
        {
            get { return _sessionAuth; }
            set { _sessionAuth = value; }
        }

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
            return true;
        }

        private async void OnLogin()
        {
            try
            {
                IsBusy = true;
                IsCameraServiceReady = await CheckPanoptoService();
                IsFilterServiceReady = await CheckCameraService();

                if (IsCameraServiceReady && IsFilterServiceReady)
                {
                    r.RemoteRecorderManagementClient client = new r.RemoteRecorderManagementClient();
                    r.AuthenticationInfo authInfo = GetRemoteRecorderAuthInfo();
                    r.ListRecordersResponse recorderResults = await client.ListRecordersAsync(authInfo,
                        new Infrastructure.Panopto.RemoteRecorder.Pagination(), r.RecorderSortField.Name);
                    r.RemoteRecorder[] recorders = recorderResults.PagedResults;
                    _session.RecorderID = recorders[0].Id;
                    OnRaiseLoginEvent();
                    OnPropertyChanged("RemoteRecorders");
                }
                else
                {
                    Message = "Camera and Filter services are not running, attempting to start them...";
                    await StartFilterService();
                    await StartPanoptoService();
                    OnLogin();
                }
                
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

        private async Task StartPanoptoService()
        {
            await Task.Run(() =>
            {

                ServiceController controller = GetPanoptoServiceController();
                controller.Start(  );
                Thread.Sleep(2000);
            });
        }

        private static ServiceController GetPanoptoServiceController()
        {
            ServiceController controller = new ServiceController();
            controller.ServiceName = "PanoptoRemoteRecorderService";
            return controller;
        }

        private async Task StartFilterService()
        {

            await Task.Run(() =>
            {
                ServiceController controller = GetSplitCamServiceController();
                controller.Start();
                Thread.Sleep(2000);
            });
        }

        private static ServiceController GetSplitCamServiceController()
        {
            ServiceController controller = new ServiceController();
            controller.ServiceName = "SpliCamService";
            return controller;
        }

        private async Task<bool> CheckCameraService()
        {
            bool retval = false;
            retval = await Task.Run<bool>(() =>
            {
                return (GetPanoptoServiceController().Status == ServiceControllerStatus.Running);
            });

            return retval;
        }

        private async Task<bool> CheckPanoptoService()
        {
            bool retval = false;
            retval = await Task.Run<bool>(() =>
            {
                return (GetSplitCamServiceController().Status == ServiceControllerStatus.Running);
                
            });

            return retval;

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


        public s.AuthenticationInfo GetSessionAuthInfo()
        {
            s.AuthenticationInfo authInfo = new s.AuthenticationInfo()
            {
                Password = this.Password,
                UserKey = this.UserID
            };

            return authInfo;
        }

        public r.AuthenticationInfo GetRemoteRecorderAuthInfo()
        {
            Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo authInfo = new Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo()
            {
                Password = this.Password,
                UserKey = this.UserID
            };

            return authInfo;
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