using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions;
using OGV.Admin.Views;
using OGV.Streaming.Views;
using OGV.Infrastructure.Services;
using OGV.Admin.Models;

namespace PrismExampleOne
{
    class Bootstrapper: UnityBootstrapper
    {
        private XService _xService = new XService();
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

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(this.Container));

            
            //RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);

            this.Container.RegisterInstance<CallbackLogger>(this.callbackLogger);

            this.Container.RegisterType(typeof(IRegionNavigationService),
                typeof(Microsoft.Practices.Prism.Regions.RegionNavigationService));

            this.Container.RegisterType<object, SimpleView>(typeof(SimpleView).FullName);
            this.Container.RegisterType<object, LoginView>(typeof(LoginView).FullName);
            this.Container.RegisterType<object, StreamerView>(typeof(StreamerView).FullName);
            this.Container.RegisterType<object, BoardView>(typeof(BoardView).FullName);
            this.Container.RegisterType<object, AgendaView>(typeof(AgendaView).FullName);
            this.Container.RegisterType<object, AgendaNavView>(typeof(AgendaNavView).FullName);
            this.Container.RegisterType<object, BoardNavView>(typeof(BoardNavView).FullName);
            //this.Container.RegisterInstance<BoardList>(new BoardList(this.Container));
            this.Container.RegisterInstance<User>(new User(this.Container));
            this.Container.RegisterInstance<IXService>(_xService);
             
           
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
            
        }

        public CallbackLogger callbackLogger { get { return _callBackLogger; } }
    }
}
