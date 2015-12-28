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

namespace CustomControls.Views
{
    /// <summary>
    /// Interaction logic for TimerDisplay.xaml
    /// </summary>
    public partial class TimerDisplay : UserControl
    {

        public bool Recording
        {
            get { return (bool)GetValue(RecordingProperty); }
            set { SetValue(RecordingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Recording.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecordingProperty =
            DependencyProperty.Register("Recording", typeof(bool), typeof(TimerDisplay), 
                new PropertyMetadata(false, new PropertyChangedCallback(OnRecordingPropertyChanged)));



        private static void OnRecordingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimerDisplay display = (TimerDisplay)d;
            if ((bool)e.NewValue == true)
                display.Background = new SolidColorBrush(Colors.Green);
            else
                display.Background = new SolidColorBrush(Colors.Gray);


        }

        public TimeSpan TimerValue
        {
            get { return (TimeSpan)GetValue(TimerValueProperty); }
            set { SetValue(TimerValueProperty, value); }
        }

       

        // Using a DependencyProperty as the backing store for TimerValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimerValueProperty =
            DependencyProperty.Register("TimerValue", typeof(TimeSpan), typeof(TimerDisplay), 
                new PropertyMetadata( new TimeSpan(0,0,0), new PropertyChangedCallback(OnStatePropertyChanged)));


        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimerDisplay display = (TimerDisplay)d;
            TimeSpan newVal = (TimeSpan)e.NewValue;
            display.txtHours.Text = newVal.Hours.ToString().PadLeft(2, '0');
            display.txtMinutes.Text = newVal.Minutes.ToString().PadLeft(2, '0');
            display.txtSeconds.Text = newVal.Seconds.ToString().PadLeft(2, '0');

        }

        public TimerDisplay()
        {
            InitializeComponent();
           
        }
    }
}
