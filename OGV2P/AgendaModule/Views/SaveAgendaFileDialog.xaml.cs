using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Extensions;
using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for SaveAgendaFileDialog.xaml
    /// </summary>
    public partial class SaveAgendaFileDialog : Window, INotifyPropertyChanged
    {
        private IUnityContainer _container;
        private ISaveAgendaViewModel _saveAgendaViewModel;

        public SaveAgendaFileDialog(IUnityContainer container)
        {
            InitializeComponent();
            _container = container;
            _saveAgendaViewModel = (SaveAgendaViewModel)_container.Resolve(typeof(ISaveAgendaViewModel), new ResolverOverride[] { });
            DataContext = _saveAgendaViewModel;
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            await PostTheFile();
        }

        private async Task PostTheFile()
        {
            _saveAgendaViewModel.IsBusy = true;
            try
            {
                await Task.Run(() =>
                {
                    _saveAgendaViewModel.SaveAgenda();
                });
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show(
                    string.Format("Error trying to save the agenda file ensure your board is setup correctly on the server.  Error Text: {0}",
                    ex.Message));
            }
            finally
            {
                _saveAgendaViewModel.IsBusy = false;
                Close();
            }
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }


        #endregion

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await PostTheFile();
        }
    }
}
