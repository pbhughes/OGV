using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OGV2P;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using forms = System.Windows.Forms;
using Infrastructure.Extensions;

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
                base.OnStartup(e);
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
            if(e.ExceptionObject is InvalidComObjectException)
            {
                //skip the screen show
            }
            else
            {
                MessageBox.Show(string.Format("A startup execption occured here is the error: {0}", ((Exception)e.ExceptionObject).Message));
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
                        //TODO: Push it to Clerkbase
                        File.Delete(fileName);
                        File.WriteAllText(fileName, sb.ToString());
                        return;
                    }
                    
                }
                File.AppendAllText(fileName, sb.ToString());
            }
            catch (Exception exLocal)
            {
                MessageBox.Show("Could not write the exception log");

            }


        }
            
            
    }
}
