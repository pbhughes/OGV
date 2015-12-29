using System.Windows;
using System.Windows.Input;
using Infrastructure.Interfaces;
using Infrastructure.AgendaService;
using System.ComponentModel;
using Infrastructure.Converters;
using Infrastructure.Models;
using System.Threading.Tasks;

namespace OGV2P.AgendaModule.Views
{

    /// <summary>
    /// Interaction logic for GetAgendaFileDialog.xaml
    /// </summary>
    public partial class GetAgendaFileDialog : Window, INotifyPropertyChanged
    {
        public string AgendaXml { get; set; }
        
        private IAgendaSelector _selector;
        public GetAgendaFileDialog(IUser user)
        {
            InitializeComponent();
            _selector = AgendaSelector.Create(user).Result;
            DataContext = _selector;
            _selector.LoadAgendaFiles();
          
        }

        private void agendaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetFileAndClose();

        }

        private void SetFileAndClose()
        {
            string fileName = ((AgendaFile)agendaList.SelectedItem).FileName;
            AgendaXml = _selector.GetXml(fileName);
            this.DialogResult = true;
            this.Close();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (agendaList.SelectedItem != null)
                SetFileAndClose();
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
