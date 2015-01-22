﻿using System;
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


namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for LoginModule.xaml
    /// </summary>
    public partial class LoginView : UserControl, INavigationAware
    {
        User _user;

        public LoginView()
        {
            InitializeComponent();
            SetUser();
           
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
            SetUser();
        }

        private void SetUser()
        {
            _user = ServiceLocator.Current.GetInstance<User>() as User;
            this.DataContext = _user;
        }
    }
}
