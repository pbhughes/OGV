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
using Microsoft.Practices.Unity;
using Infrastructure.Interfaces;

namespace OGV2P
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        IUnityContainer _container;
       
        public void SetSideBarAllignmentTop( )
        {
            SideBarRegion.VerticalContentAlignment = VerticalAlignment.Top;
            SideBarRegion.VerticalAlignment = VerticalAlignment.Top;
        }

        public Shell(IUnityContainer container)
        {
            InitializeComponent();
            _container = container;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new CustomControls.Views.AboutWindow("support@clerkbase.com");
            window.ShowDialog();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IMeeting meeting = _container.Resolve<IMeeting>();

            if (meeting.IsBusy)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(
                    "A recording is in progress, you will lose changes if changes have been made.  Continue to close?", "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            Application.Current.Shutdown(0);
        }
    }
}
