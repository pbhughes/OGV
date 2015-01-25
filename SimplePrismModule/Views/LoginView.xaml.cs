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
using OGV.Admin.Models;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;


namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for LoginModule.xaml
    /// </summary>
    public partial class LoginView : UserControl, INavigationAware
    {

        
        IUnityContainer _container;
       

        public LoginView(IUnityContainer container, IUserViewModel userModel)
        {
            InitializeComponent();
            _container = container;
            this.DataContext = userModel;
           
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
            (this.DataContext as UserViewModel).UserName = string.Empty;
        }

        
    }
}
