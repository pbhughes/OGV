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

namespace Infrastructure.Models
{
    public class Devices : Infrastructure.Interfaces.IDevices, INotifyPropertyChanged
    {
        Filters filter = new Filters();

        private ISession _sessionService;

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

        private string _title;
        public string Title {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                
                OnPropertyChanged("Title");
                
            }
        }

        public string Password { get; set; }

        public string UserID { get; set; }

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
            RemoteRecorderManagementClient client = new RemoteRecorderManagementClient();
            Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo authInfo = new Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo()
            {
                Password = this.Password,
                UserKey = this.UserID
            };
            RecorderSettings settings = new RecorderSettings() { RecorderId = CurrentRecorder.Id, SuppressSecondary = true };
            
            ScheduledRecordingResult response = await client.UpdateRecordingTimeAsync(authInfo,CurrentSessionGuid, DateTime.Now, DateTime.Now);
           
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
                Password = this.Password,
                UserKey = this.UserID
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
            bool val = string.IsNullOrEmpty(_title);

            return !val && CurrentFolder != null && CurrentRecorder != null;
        }

        private async void OnStartRecording()
        {
            IsBusy = true;
            RemoteRecorderManagementClient client = new RemoteRecorderManagementClient();
            Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo authInfo = new Infrastructure.Panopto.RemoteRecorder.AuthenticationInfo()
            {
                Password = this.Password,
                UserKey = this.UserID
            };
            RecorderSettings settings = new RecorderSettings() { RecorderId = CurrentRecorder.Id, SuppressSecondary=true };
            ScheduledRecordingResult response = await client.ScheduleRecordingAsync(authInfo, _title, CurrentFolder.Id, true, DateTime.Now, DateTime.Now.AddHours(1), new RecorderSettings[] { settings });
            CurrentSessionGuid = response.SessionIDs[0];
            _sessionService.CurrentSession = CurrentSessionGuid;
            IsBusy = false;
            
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
                Password = this.Password,
                UserKey = this.UserID
            };

            ListRecordersResponse recorderResults = await client.ListRecordersAsync(authInfo, new Infrastructure.Panopto.RemoteRecorder.Pagination(), RecorderSortField.Name);
            RemoteRecorders = recorderResults.PagedResults;


            OnPropertyChanged("RemoteRecorders");
           
        }

        public Devices(ISession sessionService)
        {
            _sessionService = sessionService;
            UserID = "barkley";
            Password = "seri502/dupe";
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
