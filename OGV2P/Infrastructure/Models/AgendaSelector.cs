using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using OGV2P.FTP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class AgendaSelector : IAgendaSelector, INotifyPropertyChanged
    {
        private static IBoard _board;
        private static IUser _user;
        private string _path;
        private FTPclient _client;

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

        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                OnPropertyChanged("Text");
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

        private int _percentDone;

        public int PercentDone
        {
            get
            {
                return _percentDone;
            }

            set
            {
                _percentDone = value;
                OnPropertyChanged("PercentDone");
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

        public string GetSelectedAgendaXML(string fileName)
        {
            try
            {
                _path = fileName;
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas", fileName);
                if (File.Exists(path))
                    File.Delete(path);

                bool downloaded = _client.Download(fileName, path, true);
                _targetFile = path;
                FileStream fs = new FileStream(_targetFile, FileMode.Open);
                while (!fs.CanRead)
                {
                    System.Diagnostics.Debug.WriteLine("Waiting on file {0}", _targetFile);
                    System.Threading.Thread.Sleep(100);
                }

                fs.Close();
                Text = File.ReadAllText(_targetFile);

                return Text;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Client_ReportProgress(int PercentDone)
        {
            if (PercentDone >= 100)
            {
                Text = File.ReadAllText(_path);
            }
        }

        private static FTPclient GetStorageClient()
        {
            Uri ftpUrl = new Uri(string.Format("ftp://{0}", _user.SelectedBoard.FtpServer));

            var client = new FTPclient(ftpUrl.ToString(), _user.UserID, _user.Password);
            client.CurrentDirectory = string.Format("/{0}", _user.SelectedBoard.FtpPath);
            return client;
        }

        public static async Task<AgendaSelector> Create(IUser user)
        {
            return await Task.Run<AgendaSelector>(() =>
           {
               AgendaSelector ags = new AgendaSelector(user);

               return ags;
           });
        }

        public List<FTPfileInfo> ListAgendaFilesOnServer()
        {
            List<FTPfileInfo> files = new List<FTPfileInfo>();
            FTPclient client = GetStorageClient();

            FTPdirectory dir = client.ListDirectoryDetail();
            foreach (FTPfileInfo fi in dir)
            {
                if (fi.FileType == FTPfileInfo.DirectoryEntryTypes.File)
                    files.Add(fi);
            }
            return files;
        }

        public AgendaSelector(IUser user)
        {
            _user = user;
            _board = _user.SelectedBoard;

            _client = GetStorageClient();
        }
    }
}