using Microsoft.Practices.Prism.Regions;
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
using OGV.Admin.Models;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for AgendaView.xaml
    /// </summary>
    public partial class AgendaView : UserControl, INavigationAware
    {
        
        private IRegionManager _regionManager;

        public AgendaView()
        {
            InitializeComponent();

            
            _regionManager =
              Microsoft.Practices.ServiceLocation.ServiceLocator.
                                  Current.GetInstance<Microsoft.
                                  Practices.Prism.Regions.IRegionManager>();

            this.DataContext = ServiceLocator.Current.GetInstance<BoardList>().SelectedBoard.SelectedAgenda;
        }


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            var view = _regionManager.Regions["NavBarRegion"].Views.FirstOrDefault();
            if (view != null)
                _regionManager.Regions["NavBarRegion"].Remove(view);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //Setup the NAV view
            Uri nn = new Uri(typeof(Views.AgendaNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("NavBarRegion", nn);
                      


        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
        }
    }
}
