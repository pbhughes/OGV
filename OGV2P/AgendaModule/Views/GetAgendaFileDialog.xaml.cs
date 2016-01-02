using System.Windows;
using System.Windows.Input;
using Infrastructure.Interfaces;
using Infrastructure.AgendaService;
using System.ComponentModel;
using Infrastructure.Converters;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;
using System.IO;

namespace OGV2P.AgendaModule.Views
{

    /// <summary>
    /// Interaction logic for GetAgendaFileDialog.xaml
    /// </summary>
    public partial class GetAgendaFileDialog : Window, INotifyPropertyChanged
    {
        public string AgendaXml { get; set; }
        public string FilePath { get; set; }
        
        private static IAgendaSelector _selector;
        public GetAgendaFileDialog(IUser user)
        {
            InitializeComponent();
            this.DataContext = _selector;
        }

        public static async Task< GetAgendaFileDialog > Create(IUser user)
        {
            _selector = AgendaSelector.Create(user).Result;
           
           
            try
            {
                await _selector.LoadAgendaFiles();
                if (_selector.LastError != null)
                    Xceed.Wpf.Toolkit.MessageBox.Show(_selector.LastError.Message);

                return new GetAgendaFileDialog(user);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        private void agendaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetFileAndClose();

        }

        private void SetFileAndClose()
        {
            string fileName = ((AgendaFile)agendaList.SelectedItem).FileName;

            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas");
            if (!File.Exists(dir))
                Directory.CreateDirectory(dir);

            AgendaXml = _selector.GetXml(fileName);

            FilePath = Path.Combine(dir, fileName);
                        
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
            Close();
        }
    }
}
