using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Infrastructure.AgendaService;
using Xceed.Wpf.Toolkit;
using System.Windows.Threading;

namespace Infrastructure.Models
{
    public class AgendaSelector : IAgendaSelector, INotifyPropertyChanged
    {
        IBoard _board;
        IUser _user;
        static BusyIndicator _indicator;

        public DelegateCommand GetAgendaFilesCommand { get; set; }

        private string _targetFile;
        public string TargetFile
        {
            get
            {
                return _targetFile;
            }

            set
            {
                _targetFile = value;
                OnPropertyChanged("TargetFile");
            }
        }

        private List<AgendaFile> _availableFiles;
        public List<AgendaFile> AvailableFiles
        {
            get
            {
                return _availableFiles;
            }

            set
            {
                _availableFiles = value;
                OnPropertyChanged("AvailableFiles");
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion


        private bool CanGetAgendaFiles()
        {
            return true;
        }

        private async void OnGetAgendaFiles()
        {
            await GetAgendaFiles();

        }

        private async Task GetAgendaFiles()
        {
            IsBusy = true;
            Task<AgendaFile[]> t =   Task.Factory.StartNew(() => 
             {
                 System.Threading.Thread.Sleep(1000);
                 StorageService client = GetStorageClient();
                 var availableFiles = client.GetAvailableAgendaFiles(_board.City, _board.State, _board.Name);
                 return availableFiles;
             });

            await t;
         
            AvailableFiles = t.Result.ToList<AgendaFile>();
            IsBusy = false;
            
        }

        public string GetXml(string fileName)
        {
            StorageService client = GetStorageClient();
            string xml = client.GetAgendaFileFromWebServer(_user.SelectedBoard.City, _user.SelectedBoard.State, _user.SelectedBoard.Name, fileName);
            return xml;
        }

        private static StorageService GetStorageClient()
        {
            var client = new AgendaService.StorageService();
            client.Timeout = 10000;
            return client;
        }

      
        public static async Task<AgendaSelector> Create(IUser user)
        {
            AgendaSelector ags = new AgendaSelector(user);
    
            return ags;

        }

        public  async Task LoadAgendaFiles()
        {
            await GetAgendaFiles();
        }

        public AgendaSelector( IUser user)
        {
            _user = user;
            _board = _user.SelectedBoard;
            GetAgendaFilesCommand = new DelegateCommand(OnGetAgendaFiles, CanGetAgendaFiles);
        }

        
    }
}
