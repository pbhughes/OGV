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

                MessageBox.Show(string.Format("A startup execption occured here is the error: {0}", ex.Message));
                WriteExceptionToFile(ex);
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
                string fileName = "Exception_Log";
                string excepitonText = ex.ToString();
                string stackTrace = ex.StackTrace;
                string textToWrite = string.Format("\n {0} \n {1} \n", excepitonText, stackTrace);
                FileInfo fInfo = new FileInfo(fileName);
                if (fInfo.Length > 10485760)
                    File.Delete(fileName);

                File.AppendAllText(fileName, textToWrite);
            }
            catch (Exception exLocal)
            {
                MessageBox.Show("Could not write the exception log");

            }


        }
            
            
    }
}
