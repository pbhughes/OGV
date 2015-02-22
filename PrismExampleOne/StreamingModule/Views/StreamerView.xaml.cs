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
using OGV.Streaming.Models;
using Microsoft.Expression.Encoder.Live;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Practices.Unity;
using OGV.Infrastructure.Interfaces;

namespace OGV.Streaming.Views
{
    /// <summary>
    /// Interaction logic for StreamerView.xaml
    /// </summary>
    public partial class StreamerView : UserControl, INavigationAware
    {
        LiveEncodingSource _encoder;
        IUnityContainer _container;
        IUserViewModel _userModel;


        public StreamerView(IUserViewModel user)
        {
            InitializeComponent();
            _encoder = new LiveEncodingSource(user);
            DataContext = _encoder;
            _userModel = user;
            
            _encoder.LoadCompletedEvent += encoder_LoadCompletedEvent;
            //_encoder.PreconnectPublishingPoint();
        }


        #region Fields, Delegates, and Events

     
        private delegate void OneArgDelegate(string message);

        private delegate void TwoArgDelegate(long frameCount, long droppedFrameCount);

        private Boolean _loadComplete = false;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the preconnect button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdPreconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        private void cmdStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

              
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// After the constructor is called the loaded function will handle
        /// reading cached settings file and setting up the devices.  It will pick
        /// the first device in the list and use it to setup the source if no cache file
        /// exists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //read cached settings and pull the device names from cache and
            //add thos devices as a live source and configure the preview window
            //on that source
      
            //set the device source  TODO: send in the devices in an overload
         

      
        }

        private void cboVideoDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadComplete)
            {
               
            }
        
        }

        private void cboAudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadComplete)
            {
                
            }
        }

        //Resized the preview window when the form gets resized.  This function
        //is called multiple time when the form builds so it will be called before
        //a device source is configured so we have to not try to set the
        //preview window until a device source exists
        private void pnlPreview_Resize(object sender, EventArgs e)
        {
            //grab the active source and reassign the newly sized
            //preview window  Devsource[0] zero represents the one
            //and only configured device source, we will not
            //support multiple simultaneous devices.
           
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                
            }
        }

        private void expWebSites_Expanded(object sender, RoutedEventArgs e)
        {
            winHostInputPreview.Visibility = System.Windows.Visibility.Hidden;
        }

        private void expWebSites_Collapsed(object sender, RoutedEventArgs e)
        {
            winHostInputPreview.Visibility = System.Windows.Visibility.Visible;
        }

        private void expWebSites_MouseLeave(object sender, MouseEventArgs e)
        {
           
        }
        #endregion

        #region Functions and Methods

       
        private void SetupPreviewWindow(){

            try
            {
                LiveDeviceSource liv = _encoder.FeedSource;

                liv.ResizeMode = Microsoft.Expression.Encoder.VideoResizeMode.Letterbox;
                System.Drawing.Size viewSize = new System.Drawing.Size(400,400);
                liv.PreviewWindow = _encoder.SetInputPreviewWindow(viewSize, pnlPreview);

                _loadComplete = true;
               
            }
            catch (Exception ex)
            {

              
            }
          
        }
        #endregion

        private void encoder_LoadCompletedEvent(object sender, EventArgs e)
        {
            SetupPreviewWindow();
        }



        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
           
        }
    }
}
