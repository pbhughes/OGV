using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
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
using OGV.Admin.Models;
using OGV.Infrastructure.Services;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for BoardViewNav.xaml
    /// </summary>
    public partial class BoardNavView : UserControl, INavigationAware
    {
        XService _xService;

        public BoardNavView()
        {
            InitializeComponent();
            this.DataContext = ServiceLocator.Current.GetInstance<BoardList>();
            
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
            this.DataContext = ServiceLocator.Current.GetInstance<BoardList>();
        }
    }
}
