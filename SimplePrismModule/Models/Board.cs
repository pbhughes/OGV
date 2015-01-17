using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;

namespace OGV.Admin.Models
{
    public class Board : INotifyPropertyChanged
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        private ObservableCollection<Agenda> _agendas;
        public ObservableCollection<Agenda> Agendas
        {
            get { return _agendas; }
            set { _agendas = value; OnPropertyChanged("Agendas"); }
        }

       

        private IRegionManager _regionManager;

        private void LoadBoardData()
        {
            
        }

        public Board(string folder)
        {
            // TODO: Complete member initialization
            _name = folder;
            _agendas = new ObservableCollection<Agenda>();

            _regionManager =
               Microsoft.Practices.ServiceLocation.ServiceLocator.
                                   Current.GetInstance<Microsoft.
                                   Practices.Prism.Regions.IRegionManager>();

            
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
