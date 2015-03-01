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
using OGV.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using OGV.Infrastructure.Interfaces;

namespace OGV.Admin.Models
{
    

    public class BoardList : INotifyPropertyChanged, IBoardList
    {
        private IUnityContainer _container;

        private IRegionManager _regionManager;

        private XService _xService;

        ObservableCollection<IBoard> _boards;
        public ObservableCollection<IBoard> Boards
        {
            get { return _boards; }
            set { _boards = value; }
        }

        private IBoard _selectedBoard;
        public IBoard SelectedBoard
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

        private IAgenda _selectedAgenda;
        public IAgenda SelectedAgenda
        {
            get { return _selectedAgenda; }
            set
            {
                if (_selectedAgenda == value)
                    return;
                _selectedAgenda = value;
                OnPropertyChanged("SelectedAgenda");
                LoadAgendaCommand.RaiseCanExecuteChanged();
                OnAgendaSelected(value);
            }
        }

      
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        public event EventHandler CanExecuteChanged;

        public event AgendaSelectedDelegate AgendaSelectedEvent;

        public DelegateCommand LoadAgendaCommand { get; private set; }

        public DelegateCommand ChooseAgendaCommand { get; private set; }

        public DelegateCommand LogOutCommand { get; private set; }

      


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

        public void SaveAgenda()
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
            SelectedAgenda.OnSave();

            //show the BoardView in the main region
            Uri vv = new Uri(typeof(Views.LoginView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);
        }

        private bool CanLogOut()
        {
            if(SelectedAgenda == null)
                return true;

            if (SelectedAgenda.IsRecording)
                return false;

            return true;
        }

        private bool CanChooseAgenda()
        {
            if (SelectedAgenda != null)
                return !SelectedAgenda.SaveNeeded && !SelectedAgenda.IsRecording;

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

     
        public void OnAgendaSelected(IAgenda selected)
        {
            if (AgendaSelectedEvent != null)
                AgendaSelectedEvent(SelectedAgenda);
        }
        public BoardList()
        {

            this.LoadAgendaCommand = new DelegateCommand(OnLoadAgenda, CanLoadAgenda);
            this.LogOutCommand = new DelegateCommand(OnLogOut, CanLogOut);
            this.ChooseAgendaCommand = new DelegateCommand(OnChooseAgenda, CanChooseAgenda);
        


            _regionManager =
                Microsoft.Practices.ServiceLocation.ServiceLocator.
                                    Current.GetInstance<Microsoft.
                                    Practices.Prism.Regions.IRegionManager>();


            _boards = new ObservableCollection<IBoard>();
            

        }

        public BoardList(IUnityContainer container)
        {

            _container = container;
            this.LoadAgendaCommand = new DelegateCommand(OnLoadAgenda, CanLoadAgenda);
            this.LogOutCommand = new DelegateCommand(OnLogOut, CanLogOut);
            this.ChooseAgendaCommand = new DelegateCommand(OnChooseAgenda, CanChooseAgenda);
       
            _regionManager = 
                Microsoft.Practices.ServiceLocation.ServiceLocator.
                                    Current.GetInstance<Microsoft.
                                    Practices.Prism.Regions.IRegionManager>();


            _boards = new ObservableCollection<IBoard>();
            

        }

        public void Load()
        {

            try
            {
                if (_boards == null)
                    _boards = new ObservableCollection<IBoard>();

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
                            Agenda a = new Agenda();
                            a = a.ParseAgenda(agendaPath) as Agenda;
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
            if (SelectedAgenda != null)
            {
                SelectedAgenda.SaveCommand.RaiseCanExecuteChanged();
                SelectedAgenda.ResetCommand.RaiseCanExecuteChanged();
            }
            
            LoadAgendaCommand.RaiseCanExecuteChanged();
            LogOutCommand.RaiseCanExecuteChanged();
            ChooseAgendaCommand.RaiseCanExecuteChanged();
            
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
