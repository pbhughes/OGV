using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions;
using OGV.Admin.Views;
using OGV.Streaming.Views;

namespace PrismExampleOne
{
    class Bootstrapper: UnityBootstrapper
    {

        private CallbackLogger _callBackLogger = new CallbackLogger();

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            moduleCatalog.AddModule(typeof(OGV.Admin.Module));
            moduleCatalog.AddModule(typeof(OGV.Streaming.ModuleStreaming));
            
        }

        protected override Microsoft.Practices.Prism.Regions.RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            return base.ConfigureRegionAdapterMappings();
        }

     
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
            this.Container.RegisterInstance<CallbackLogger>(this.callbackLogger);
            this.Container.RegisterType(typeof(IRegionNavigationService),
                typeof(Microsoft.Practices.Prism.Regions.RegionNavigationService));

            this.Container.RegisterType<object,SimpleView>(typeof(SimpleView).FullName);
            this.Container.RegisterType<object, LoginView>(typeof(LoginView).FullName);
            this.Container.RegisterType<object, StreamerView>(typeof(StreamerView).FullName);
            this.Container.RegisterType<object, ChooseBoardView>(typeof(ChooseBoardView).FullName);
            this.Container.RegisterType<object, AgendaView>(typeof(AgendaView).FullName);
            this.Container.RegisterType<object, NavigationView>(typeof(NavigationView).FullName);
             
           
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }

        public CallbackLogger callbackLogger { get { return _callBackLogger; } }
    }
}
