using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace OGV2P.WpfSingleInstanceByEventWaitHandle
{
    public static class WpfSingleInstance
    {
        internal static void Make(String name, Application app)
        {
            EventWaitHandle eventWaitHandle = null;
            String eventName = Environment.MachineName + "-" + name;

            bool isFirstInstance = false;

            try
            {
                eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
            }
            catch
            {
                // it's first instance
                isFirstInstance = true;
            }

            if (isFirstInstance)
            {
                eventWaitHandle = new EventWaitHandle(
                    false,
                    EventResetMode.AutoReset,
                    eventName);

                ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, waitOrTimerCallback, app, Timeout.Infinite, false);

                // not need more
                eventWaitHandle.Close();

              
            }
            else
            {
              

                eventWaitHandle.Set();

                NativeMethods.PostMessage(
                           (IntPtr)NativeMethods.HWND_BROADCAST,
                           NativeMethods.WM_SHOWME,
                           IntPtr.Zero,
                           IntPtr.Zero);

                // For that exit no interceptions
                Environment.Exit(0);
            }
        }

        private delegate void dispatcherInvoker();

        private static void waitOrTimerCallback(Object state, Boolean timedOut)
        {
            Application app = (Application)state;
            app.Dispatcher.BeginInvoke(
                    new dispatcherInvoker(delegate ()
                    {
                        Application.Current.MainWindow.Topmost = true;
                        Application.Current.MainWindow.Activate();
                        

                       
                    }),
                    null
                );
        }

        // Args functionality for test purpose and not developed carefully

        #region Args

        internal static readonly object StartArgKey = "StartArg";

        private static readonly String isolatedStorageFileName = "SomeFileInTheRoot.txt";

        private static void setArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (1 < args.Length)
            {
                IsolatedStorageFile isoStore =
                    IsolatedStorageFile.GetStore(
                        IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                        null,
                        null);

                IsolatedStorageFileStream isoStream1 = new IsolatedStorageFileStream(isolatedStorageFileName, FileMode.Create, isoStore);
                StreamWriter sw = new StreamWriter(isoStream1);
                string arg = args[1];
                sw.Write(arg);
                sw.Close();
            }
        }

        private static void setFirstArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (1 < args.Length)
            {
                Application.Current.Resources[WpfSingleInstance.StartArgKey] = args[1];
            }
        }

        private static void processArgs()
        {
            IsolatedStorageFile isoStore =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                    null,
                    null);

            IsolatedStorageFileStream isoStream1 = new IsolatedStorageFileStream(isolatedStorageFileName, FileMode.OpenOrCreate, isoStore);
            StreamReader sr = new StreamReader(isoStream1);
            string arg = sr.ReadToEnd();
            sr.Close();

            isoStore.DeleteFile(isolatedStorageFileName);

            OGV2P.Shell.ProcessArg(arg);
        }

        #endregion Args

        internal class NativeMethods
        {
            public const int HWND_BROADCAST = 0xffff;
            public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImport("user32")]
            public static extern int RegisterWindowMessage(string message);
        }
    }
}