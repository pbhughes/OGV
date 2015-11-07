
namespace OGV2P.Admin
{
    using Infrastructure.Interfaces;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Unity;

    public class AdminModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;
        private IRegionManager _regionManager;
        private IUnityContainer _container;
        private ISession _session;
        private IUser _user;

        public AdminModule(IRegionViewRegistry registry, IRegionManager regionManager, IUnityContainer container, ISession session, IUser user)
        {
            _regionViewRegistry = registry;
            _regionManager = regionManager;
            _container = container;
            _session = session;
            _user = user;
        }

        public void Initialize()
        {
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Add(new OGV2P.Admin.Views.LoginView(_container, _session, _user), "LoginView");
            //_regionViewRegistry.RegisterViewWithRegion(Infrastructure.Models.Regions.Middle, typeof(Views.LoginView));
        }
    }
}
