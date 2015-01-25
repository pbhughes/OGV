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
using Microsoft.Practices.Prism.Regions;
using System.Threading.Tasks;
using OGV.Admin.Models;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for ChooseBoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl, INavigationAware
    {

        private IUnityContainer _container;
        private IRegionManager _regionManager;

        [InjectionConstructor]
        public BoardView(IUnityContainer container, IUserViewModel userModel)
        {
            InitializeComponent();
            _container = container;
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
            var view = _regionManager.Regions["NavBarRegion"].Views.FirstOrDefault();
            if ( view != null && view is BoardNavView)
                _regionManager.Regions["NavBarRegion"].Remove(view);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //show the Board NAV View in the NAV region
            Uri nn = new Uri(typeof(Views.BoardNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("NavBarRegion", nn);
        }

       
    }
}
