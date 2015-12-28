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
using System.Diagnostics;
using System.Timers;
using OGV2P.Admin.Views;

namespace OGV2P
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        IUnityContainer _container;
        IMeeting _meeting;
        private PerformanceCounter cpuCounter;
        private System.Timers.Timer cpuReadingTimer;



        public void SetSideBarAllignmentTop( )
        {
            SideBarRegion.VerticalContentAlignment = VerticalAlignment.Top;
            SideBarRegion.VerticalAlignment = VerticalAlignment.Top;
        }

        public Shell(IUnityContainer container, IMeeting meeting)
        {
            InitializeComponent();
            _container = container;
            _meeting = meeting;
            DataContext = _meeting;
            _meeting.LeftStatus = "Stream Idle";

            // initialize performance counter
            cpuReadingTimer = new Timer();
            cpuReadingTimer.Interval = 1000;
            cpuReadingTimer.Elapsed += cpuReadingTimer_Elapsed;
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuReadingTimer.Start();
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
                    "A recording is in progress, you will lose changes if changes have been made.  Continue to close?",
                    "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            Application.Current.Shutdown(0);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void cpuReadingTimer_Elapsed(object p1, object p2)
        {
            // get the CPU reading
            Dispatcher.Invoke(() =>
            {
                float cpuUtilization = cpuCounter.NextValue();
                shellPBar.Value = cpuUtilization;
                txtCPUReading.Text = string.Format("CPU: {0}", cpuUtilization.ToString("n2") + "%");
            });

        }

        private void File_SettingsMenu_CLick(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingWindowDialog diag = new SettingWindowDialog(_meeting.LandingPage, _meeting.LocalFile, _meeting.PublishingPoint);
                diag.ShowDialog();
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }

        }
    }
}
