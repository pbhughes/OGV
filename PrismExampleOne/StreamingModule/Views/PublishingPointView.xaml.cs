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
    public partial class PublishingPointView : UserControl
    {
        LiveEncodingSource _encoder;
        IUnityContainer _container;
        IUserViewModel _userModel;

        public PublishingPointView(IUserViewModel user)
        {
            InitializeComponent();
            _userModel = user;
            this.DataContext = _userModel.BoardList;
        }

      
    }
}
