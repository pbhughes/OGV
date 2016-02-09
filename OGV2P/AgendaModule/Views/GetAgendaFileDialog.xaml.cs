using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using OGV2P.FTP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            DataContext = _selector;
        }

        private async void agendaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _selector.IsBusy = true;
            try
            {
                if (agendaList.SelectedItem != null)
                {
                    await Task.Run(() =>
                    {
                        SetFileAndClose();
                    });
                }
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();

                msgBox.Text = ex.Message;
                msgBox.Caption = "Error getting the agenda XML";
                throw;
            }
            finally
            {
                _selector.IsBusy = false;
                this.DialogResult = true;
                this.Close();
            }
        }

        private  void SetFileAndClose()
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    string fileName = ((FTPfileInfo)agendaList.SelectedItem).Filename;

                    string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    AgendaXml = _selector.GetSelectedAgendaXML(fileName);
                    FilePath = _selector.TargetFile;
                });
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception )
            {

                throw;
            }
            
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _selector.IsBusy = true;
            try
            {
                if (agendaList.SelectedItem != null)
                {
                    await Task.Run(() =>
                    {
                        SetFileAndClose();
                    });
                }
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();

                msgBox.Text = ex.Message;
                msgBox.Caption = "Error getting the agenda XML";
                throw;
            }
            finally
            {
                _selector.IsBusy = false;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _selector.IsBusy = true;

            try
            {
                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    List<FTPfileInfo> files = _selector.ListAgendaFilesOnServer();
                    Dispatcher.Invoke(() =>
                   {
                       foreach (FTPfileInfo fi in files)
                       {
                           agendaList.Items.Add(fi);
                       }
                       if (_selector.LastError != null)
                           Xceed.Wpf.Toolkit.MessageBox.Show(_selector.LastError.Message);
                   });

                    System.Threading.Thread.Sleep(50);
                });
            }
            catch (Exception ex)
            {
                var msgBox = new Xceed.Wpf.Toolkit.MessageBox();
                msgBox.Text = ex.Message;
                msgBox.Caption = "Error getting the file list";
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