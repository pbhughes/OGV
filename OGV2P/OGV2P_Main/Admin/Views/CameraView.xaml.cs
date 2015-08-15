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
using DirectShowLib;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        

        public CameraView()
        {
            InitializeComponent();
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DsDevice[] devices;
            DsDevice [] capDevices;
            var audioDevices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat( FilterCategory.VideoInputDevice );

          
        }
    }
}
