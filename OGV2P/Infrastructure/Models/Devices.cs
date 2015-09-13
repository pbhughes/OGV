using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectX.Capture;
using DShowNET;
using Infrastructure.Panopto.RemoteRecorder;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Infrastructure.Panopto.Session;
using Infrastructure.Interfaces;
using System.Windows.Input;
using System.Timers;

namespace Infrastructure.Models
{
    public class Devices : Infrastructure.Interfaces.IDevices, INotifyPropertyChanged
    {
        private IUser _user;

        private Timer _statusChecknTimer;

        Filters filter = new Filters();

        private ISession _sessionService;

        public ISession SessionService
        {
            get { return _sessionService; }
            set { _sessionService = value; OnPropertyChanged("SessionService"); }
        }

        public List<string> Cameras { get; set; }

        public List<string> Microphones { get; set; }

        public RemoteRecorder[] RemoteRecorders { get; set; }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private ScheduledRecordingResult _currentSession;
        public ScheduledRecordingResult CurrentSession
        {
            get { return _currentSession; }
            set { _currentSession = value; OnPropertyChanged("CurrentSession"); }
        }

        private Guid _currentSessionGuid;
        public Guid CurrentSessionGuid
        {
            get { return _currentSessionGuid; }
            set { _currentSessionGuid = value; OnPropertyChanged("CurrentSessionGuid"); }
        }

        public Folder[] FolderList {get; set;}

        private DelegateCommand _stopRecording;
        public DelegateCommand StopRecording
        {
            get { return _stopRecording; }
            set { _stopRecording = value; }
        }

        private bool CanStopRecording()
        {
            return true;
        }

        private async void OnStopRecording()
        {
            try
            {
                _statusChecknTimer.Stop();
                RemoteRecorderManagementClient client = new RemoteRecorderManagementClient();
                RecorderSettings settings = new RecorderSettings() { RecorderId = CurrentRecorder.Id, SuppressSecondary = true };
                ScheduledRecordingResult response = await 
                    client.UpdateRecordingTimeAsync(_user.GetRemoteRecorderAuthInfo(), _sessionService.CurrentSession.Id, DateTime.Now, DateTime.Now);
            }
            catch (Exception ex)
            {

                throw;
            }
            
           
        }

        private Folder _currentFolder;
        public Folder CurrentFolder
        {
            get { return _currentFolder; }
            set { _currentFolder = value; 
                OnPropertyChanged("CurrentFolder"); }
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

        private async void OnGetFolders()
        {
            Infrastructure.Panopto.Session.SessionManagementClient sessionClient = new SessionManagementClient();
            Infrastructure.Panopto.Session.AuthenticationInfo authInfo = new Panopto.Session.AuthenticationInfo()
            {
                Password = _user.Password,
                UserKey = _user.UserID
            };

            var folderListResponse = await sessionClient.GetCreatorFoldersListAsync(authInfo, new ListFoldersRequest(), null);
            FolderList = folderListResponse.Results;
            OnPropertyChanged("FolderList");
            
        }
        private RemoteRecorder _currentRecorder;
        public RemoteRecorder CurrentRecorder {
            get
            {
                return _currentRecorder;
            }
            set
            {
                _currentRecorder = value;
                OnPropertyChanged("CurrentRecorder");
            }
        }

        private DelegateCommand _startRecording;
        public DelegateCommand StartRecording
        {
            get { return _startRecording; }
            set { _startRecording = value; }
        }

        private DelegateCommand _getRecorders;
        public DelegateCommand GetRecorders
        {
            get { return _getRecorders; }
        }

        private bool CanStartRecording()
        {
            bool val = string.IsNullOrEmpty(_sessionService.MeetingName);

            return !val && CurrentFolder != null && CurrentRecorder != null;
        }

        private async void OnStartRecording()
        {
            try
            {
                IsBusy = true;
                if (_statusChecknTimer == null)
                {
                    _statusChecknTimer = new Timer();
                    _statusChecknTimer.Elapsed += _statusChecknTimer_Elapsed;
                    _statusChecknTimer.Interval = 1000.0;
                }
                _statusChecknTimer.Start();
                RemoteRecorderManagementClient remoteClient = new RemoteRecorderManagementClient();
                Infrastructure.Panopto.Session.SessionManagementClient sessionClient = new Infrastructure.Panopto.Session.SessionManagementClient();
                RecorderSettings settings = new RecorderSettings() { RecorderId = CurrentRecorder.Id, SuppressSecondary = true };
                ScheduledRecordingResult response = await remoteClient.ScheduleRecordingAsync(_user.GetRemoteRecorderAuthInfo(), _sessionService.MeetingName, CurrentFolder.Id, true, DateTime.Now, DateTime.Now.AddHours(1), new RecorderSettings[] { settings });
                Guid sessionID = response.SessionIDs[0];
                Infrastructure.Panopto.Session.Session[] allSessions;
                allSessions = await sessionClient.GetSessionsByIdAsync(_user.GetSessionAuthInfo(), new Guid[] { sessionID });
                SessionService.CurrentSession = allSessions[0];    
                IsBusy = false;
            
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        private async void _statusChecknTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SessionService != null && SessionService.CurrentSession != null)
            {
                Infrastructure.Panopto.Session.SessionManagementClient sessionClient = new Infrastructure.Panopto.Session.SessionManagementClient();
                Infrastructure.Panopto.Session.Session[] allSessions = await sessionClient.GetSessionsByIdAsync(_user.GetSessionAuthInfo(), new Guid[] { SessionService.CurrentSession.Id });
                SessionService.CurrentSession = allSessions[0];
            }
        }

        private bool CanGetRecorders()
        {
            return true;
        }

        private async void OnGetRecorders()
        {
            RemoteRecorderManagementClient client = new RemoteRecorderManagementClient();
            Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo authInfo = new Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo()
            {
                Password = _user.Password,
                UserKey = _user.UserID
            };

            ListRecordersResponse recorderResults = await client.ListRecordersAsync(authInfo, new Infrastructure.Panopto.RemoteRecorder.Pagination(), RecorderSortField.Name);
            RemoteRecorders = recorderResults.PagedResults;


            OnPropertyChanged("RemoteRecorders");
           
        }

        void _sessionService_RaiseMeetingNameSet(object sender, EventArgs e)
        {
            StartRecording.RaiseCanExecuteChanged();
        }

        public Devices(ISession sessionService, IUser user)
        {
            _sessionService = sessionService;
            _user = user;


            Cameras = new List<string>();
            Microphones = new List<string>();
            foreach (Filter c in filter.VideoInputDevices)
            {
                Cameras.Add(c.Name);
            }

            foreach (Filter m in filter.AudioInputDevices)
            {
                Microphones.Add(m.Name);
            }

            _getRecorders = new DelegateCommand(OnGetRecorders, CanGetRecorders);
            OnGetRecorders();

            _getFolders = new DelegateCommand(OnGetFolders, CanGetFolders);
            OnGetFolders();

            _startRecording = new DelegateCommand(OnStartRecording, CanStartRecording);

            _stopRecording = new DelegateCommand(OnStopRecording, CanStopRecording);

            _sessionService.RaiseMeetingNameSet += _sessionService_RaiseMeetingNameSet;
        }

       


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            StartRecording.RaiseCanExecuteChanged();
            
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        
    }
}
