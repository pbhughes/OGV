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
using OGV2P.Admin.Models;
using System.ServiceProcess;

namespace OGV2P.Admin.Views
{
    /// <summary>
    /// Interaction logic for ServiceView.xaml
    /// </summary>
    public partial class ServiceView : UserControl
    {
        PanoptoService _service;

        public ServiceView()
        {
            this.Loaded += ServiceView_Loaded;
            InitializeComponent();
        }

        void ServiceView_Loaded(object sender, RoutedEventArgs e)
        {
            _service = new PanoptoService(ServiceName);
            this.DataContext = _service;
        }

        public string ServiceName
        {
            get { return (string)GetValue(ServiceNameProperty); }
            set { SetValue(ServiceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServiceName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceNameProperty =
            DependencyProperty.Register("ServiceName", typeof(string), typeof(ServiceView), null);



        public ServiceControllerStatus Status
        {
            get { return (ServiceControllerStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ServiceControllerStatus), typeof(ServiceView), null);

        
        
    }
}
