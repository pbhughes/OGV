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
using Microsoft.Practices.Prism.Regions;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for ChooseBoardView.xaml
    /// </summary>
    public partial class ChooseBoardView : UserControl
    {
        public ChooseBoardView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Practices.Prism.Regions.IRegionManager rm =
                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

            Uri vu = new Uri(typeof(Views.AgendaView).FullName, UriKind.RelativeOrAbsolute);
            rm.RequestNavigate("MainRegion", vu);
        }
    }
}
