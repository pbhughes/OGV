using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
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
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Converters;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    /// 
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class LoginView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private IUnityContainer _container;
        private ISession _session;
        private IUser _user;

        private IRegionManager _regionManager;

        public LoginView(IUnityContainer container, ISession session, IUser user)
        {
            InitializeComponent();
            _container = container;
            _session = session;
            _user = user;
            DataContext = _user;
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
            this.Loaded += LoginView_Loaded;
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

        public bool KeepAlive
        {
            get { return false; }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _user.EvaluateLoginCapability();
            if (e.Key == Key.Enter)
            {
                cmdLogin.Focus();

            }
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserID.Focus();
        }
    }
}
