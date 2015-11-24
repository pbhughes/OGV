using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
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
        private Window _shell;
        private IMeeting _meeting;

        protected override System.Windows.DependencyObject CreateShell()
        {
            _shell = this.Container.Resolve<Shell>();
            return _shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            OGV2P.App.Current.MainWindow = (Window)Shell;
            _shell = OGV2P.App.Current.MainWindow;
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
            this.Container.RegisterType<object, OGV2P.Admin.Views.ServicesView>(typeof(OGV2P.Admin.Views.ServicesView).FullName);
            this.Container.RegisterType<Infrastructure.Interfaces.IDevices, Infrastructure.Models.Devices>();
            
                    
            _session = new Session();
            _user = new User(_session);
            _meeting = new Meeting(_session);

            _user.RaiseLoginEvent += _user_RaiseLoginEvent;
            this.Container.RegisterInstance<ISession>(_session);
            this.Container.RegisterInstance<IUser>(_user);
            this.Container.RegisterInstance<IMeeting>(_meeting);


        }

        void _user_RaiseLoginEvent(object sender, EventArgs e)
        {
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

            var loginView = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("LoginView");
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Deactivate ( loginView );

            ((Shell)_shell).SetSideBarAllignmentTop();
            
            Uri vv = new Uri(typeof(OGV2P.AgendaModule.Views.AgendaStartView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate(Infrastructure.Models.Regions.Main, vv);

            Uri uu = new Uri(typeof(OGV2P.Admin.Views.CameraView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate(Infrastructure.Models.Regions.SideBar, uu);
        }
    }
}
