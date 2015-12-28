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
using System.Diagnostics;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    /// 
    [RegionMemberLifetime(KeepAlive = true)]
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
            if(_user.Boards.Boards.Count >0)
            {
                _user.SelectedBoard = _user.Boards.Boards[0];
            }

            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
            this.Loaded += LoginView_Loaded;
            this.Name = "LoginView";
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

            DecorateLoginScreen();
        }

        private void DecorateLoginScreen()
        {
            if (_user.SelectedBoard.RequireLogin)
            {
                pnlPassword.Visibility = Visibility.Visible;
                pnlUserID.Visibility = Visibility.Visible;
                cmdLogin.Content = "Login";
            }
            else
            {
                pnlPassword.Visibility = Visibility.Collapsed;
                pnlUserID.Visibility = Visibility.Collapsed;
                cmdLogin.Content = "Start";

            }
        }

        private void cmdLogin_Click(object sender, RoutedEventArgs e)
        {
            txtUserID.SelectAll();
            txtUserID.Focus();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.SelectAll();
        }

        private void lstBoards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DecorateLoginScreen();
            txtUserID.Focus();
            txtUserID.SelectAll();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

    }
}
