using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;

namespace OGV2P
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            OGV2P.App.Current.MainWindow = (Window)this.Shell;
            OGV2P.App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(OGV2P.Admin.AdminModule));

            moduleCatalog.AddModule(typeof(OGV2P.Agenda.AgendaModule));

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterType<object, OGV2P.Admin.Views.CameraView>(typeof(OGV2P.Admin.Views.CameraView).FullName);
            this.Container.RegisterType<object, OGV2P.Admin.Views.LoginView>(typeof(OGV2P.Admin.Views.LoginView).FullName);

        }
    }
}
