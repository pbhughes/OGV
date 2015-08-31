using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using OGV2P.AgendaModule.Interfaces;
using OGV2P.AgendaModule.Models;
using Infrastructure.Interfaces;

using Infrastructure.Models;
using Microsoft.Practices.Prism.Regions;

namespace OGV2P
{
    public class BootStrapper : UnityBootstrapper
    {
        private ISession _session;
        private IUser _user;
        private IRegionManager _regionManager;

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

            moduleCatalog.AddModule(typeof(OGV2P.AgendaModule.AgendaModule));

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterType<object, OGV2P.Admin.Views.CameraView>(typeof(OGV2P.Admin.Views.CameraView).FullName);
            this.Container.RegisterType<object, OGV2P.Admin.Views.LoginView>(typeof(OGV2P.Admin.Views.LoginView).FullName);
            this.Container.RegisterType<object, OGV2P.AgendaModule.Views.AgendaStartView>(typeof(OGV2P.AgendaModule.Views.AgendaStartView).FullName);
            this.Container.RegisterType<Infrastructure.Interfaces.IDevices, Infrastructure.Models.Devices>();
            this.Container.RegisterType<OGV2P.AgendaModule.Interfaces.IMeeting, OGV2P.AgendaModule.Models.Meeting>();
            _session = new Session();
            _user = new User(_session);
            _user.RaiseLoginEvent += _user_RaiseLoginEvent;
            this.Container.RegisterInstance<ISession>(_session);
            this.Container.RegisterInstance<IUser>(_user);


        }

        void _user_RaiseLoginEvent(object sender, EventArgs e)
        {
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

            Uri vv = new Uri(typeof(OGV2P.Admin.Views.CameraView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SideBarRegion", vv);

            Uri uu = new Uri(typeof(OGV2P.AgendaModule.Views.AgendaStartView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("MainRegion", uu);
        }
    }
}
