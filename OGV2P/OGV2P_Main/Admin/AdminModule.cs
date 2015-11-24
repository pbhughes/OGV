﻿
namespace OGV2P.Admin
{
    using Infrastructure.Interfaces;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Unity;
    using System.Windows.Forms;

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
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Add(new Views.LoginView(_container, _session, _user), "LoginView");
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Add(new Views.ServicesView(_regionManager, null, _user, _session,_container), "SettingsView");

            var v = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("LoginView");
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Activate( v );
        }
    }
}
