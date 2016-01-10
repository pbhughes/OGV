using System.Windows;
using System.Windows.Input;
using Infrastructure.Interfaces;
using System.ComponentModel;
using Infrastructure.Converters;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;
using System.IO;
using Infrastructure.Extensions;
using OGV2P.FTP.Utilities;


namespace OGV2P.AgendaModule.Views
{

    /// <summary>
    /// Interaction logic for GetAgendaFileDialog.xaml
    /// </summary>
    public partial class GetAgendaFileDialog : Window, INotifyPropertyChanged
    {
        public string AgendaXml { get; set; }
        public string FilePath { get; set; }

        private IAgendaSelector _selector;

        public GetAgendaFileDialog(IUser user)
        {
            InitializeComponent();
            _selector = new AgendaSelector(user);
            this.DataContext = _selector;
        }

       
        private void agendaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (agendaList.SelectedItem != null)
                    SetFileAndClose();
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();

                msgBox.Text = ex.Message;
                msgBox.Caption = "Error getting the agenda";
                throw;
            }
           

        }

        private void SetFileAndClose()
        {
            string fileName = ((FTPfileInfo) agendaList.SelectedItem).Filename;

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
            try
            {
                if (agendaList.SelectedItem != null)
                    SetFileAndClose();
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();

                msgBox.Text = ex.Message;
                msgBox.Caption = "Error getting the agenda";
                throw;
            }
          
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _selector.IsBusy = true;
                await _selector.LoadAgendaFiles();
                if (_selector.LastError != null)
                    Xceed.Wpf.Toolkit.MessageBox.Show(_selector.LastError.Message);
            }
            catch (Exception ex)
            {
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();
                msgBox.Content = ex.Message;
                msgBox.ShowDialog();
                
                ex.WriteToLogFile();
            }
            finally
            {
                _selector.IsBusy = false;
            }
        }
    }
}
