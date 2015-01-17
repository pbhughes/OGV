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
using System.Threading.Tasks;
using OGV.Admin.Models;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for ChooseBoardView.xaml
    /// </summary>
    public partial class ChooseBoardView : UserControl, INavigationAware
    {
        IRegionManager _rm;

        public ChooseBoardView(IRegionManager rm)
        {
            InitializeComponent();
            _rm = rm;
        }




        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            var view = _rm.Regions["NavBarRegion"].Views.FirstOrDefault();
            if (view != null)
                _rm.Regions["NavBarRegion"].Remove(view);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _rm.RegisterViewWithRegion("NavBarRegion", typeof(OGV.Admin.Views.ChangeBoardNavView));
        }
    }
}
