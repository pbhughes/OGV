using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;

namespace OGV.Admin.Models
{
    public class BoardList : INotifyPropertyChanged
    {

        private IRegionManager _regionManager;

        ObservableCollection<Board> _boards;
        public ObservableCollection<Board> Boards
        {
            get { return _boards; }
            set { _boards = value; }
        }

        private Board _selectedBoard;
        public Board SelectedBoard
        {
            get { return _selectedBoard; }
            set
            {
                if (_selectedBoard == value)
                    return;
                _selectedBoard = value;
                OnPropertyChanged("SelectedBoard");
                LoadAgendaCommand.RaiseCanExecuteChanged();
              
            }
        }

        private Agenda _selectedAgenda;
        public Agenda SelectedAgenda
        {
            get { return _selectedAgenda; }
            set
            {
                if (_selectedAgenda == value)
                    return;
                _selectedAgenda = value;
                OnPropertyChanged("SelectedAgenda");
                LoadAgendaCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        public event EventHandler CanExecuteChanged;

        public DelegateCommand LoadAgendaCommand { get; private set; }

        private async void OnLoadAgenda()
        {
            //Authenticate against the web service async and reject or navigate to
            //Board Selection view
            IsBusy = true;
            NavigationParameters navParams = new NavigationParameters();
            navParams.Add("agenda", SelectedAgenda);
            IsBusy = false;
            Uri vv = new Uri(typeof(Views.AgendaView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv, navParams);
        }

        private bool CanLoadAgenda()
        {

            return !(SelectedAgenda == null);
        }

        public BoardList()
        {
            this.LoadAgendaCommand = new DelegateCommand(OnLoadAgenda, CanLoadAgenda);
            
          
            _regionManager = 
                Microsoft.Practices.ServiceLocation.ServiceLocator.
                                    Current.GetInstance<Microsoft.
                                    Practices.Prism.Regions.IRegionManager>();
          

            _boards = new ObservableCollection<Board>();
            Task loadTask = Task.Run( async () => { await Load(); });

        }

      
        public async Task<ObservableCollection<Board>> Load()
        {

            try
            {
                if (_boards == null)
                    _boards = new ObservableCollection<Board>();

                if( ! Directory.Exists("Agendas"))
                {
                    throw new FileNotFoundException("Missing agendas folder, please download the agenda files");
                }

                Task t = Task.Run(( ) => {
                    var boards = Directory.GetDirectories("Agendas");
                    foreach (var folder in boards)
                    {
                        DirectoryInfo di = new DirectoryInfo(folder);
                        Board b = new Board(di.Name);
                        FileSystemInfo[] agendaFiles = di.GetFileSystemInfos();
                        
                        foreach (var agendaPath in agendaFiles)
                        {
                            Agenda a = ParseAgenda(agendaPath);
                            b.Agendas.Add(a);
                        }
                       
                        Application.Current.Dispatcher.Invoke(() => { Boards.Add(b); });
                      
                        
                    }
                });

                Task[] allTasks = { t };
                int count =  Task.WaitAny(allTasks, -1);
                if (count == -1)
                    throw new IOException("The agenda file exists, unable to load boards, check board folder and agenda file formating");
                

                return _boards;

            }
            catch (Exception ex)
            {
                
                throw;
            }
            finally
            {

            }
        }

        public Agenda ParseAgenda(FileSystemInfo agenda)
        {
            try
            {
                string allText = File.ReadAllText(agenda.FullName);
                string filePath = agenda.FullName;
                Agenda a = new Agenda() { OriginalText = allText, FilePath = filePath };
                XDocument xDoc = XDocument.Parse(a.OriginalText);
                a.MeetingDate = DateTime.Parse(xDoc.Element("meeting").Element("meetingdate").Value);
                a.Name = agenda.Name;
                var allAgendaItems = xDoc.Element("meeting").Element("agenda").Element("items").Elements("item");
                foreach(var itemElement in allAgendaItems )
                {

                    AgendaItem ai = ParseAgendaItem(itemElement);
                    a.Items.Add(ai);
                }

                return a;
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
        }

        private static AgendaItem ParseAgendaItem(XElement itemElement)
        {
            AgendaItem ai = new AgendaItem()
            {
                Title = (string)itemElement.Element("title") ?? "",
                Description = (string)itemElement.Element("desc") ?? "",
                Frame = long.Parse((string)itemElement.Element("frame") ??"0"),
                TimeStamp = TimeSpan.Parse((string)itemElement.Element("timestamp")?? (new TimeSpan(0,0,0)).ToString())
            };

            if (itemElement.Element("timestamp") != null)
                ai.TimeStamp = (TimeSpan)itemElement.Element("timestamp");

            if (itemElement.Element("items") != null)
            {
                foreach (var subItem in itemElement.Element("items").Elements("item"))
                {
                    var subAgendaItem = ParseAgendaItem(subItem);
                    ai.Items.Add(subAgendaItem);
                }
            }
            return ai;
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
