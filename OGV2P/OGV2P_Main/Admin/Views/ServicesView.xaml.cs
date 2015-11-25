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
using System.ServiceProcess;
using Microsoft.Practices.Prism.Regions;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using System.IO;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    /// 
    [RegionMemberLifetime(KeepAlive = true)]
    public partial class ServicesView : UserControl, INavigationAware, IRegionMemberLifetime, INotifyPropertyChanged
    {

        private IUnityContainer _container;
        private IRegionManager _regionManager;
        private IMeeting _meeting;
        public IMeeting Meeting
        {
            get
            {
                return _meeting;
            }

            set
            {
                _meeting = value;
                OnPropertyChanged("Meeting");
            }
        }
        private IUser _user;
        private ISession _session;

        public bool KeepAlive
        {
            get
            {
                return true;
            }
        }

        public IRegionManager RegionManager
        {
            get
            {
                return _regionManager;
            }

            set
            {
                _regionManager = value;
            }
        }

       

        public ServicesView(IRegionManager regionManager, IMeeting meeting, IUser user, ISession session, IUnityContainer container)
        {
            this.Loaded += ServicesView_Loaded;
            RegionManager = regionManager;
            _meeting = meeting;
            _user = user;
            _session = session;
            InitializeComponent();
            this.Name = "SettingsView";
            _container = container;
            this.DataContext = Meeting;

        }

        async void  ServicesView_Loaded(object sender, RoutedEventArgs e)
        {
            Meeting = _container.Resolve<IMeeting>();
            txtPublishingPoint.Text = Meeting.PublishingPoint;
            txtUrl.Text = Meeting.LandingPage;
            txtLocalFile.Text = Meeting.LocalFile;
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }




        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtUrl.Text == null || txtUrl.Text == string.Empty)
                ;//do nothing
            else
                System.Diagnostics.Process.Start(txtUrl.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(! string.IsNullOrEmpty(txtLocalFile.Text))
            {
                string fullPath = txtLocalFile.Text;
                var dInfo = Directory.GetParent(fullPath);
                System.Diagnostics.Process.Start(dInfo.FullName);
            }
        }
    }
}
