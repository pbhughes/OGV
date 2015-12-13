using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ToggleSwtich : UserControl, INotifyPropertyChanged
    {
        public ToggleSwtich()
        {
            InitializeComponent();
          
        }

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(ToggleSwtich), new PropertyMetadata(false, new PropertyChangedCallback( OnStatePropertyChanged )));

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var theToggle = (ToggleSwtich)d;
            if (((bool)e.NewValue))
                theToggle.ball.SetValue(Grid.ColumnProperty, 0);
            else
                theToggle.ball.SetValue(Grid.ColumnProperty, 1);
        }

       
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
