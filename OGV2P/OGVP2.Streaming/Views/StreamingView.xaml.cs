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
using Microsoft.Practices.Unity;
using Infrastructure.Interfaces;

namespace OGV2P.Streaming.Views
{
    /// <summary>
    /// Interaction logic for StreamingView.xaml
    /// </summary>
    public partial class StreamingView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private ISession _session;
        private IUser _user;

        public StreamingView(IUnityContainer container, ISession session, IUser user)
        {
            InitializeComponent();
            _session = session;
            _user = user;
        }

        public bool KeepAlive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}
