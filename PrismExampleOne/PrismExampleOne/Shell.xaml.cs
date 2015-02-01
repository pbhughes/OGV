using OGV.Admin.Models;
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
using OGV.Infrastructure.Interfaces;

namespace PrismExampleOne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        IUserViewModel _user;
        public Shell(IUserViewModel user)
        {
            InitializeComponent();
            _user = user;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_user != null)
                if (_user.BoardList != null)
                    if (_user.BoardList.SelectedAgenda != null)
                        if (_user.BoardList.SelectedAgenda.SaveNeeded)
                        {
                            MessageBoxResult result = MessageBox.Show("You have unsaved changes, do you want to save them?", "OGV2", MessageBoxButton.YesNoCancel);
                            switch (result)
                            {
                                case MessageBoxResult.Cancel:
                                    e.Cancel = true;
                                    break;
                                case MessageBoxResult.Yes:
                                    _user.BoardList.SaveAgenda();
                                    break;
                                case MessageBoxResult.No:
                                    break;
                            }
   
                        }
        }
    }
}
