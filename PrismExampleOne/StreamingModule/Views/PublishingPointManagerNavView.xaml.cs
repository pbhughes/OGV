using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
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
    /// Interaction logic for PublishingPointManagerNavView.xaml
    /// </summary>
    public partial class PublishingPointManagerNavView : UserControl
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;
        private IUserViewModel _user;
        private IPublishingPointMonitor _pubMonitor;
        

        public PublishingPointManagerNavView(IUnityContainer container, IUserViewModel user, IPublishingPointMonitor pubMonitor)
        {
            InitializeComponent();
            _container = container;
            _user = user;
            _pubMonitor = pubMonitor;
            this.DataContext = _pubMonitor;

          
        }

       
    }
}
