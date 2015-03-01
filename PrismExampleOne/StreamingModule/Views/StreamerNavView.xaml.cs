using Microsoft.Practices.Prism.Regions;
using OGV.Infrastructure.Interfaces;
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

namespace OGV.Streaming.Views
{
    /// <summary>
    /// Interaction logic for StreamerNavView.xaml
    /// </summary>
    public partial class StreamerNavView : UserControl, INavigationAware
    {
        IRegionManager _regionManager;
        IUserViewModel _user;
        public StreamerNavView(IRegionManager regionManger, IUserViewModel user)
        {
            InitializeComponent();
            _regionManager = regionManger;
            _user = user;
            this.DataContext = _user.BoardList.SelectedAgenda;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //navigate to the streamer control
            Uri vv = new Uri(typeof(Views.PublishingPointView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SidebarRegion", vv);

            Uri xx = new Uri(typeof(Views.PublishingPointManagerNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SideNavBarRegion", xx);
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
            this.DataContext = _user.BoardList.SelectedAgenda;
        }
    }
}
