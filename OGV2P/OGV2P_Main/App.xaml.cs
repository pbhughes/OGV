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
            base.OnStartup(e);
            OGV2P.BootStrapper bootStrapper = new BootStrapper();
            bootStrapper.Run();
             
        }
    }
}
