using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Infrastructure.AgendaService;

namespace Infrastructure.Models
{
    public class AgendaSelector : IAgendaSelector, INotifyPropertyChanged
    {
        IBoard _board;
        IUser _user;

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

        private void OnGetAgendaFiles()
        {
            GetAgendaFiles();

        }

        private AgendaFile[] GetAgendaFiles()
        {
            var client = new AgendaService.StorageService();
            client.Timeout = 10;
            var availableFiles = client.GetAvailableAgendaFiles(_board.City, _board.State, _board.Name);
            return availableFiles;
        }

        public AgendaSelector(IBoard board, IUser user)
        {
            _board = board;
            _user = user;
            GetAgendaFilesCommand = new DelegateCommand(OnGetAgendaFiles, CanGetAgendaFiles);
            AvailableFiles = GetAgendaFiles().ToList();

        }

        
    }
}
