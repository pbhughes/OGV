using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OGV2P;

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
            }
         
             
        }

        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("A startup execption occured here is the error: {0}", ((Exception) e.ExceptionObject).Message));

        }
    }
}
