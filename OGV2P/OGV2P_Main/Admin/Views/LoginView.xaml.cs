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

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager _regionManager;

        public LoginView()
        {
            InitializeComponent();
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri vv = new Uri(typeof(Views.CameraView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SideBarRegion", vv);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool KeepAlive
        {
            get { return false; }
        }
    }
}
