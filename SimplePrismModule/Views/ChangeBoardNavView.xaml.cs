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

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for ChangeBoardNavView.xaml
    /// </summary>
    public partial class ChangeBoardNavView : UserControl
    {
        private IRegionManager _regionManager;

        public ChangeBoardNavView()
        {
            InitializeComponent();
            _regionManager =
              Microsoft.Practices.ServiceLocation.ServiceLocator.
                                  Current.GetInstance<Microsoft.
                                  Practices.Prism.Regions.IRegionManager>();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            //Authenticate against the web service async and reject or navigate to
            //Board Selection view
            
           
            Uri vv = new Uri(typeof(Views.LoginView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", vv);
        }
    }
}
