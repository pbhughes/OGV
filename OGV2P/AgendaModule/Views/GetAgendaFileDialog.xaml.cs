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


        private  void SetFileAndClose(string target)
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    string fileName = target;

                    string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Agendas");
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
                        SetFileAndClose(_selector.SelectedFile.FullName);
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

        private void agendaList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selector.SelectedFile = (FTPfileInfo)agendaList.SelectedItem;
        }
        private async void agendaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _selector.IsBusy = true;
            try
            {
                _selector.SelectedFile = (FTPfileInfo)agendaList.SelectedItem;

                if (agendaList.SelectedItem != null)
                {
                    System.Diagnostics.Debug.WriteLine(_selector.SelectedFile.FullName + "    " + _selector.SelectedFile.Path);
                    await Task.Run(() =>
                    {
                        SetFileAndClose(_selector.SelectedFile.FullName);
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

    }
}