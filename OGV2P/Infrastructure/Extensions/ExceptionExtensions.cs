using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using forms = System.Windows.Forms;

namespace Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static void WriteToLogFile(this Exception ex)
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
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception exLocal)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                MessageBox.Show("Could not write the exception log");

            }
        }
    }
}
