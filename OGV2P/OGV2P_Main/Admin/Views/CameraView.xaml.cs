using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using System.Timers;
using System;
using System.Windows.Media;
using AxRTMPActiveX;
using RTMPActiveX;
using System.Diagnostics;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        
        ISession _sessionService;

        private System.Timers.Timer cpuReadingTimer;
        private PerformanceCounter cpuCounter;

        LinearGradientBrush _yellow = 
            new LinearGradientBrush(Colors.Green, Colors.Yellow, 
                new Point(0, 1), new Point(1, 0));

        public bool KeepAlive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            try
            {
                vuMeter.Minimum = 0;
                vuMeter.Maximum = 100;
                vuMeter.Foreground = _yellow;


                _sessionService.LocalVideoFile = "";
          
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to switch to devices  video: {0} audio: {1}", "Video Name", "audio Name"));
            }
        }

     
        private void UpdateVUMeter(float sampleVolume)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                vuMeter.Value = (double)sampleVolume;
                //System.Diagnostics.Debug.WriteLine(string.Format("Volume {0} ", vuMeter.Value));
            });
        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCapture();
        }


        public CameraView()
        {
           
        }

        public CameraView(IDevices devices, ISession sessionService)
        {
            InitializeComponent();

            try
            {
                axRControl.License = "nlic:1.2:LiveEnc:3.0:LvApp=1,LivePlg=1,MSDK=4,MPEG2DEC=1,MPEG2ENC=1,PS=1,TS=1,H264DEC=1,H264ENC=1,H264ENCQS=1,MP4=4,RTMPsrc=1,RtmpMsg=1,RTMPs=1,RTSP=1,RTSPsrc=1,UDP=1,UDPsrc=1,HLS=1,WMS=1,WMV=1,RTMPm=4,RTMPx=3,Resz=1,RSrv=1,VMix2=1,3DRemix=1,ScCap=1,AuCap=1,AEC=1,Demo=1,Ic=1,NoMsg=1,Tm=1800,T1=600,NoIc=1:win,win64,osx:20151030,20160111::0:0:nanocosmosdemo-292490-3:ncpt:f6044ea043c479af5911e60502f1a334";
                axRControl.InitEncoder();

                // Video Source Device (0...n)
                axRControl.VideoSource = 0;

                // Device/Camera Resolution
                axRControl.VideoWidth = 320;
                axRControl.VideoHeight = 240;
                axRControl.VideoFrameRate = 25;

                // Video Encoder Bitrate (Bits/s)
                axRControl.VideoBitrate = 500000;

                axRControl.TextOverlayText = "Hello World!";
                //axRTMPActiveX1.TextOverlayText = "c:\\temp\\icon.png";
                axRControl.VideoEffect = 3;

                axRControl.StartPreview();

          

            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        private void cpuReadingTimer_Elapsed(object p1, object p2)
        {
            // get the CPU reading
            float cpuUtilization = cpuCounter.NextValue();
            lblCpu.Content = "CPU: " + cpuUtilization + "%";
        }

        void axRTMPActiveX1_OnStop(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnStopEvent e)
        {
            //throw new NotImplementedException();
            lblStatus.Content = "Stopped.";
            MessageBox.Show(e.ToString() + " - " + e.result);
        }

        void axRTMPActiveX1_OnEvent(object sender, AxRTMPActiveX.IRTMPActiveXEvents_OnEventEvent e)
        {
            if (e.type == "10")
                lblStatus.Content = "Running...";
            else
                lblStatus.Content = "Event Status " + e.type + " Text: " + e.result;

            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}