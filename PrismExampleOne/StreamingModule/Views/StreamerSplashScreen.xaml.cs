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
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class StreamerSplashScreen : UserControl
    {
        IRegionManager _regionManager;
        IUserViewModel _user;
        public StreamerSplashScreen(IRegionManager regionManger, IUserViewModel user)
        {
            InitializeComponent();
            _regionManager = regionManger;
            _user = user;
            _user.BoardList.AgendaSelectedEvent += BoardList_AgendaSelectedEvent;
            
        }

        void BoardList_AgendaSelectedEvent(IAgenda selected)
        {
            Uri nn = new Uri(typeof(Views.PublishingPointManagerNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SideNavBarRegion", nn);

            Uri mm = new Uri(typeof(Views.PublishingPointView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SidebarRegion", mm);
        }
    }
}
