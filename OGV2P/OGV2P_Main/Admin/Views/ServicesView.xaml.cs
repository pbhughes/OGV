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

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    public partial class ServicesView : UserControl
    {
        private IRegionManager _regionManager;

        public ServicesView()
        {
            this.Loaded += ServicesView_Loaded;
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
            InitializeComponent();

        }

        async void  ServicesView_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceController recorder = new ServiceController(OGV2P.Admin.Properties.Settings.Default.PanoptoRecorderServiceName);
            ServiceController uploader = new ServiceController(OGV2P.Admin.Properties.Settings.Default.PanoptoUploadService);

            if (recorder.Status != ServiceControllerStatus.Running || uploader.Status != ServiceControllerStatus.Running)
                System.Diagnostics.Process.Start("services.msc");
            else
            {
                Uri vv = new Uri(typeof(Views.LoginView).FullName, UriKind.RelativeOrAbsolute);
                _regionManager.RequestNavigate("SideBarRegion", vv);
            }
           
        }
    }
}
