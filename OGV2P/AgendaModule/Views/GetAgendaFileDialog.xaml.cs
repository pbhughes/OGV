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
using System.Windows.Shapes;
using Infrastructure.Models;
using Infrastructure.Interfaces;
using Infrastructure.AgendaService;
using System.ComponentModel;

namespace OGV2P.AgendaModule.Views
{
    
    /// <summary>
    /// Interaction logic for GetAgendaFileDialog.xaml
    /// </summary>
    public partial class GetAgendaFileDialog : Window, INotifyPropertyChanged
    {
        public string AgendaXml { get; set; }
        
        private IAgendaSelector _selector;
        public GetAgendaFileDialog( IAgendaSelector selector)
        {
            InitializeComponent();
            _selector = selector;
            DataContext = _selector;
          
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
