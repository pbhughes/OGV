using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OGV.Infrastructure.Interfaces;
using OGV.Streaming.Models;
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

namespace OGV.Streaming.Views
{
    /// <summary>
    /// Interaction logic for PublishingPointView.xaml
    /// </summary>
    public partial class PublishingPointView : UserControl, INavigationAware
    {
        LiveEncodingSource _encoder;
        IUnityContainer _container;
        IUserViewModel _userModel;
        IPublishingPointMonitor _pubMonitor;
        IRegionManager _regionManager;

        public PublishingPointView(IUserViewModel user, IPublishingPointMonitor pubMonitor, IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _userModel = user;
            _pubMonitor = pubMonitor;
            pnlPointMonitor.DataContext = _pubMonitor;
            this.DataContext = _userModel.BoardList;
            _userModel.BoardList.AgendaSelectedEvent += BoardList_AgendaSelectedEvent;
        }

        void BoardList_AgendaSelectedEvent(IAgenda selected)

        {

            ResetButtons();
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

        private void ResetButtons()
        {
            if (_pubMonitor != null)
                _pubMonitor.CheckStateCommand.RaiseCanExecuteChanged();
        }
    }
}
