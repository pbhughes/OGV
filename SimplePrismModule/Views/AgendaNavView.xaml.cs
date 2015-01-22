using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using OGV.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for AgendaNavView.xaml
    /// </summary>
    public partial class AgendaNavView : UserControl, INavigationAware  
    {
        private IRegionManager _regionManager;

        public AgendaNavView()
        {
            InitializeComponent();
            _regionManager =
              Microsoft.Practices.ServiceLocation.ServiceLocator.
                                  Current.GetInstance<Microsoft.
                                  Practices.Prism.Regions.IRegionManager>();
        }

       

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //setup the data
            this.DataContext = ServiceLocator.Current.GetInstance<BoardList>();


        }
    }
}
