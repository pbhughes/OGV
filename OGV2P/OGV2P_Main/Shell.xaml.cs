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
using CustomControls;

namespace OGV2P
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
       
        public void SetSideBarAllignmentTop( )
        {
            SideBarRegion.VerticalContentAlignment = VerticalAlignment.Top;
            SideBarRegion.VerticalAlignment = VerticalAlignment.Top;
        }

        public Shell()
        {
            InitializeComponent();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new CustomControls.Views.AboutWindow("support@clerkbase.com");
            window.ShowDialog();
        }
    }
}
