using Infrastructure.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using forms = System.Windows.Forms;
using OGV2P.WpfSingleInstanceByEventWaitHandle;


namespace OGV2P
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {

                AppDomain current = AppDomain.CurrentDomain;
                current.UnhandledException += Current_UnhandledException;

                WpfSingleInstance.Make("424dbbdd-3c99-4390-81a0-f1efb384e1bd", this);

           

            
                base.OnStartup(e);
                EventManager.RegisterClassHandler(typeof(DatePicker),
                       DatePicker.LoadedEvent,
                       new RoutedEventHandler(DatePicker_Loaded));

                OGV2P.BootStrapper bootStrapper = new BootStrapper();
                bootStrapper.Run();
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                MessageBox.Show(string.Format("A startup exception occurred here is the error: {0}", ex.Message));
            }
        }

        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteExceptionToFile((Exception)e.ExceptionObject);
            if (e.ExceptionObject is InvalidComObjectException)
            {
                //skip the screen show
            }
            else
            {
                MessageBox.Show(string.Format("A startup exception occurred here is the error: {0}", ((Exception)e.ExceptionObject).Message));
            }
        }

        public void WriteExceptionToFile(Exception ex)
        {
            try
            {
                string path = forms.Application.LocalUserAppDataPath;
                string exceptionFileName = "Exception_Log.txt";
                string fileName = Path.Combine(path, exceptionFileName);
                string excepitonText = ex.ToString();
                string stackTrace = ex.StackTrace;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("*****************************************************");
                sb.AppendLine(DateTime.Now.ToString());
                sb.AppendLine("                       ");
                sb.AppendLine(excepitonText);
                sb.AppendLine("                       ");
                sb.AppendLine(stackTrace);
                sb.AppendLine("                       ");
                sb.AppendLine("*****************************************************");

                if (File.Exists(fileName))
                {
                    FileInfo fInfo = new FileInfo(fileName);
                    if (fInfo.Length > 10485760)
                    {
                        //TODO: Push it to ClerkBase
                        File.Delete(fileName);
                        File.WriteAllText(fileName, sb.ToString());
                        return;
                    }
                }
                File.AppendAllText(fileName, sb.ToString());
            }
            catch (Exception exLocal)
            {
                exLocal.WriteToLogFile();
                MessageBox.Show("Could not write the exception log");
            }
        }

        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            var dp = sender as DatePicker;
            if (dp == null) return;

            var tb = GetChildOfType<DatePickerTextBox>(dp);
            if (tb == null) return;

            var wm = tb.Template.FindName("PART_Watermark", tb) as ContentControl;
            if (wm == null) return;

            wm.Content = "";
        }
    }
}