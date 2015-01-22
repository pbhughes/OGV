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
using Microsoft.Practices.ServiceLocation;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for ChooseBoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl, INavigationAware
    {
        IRegionManager _rm;
   

        public BoardView(IRegionManager rm)
        {
            InitializeComponent();
            _rm = rm;
            SetBoard();
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
            SetBoard();
        }

        private void SetBoard()
        {

            this.DataContext = ServiceLocator.Current.GetInstance<BoardList>() as BoardList;
        }
    }
}
