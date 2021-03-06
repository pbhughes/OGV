﻿using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OGV2P.Admin.Views;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using forms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;

namespace OGV2P
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window, IDisposable
    {
        private IUnityContainer _container;
        private IMeeting _meeting;
        private PerformanceCounter cpuCounter;
        private System.Timers.Timer cpuReadingTimer;
        private IRegionManager _regionManager;
       

        public void SetSideBarAllignmentTop()
        {
            SideBarRegion.VerticalContentAlignment = VerticalAlignment.Top;
            SideBarRegion.VerticalAlignment = VerticalAlignment.Top;
        }

        public Shell(IUnityContainer container)
        {
            InitializeComponent();
            _container = container;
            _meeting = _container.Resolve<IMeeting>();
            _meeting.RaiseMeetingSetEvent += _meeting_RaiseMeetingSetEvent;
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

        private void _meeting_RaiseMeetingSetEvent(object sender, EventArgs e)
        {
            string x = _meeting.MeetingName;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new CustomControls.Views.AboutWindow("support@opengovideo.com");
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
                SettingWindowDialog diag = new SettingWindowDialog(_meeting.LandingPage ?? "http://www.opengovideo.com/ogv2help", _meeting.LocalFile, _meeting.PublishingPoint);
                diag.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void File_LogOutMenu_Click(object sender, RoutedEventArgs e)
        {
            var session = _container.Resolve<ISession>();

            if (_meeting.IsBusy)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(
                        "A recording is in progress if you want to log out please stop the recording.",
                        "OpenGoVideo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_meeting.HasChanged)
            {
                if (_meeting.HasChanged)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show(
                        "The agenda file changes if you close without saving you will lose them.  Do you want to Continue?",
                        "OpenGoVideo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }

            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

            var loginView = _regionManager.Regions[Infrastructure.Models.Regions.Main].GetView("LoginView");

            if (loginView == null)
            {
                loginView = new OGV2P.Admin.Views.LoginView(_container,
                    _container.Resolve<ISession>(), _container.Resolve<IUser>());
                _regionManager.Regions[Infrastructure.Models.Regions.Main].Add(loginView, "LoginView");
            }

            _regionManager.Regions[Infrastructure.Models.Regions.Main].Activate(loginView);

            foreach (var view in _regionManager.Regions[Infrastructure.Models.Regions.Main].Views)
            {
                if (!(view is LoginView))
                    _regionManager.Regions[Infrastructure.Models.Regions.Main].Remove(view);
            }

            var user = _container.Resolve<IUser>();
            _meeting = new Meeting(session, user);

            session.LogOut();
        }

        internal static void ProcessArg(string arg)
        {
            throw new NotImplementedException();
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
            _meeting = _container.Resolve<IMeeting>();
            if (_meeting.IsBusy || _meeting.HasChanged)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == OGV2P.WpfSingleInstanceByEventWaitHandle.WpfSingleInstance.NativeMethods.WM_SHOWME)
            {
                
            }
            return IntPtr.Zero;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            foreach (InputBinding inputBinding in this.InputBindings)
            {
                
                KeyGesture keyGesture = inputBinding.Gesture as KeyGesture;
                System.Diagnostics.Debug.WriteLine("Key {0} with Modifier {1} pushed", e.Key.ToString(), keyGesture.Modifiers);
                if (keyGesture != null && keyGesture.Key == e.Key && keyGesture.Modifiers == Keyboard.Modifiers)
                {
                    var currentMeeting = _container.Resolve<IMeeting>();
                    currentMeeting.ShowSettings = !currentMeeting.ShowSettings;
                    
                }
            }
        }
    }
}