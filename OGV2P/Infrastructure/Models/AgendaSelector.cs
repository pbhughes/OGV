using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using OGV2P.FTP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.IO;

namespace Infrastructure.Models
{
    public class AgendaSelector : IAgendaSelector, INotifyPropertyChanged
    {
        private static IBoard _board;
        private static IUser _user;
        private static BusyIndicator _indicator;

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

        private List<FTPfileInfo> _availableFiles = new List<FTPfileInfo>();

        public List<FTPfileInfo> AvailableFiles
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

        private Exception _lastError;

        public Exception LastError
        {
            get
            {
                return _lastError;
            }

            set
            {
                _lastError = value;
                OnPropertyChanged("LastError");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

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
            Task t = Task.Factory.StartNew(() =>
           {
               System.Threading.Thread.Sleep(1000);
               FTPclient client = GetStorageClient();

               FTPdirectory dir  = client.ListDirectoryDetail();
               foreach(FTPfileInfo fi in dir )
               {

                   if (fi.FileType == FTPfileInfo.DirectoryEntryTypes.File)
                       _availableFiles.Add(fi);
               }
           });

            try
            {
                t.Wait(new TimeSpan(0, 0, 10));
               
            }
            catch (Exception ex)
            {
                throw;
            }

          
            IsBusy = false;
        }

        public string GetXml(string fileName)
        {
            FTPclient client = GetStorageClient();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas", fileName);
            bool  downloaded = client.Download(fileName, path, true);
            string xml =  File.ReadAllText(path);
            return xml;
        }

        private static FTPclient GetStorageClient()
        {
            Uri ftpUrl = new Uri(string.Format("ftp://{0}/{1}", _user.SelectedBoard.FtpServer, _user.SelectedBoard.FtpPath));
            var client = new FTPclient(ftpUrl.ToString(), _user.UserID, _user.Password);
            
            return client;
        }

        public static async Task<AgendaSelector> Create(IUser user)
        {
            AgendaSelector ags = new AgendaSelector(user);

            return ags;
        }

        public async Task LoadAgendaFiles()
        {
            try
            {
                await GetAgendaFiles();
                OnPropertyChanged("AvailableFiles");
            }
            catch (System.Exception ex)
            {
                IsBusy = false;
                throw;
            }
        }

        public AgendaSelector(IUser user)
        {
            _user = user;
            _board = _user.SelectedBoard;
            GetAgendaFilesCommand = new DelegateCommand(OnGetAgendaFiles, CanGetAgendaFiles);
        }
    }
}