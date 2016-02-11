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
using Microsoft.Practices.Prism.Regions;
using Infrastructure.Models;
using forms = System.Windows.Forms;

namespace OGV2P
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window, IDisposable
    {
        IUnityContainer _container;
        IMeeting _meeting;
        private PerformanceCounter cpuCounter;
        private System.Timers.Timer cpuReadingTimer;
        private IRegionManager _regionManager;


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

        private void File_LogOutMenu_Click(object sender, RoutedEventArgs e)
        {
            

            if (_meeting.IsBusy || _meeting.HasChanged)
            {
                if (_meeting.IsBusy)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show(
                        "A recording is in progress.  Continue to close?",
                        "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        e.Handled = true;
                        return;
                    }
                }

                if (_meeting.HasChanged)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show(
                        "The agenda file changes if you close without saving you will lose them.  Do you want to Continue?",
                        "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        e.Handled = true;
                        return ;
                    }
                }
            }

            
                _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

                var loginView = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("LoginView");

                if (loginView == null)
                {
                    loginView = new OGV2P.Admin.Views.LoginView(_container,
                        _container.Resolve<ISession>(), _container.Resolve<IUser>());
                    _regionManager.Regions[Infrastructure.Models.Regions.Middle].Add(loginView, "LoginView");
                }

                _regionManager.Regions[Infrastructure.Models.Regions.Middle].Activate(loginView);

                foreach (var view in _regionManager.Regions[Infrastructure.Models.Regions.Main].Views)
                {
                    _regionManager.Regions[Infrastructure.Models.Regions.Main].Remove(view);
                }
            

        }

        private void BoardsFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            Process.Start(forms.Application.LocalUserAppDataPath);
        }

        private void ExceptionLog_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(forms.Application.LocalUserAppDataPath);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_meeting.IsBusy || _meeting.HasChanged)
            {
                if (_meeting.IsBusy)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show(
                        "A recording is in progress.  Continue to close?",
                        "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                if (_meeting.HasChanged)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show(
                        "The agenda file changes if you close without saving you will lose them.  Do you want to Continue?",
                        "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }


          
        }

        public void Dispose()
        {
            if (cpuCounter != null)
                cpuCounter.Dispose();

            if (cpuReadingTimer != null)
                cpuReadingTimer.Dispose();
        }
    }
}
