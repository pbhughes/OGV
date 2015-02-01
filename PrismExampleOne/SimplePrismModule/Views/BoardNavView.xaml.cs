using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
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
using OGV.Infrastructure.Services;
using Microsoft.Practices.Unity;
using OGV.Infrastructure.Interfaces;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for BoardViewNav.xaml
    /// </summary>
    public partial class BoardNavView : UserControl, INavigationAware
    {
        private IUnityContainer _container;
        private IRegionManager _regionManager;

        [InjectionConstructor]
        public BoardNavView(IUnityContainer container, IUserViewModel userModel)
        {
            InitializeComponent();
            _container = container;

            this.DataContext = userModel;
            
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
