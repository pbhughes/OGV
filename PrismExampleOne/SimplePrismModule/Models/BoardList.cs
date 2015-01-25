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
using OGV.Infrastructure.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;

namespace OGV.Admin.Models
{
    public class BoardList : INotifyPropertyChanged
    {
        private IUnityContainer _container;

        private IRegionManager _regionManager;

        private XService _xService;

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

        public DelegateCommand SaveAgendaCommand { get; private set; }

        public DelegateCommand LoadAgendaCommand { get; private set; }

        public DelegateCommand ChooseAgendaCommand { get; private set; }

        public DelegateCommand LogOutCommand { get; private set; }

        public DelegateCommand ResetAgendaCommand { get; private set; }

        public DelegateCommand AddNodeCommand { get; private set; }

        public DelegateCommand<AgendaItem> RemoveNodeCommand { get; private set; }

        private async void OnLoadAgenda()
        {
            //navigate to the agenda view
            //show the BoardView in the main region
            Uri vv = new Uri(typeof(Views.AgendaView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);

            Uri uu = new Uri(typeof(Views.AgendaNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("NavBarRegion", uu);
        }

        private bool CanLoadAgenda()
        {
            if(SelectedBoard != null)
                return !(SelectedAgenda == null);

            return false;
        }

        private bool CanSaveAgenda()
        {
            if (SelectedAgenda != null)
            {
                return SelectedAgenda.SaveNeeded;
            }

            return false;
        }

        private void OnSaveAgenda()
        {
            if (SelectedAgenda == null)
                throw new InvalidOperationException("No agenda has been loaded");

            if (string.IsNullOrEmpty(SelectedAgenda.FilePath))
                throw new InvalidOperationException("File name has not been set");

            string allText = SelectedAgenda.ToString();
            File.WriteAllText(SelectedAgenda.FilePath, allText);
            SelectedAgenda.OriginalText = allText;

            //reset the buttons
            ResetButtons();
        }

        private void OnLogOut()
        {
            //TODO: Log out with the server

            //show the BoardView in the main region
            Uri vv = new Uri(typeof(Views.LoginView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);
        }

        private bool CanLogOut()
        {
            return true;
        }

        private bool CanChooseAgenda()
        {
            if(SelectedAgenda != null)
                return ! SelectedAgenda.SaveNeeded;

            return false;
        }

        private void OnChooseAgenda()
        {
            //clear the board and the agenda user is going to get 
            //a new one

            SelectedAgenda = null;
            SelectedBoard = null;
            //navigate to the board view
            //show the BoardView in the main region
            Uri vv = new Uri(typeof(Views.BoardView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);

          
        }

        private bool CanResetAgenda()
        {
            if (SelectedAgenda != null)
            {
                return SelectedAgenda.SaveNeeded;
            }

            return false;
        }

        private void OnResetAgenda()
        {
            SelectedAgenda.Reset();
        }

        private bool CanInsertNode()
        {
            if (SelectedAgenda == null)
                return false;

            return true;
        }

        private void OnInsertNode()
        {
            string newTitleVerbiage = "New Agenda Item... Please add a title";
            if (SelectedAgenda.SelectedItem == null)
                SelectedAgenda.AddItem(new AgendaItem() { Title = newTitleVerbiage  });
            else
                SelectedAgenda.SelectedItem.AddItem(new AgendaItem() { Title = newTitleVerbiage });
        }

        private bool CanRemoveNode(AgendaItem item)
        {
            if (item == null)
                return false;

            if (item.Parent == null)
                return false;

            return true;
        }

        private void OnRemoveNode(AgendaItem item)
        {
            if (item == null)
                return;

            if (item.Parent == null)
                return;

            item.Parent.RemoveItem(item);
        }
        
        public BoardList()
        {

            this.LoadAgendaCommand = new DelegateCommand(OnLoadAgenda, CanLoadAgenda);
            this.LogOutCommand = new DelegateCommand(OnLogOut, CanLogOut);
            this.SaveAgendaCommand = new DelegateCommand(OnSaveAgenda, CanSaveAgenda);
            this.ChooseAgendaCommand = new DelegateCommand(OnChooseAgenda, CanChooseAgenda);
            this.ResetAgendaCommand = new DelegateCommand(OnResetAgenda, CanResetAgenda);
            this.AddNodeCommand = new DelegateCommand(OnInsertNode, CanInsertNode);
            this.RemoveNodeCommand = new DelegateCommand<AgendaItem >(OnRemoveNode, CanRemoveNode);


            _regionManager =
                Microsoft.Practices.ServiceLocation.ServiceLocator.
                                    Current.GetInstance<Microsoft.
                                    Practices.Prism.Regions.IRegionManager>();


            _boards = new ObservableCollection<Board>();
            

        }

        public BoardList(IUnityContainer container)
        {

            _container = container;
            this.LoadAgendaCommand = new DelegateCommand(OnLoadAgenda, CanLoadAgenda);
            this.LogOutCommand = new DelegateCommand(OnLogOut, CanLogOut);
            this.SaveAgendaCommand = new DelegateCommand(OnSaveAgenda, CanSaveAgenda);
            this.ChooseAgendaCommand = new DelegateCommand(OnChooseAgenda, CanChooseAgenda);
            this.ResetAgendaCommand = new DelegateCommand(OnResetAgenda, CanResetAgenda);
            this.AddNodeCommand = new DelegateCommand(OnInsertNode, CanInsertNode);
            this.RemoveNodeCommand = new DelegateCommand<AgendaItem>(OnRemoveNode, CanRemoveNode);
            _regionManager = 
                Microsoft.Practices.ServiceLocation.ServiceLocator.
                                    Current.GetInstance<Microsoft.
                                    Practices.Prism.Regions.IRegionManager>();


            _boards = new ObservableCollection<Board>();
            

        }

        public void Load()
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
                       
                        Application.Current.Dispatcher.Invoke(() => { _boards.Add(b); });
                        
                      
                        
                    }
                });

                Task[] allTasks = { t };
                int count =  Task.WaitAny(allTasks, -1);
                if (count == -1)
                    throw new IOException("The agenda file exists, unable to load boards, check board folder and agenda file formatting");
                

               

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
                Agenda a = new Agenda() { FilePath = filePath };
                XDocument xDoc = XDocument.Parse(allText);
                xDoc.Declaration = null;
                a.MeetingDate = DateTime.Parse(xDoc.Element("meeting").Element("meetingdate").Value);
                a.Name = agenda.Name;
                a.VideoFileName = xDoc.Element("meeting").Element("filename").Value;
                var allAgendaItems = xDoc.Element("meeting").Element("agenda").Element("items").Elements("item");
                foreach(var itemElement in allAgendaItems )
                {

                    AgendaItem ai = ParseAgendaItem(itemElement);
                    ai.ChangedEvent += ItemChanged_Event;
                    a.AddItem(ai);
                }
                a.OriginalText = a.ToString();
                a.ChangedEvent += Agenda_Changed;
                return a;
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
        }

        void Agenda_Changed(object sender, EventArgs e)
        {
            ResetButtons();
        }

        void ItemChanged_Event(object sender, EventArgs e)
        {
            ResetButtons();

        }

        private void ResetButtons()
        {
            SaveAgendaCommand.RaiseCanExecuteChanged();
            LoadAgendaCommand.RaiseCanExecuteChanged();
            LogOutCommand.RaiseCanExecuteChanged();
            ChooseAgendaCommand.RaiseCanExecuteChanged();
            ResetAgendaCommand.RaiseCanExecuteChanged();
            AddNodeCommand.RaiseCanExecuteChanged();
            RemoveNodeCommand.RaiseCanExecuteChanged();
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
                ai.TimeStamp = TimeSpan.Parse(itemElement.Element("timestamp").Value);

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
