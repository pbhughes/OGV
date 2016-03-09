using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindowDialog : Window
    {

        public SettingWindowDialog(string previewUrl, string localVideoFile, string publishingPoint)
        {
            InitializeComponent();
            navUrl.NavigateUri = new Uri((string.IsNullOrEmpty(previewUrl) ? "http://www.opengovideo.com" : previewUrl));
            txtNavUrlContainer.Text = (string.IsNullOrEmpty(previewUrl) ? " www.opengovideo.com/ogv2help" : previewUrl);
            txtLocalFile.Text = (localVideoFile == null || localVideoFile == string.Empty)?"The folder where the local video is stored....":localVideoFile;
            txtAppVersion.Text = SoftwareVersion();
            txtPublishingPoint.Text = (publishingPoint == null || publishingPoint == string.Empty)?"The url on the internet where the video is sent....":publishingPoint;

           
                
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }


        private string SoftwareVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
                return "Debug";
        }

        private void OpenFileCmd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLocalFile.Text))
            {
                
                var dInfo = Directory.GetParent(txtLocalFile.Text);
                if (Directory.Exists(dInfo.FullName))
                {
                    
                    System.Diagnostics.Process.Start(dInfo.FullName);
                }
                else
                {
                    MessageBox.Show("Invalid directory, this value is loaded from the agenda file.");
                }
            }
        }

      
    }
}
