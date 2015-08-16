using DirectX.Capture;
using System.Windows;
using System.Windows.Controls;

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
            Filters filters = new Filters();

            Capture capture = new Capture(
                filters.VideoInputDevices[0],
                filters.AudioInputDevices[0]
            );

            capture.PreviewWindow = videoPanel;

            capture.Start();
        }
    }
}