using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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
        private IUnityContainer _container;

        public AgendaNavView(IUnityContainer container, IUserViewModel userModel)
        {
            InitializeComponent();
            _regionManager =
              Microsoft.Practices.ServiceLocation.ServiceLocator.
                                  Current.GetInstance<Microsoft.
                                  Practices.Prism.Regions.IRegionManager>();

            this.DataContext = userModel.BoardList;
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
 
        }
    }
}
