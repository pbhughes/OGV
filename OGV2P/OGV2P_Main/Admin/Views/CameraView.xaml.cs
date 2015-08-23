using DirectX.Capture;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        private IDevices _devices;

        public IDevices Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }

        public CameraView(IDevices devices)
        {
            InitializeComponent();
            _devices = devices;
            DataContext = Devices;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            Filters filters = new Filters();

            Filter video = null;
            Filter audio = null;

            foreach (Filter v in filters.VideoInputDevices)
            {
                if (v.Name == cboCameras.SelectedValue.ToString())
                    video = v;
            }

            foreach (Filter c in filters.VideoInputDevices)
            {
                if (c.Name == cboMicrophones.SelectedValue.ToString())
                    audio = c;
            }

            Capture capture = new Capture(
                video,
                audio
            );

            capture.PreviewWindow = videoPanel;

            capture.Start();
        }

        private void cboMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCapture();
        }
    }
}