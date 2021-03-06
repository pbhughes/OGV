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
        private IDevices _devices;

        public AdminModule(IRegionViewRegistry registry, IRegionManager regionManager, IUnityContainer container, ISession session, IUser user, IDevices devices)
        {
            _regionViewRegistry = registry;
            _regionManager = regionManager;
            _container = container;
            _session = session;
            _user = user;
            _devices = devices;
        }

        public void Initialize()
        {
            _regionManager.Regions[Infrastructure.Models.Regions.Main].Add(new Views.LoginView(_container, _session, _user), "LoginView");

            var v = _regionManager.Regions[Infrastructure.Models.Regions.Main].GetView("LoginView");
            _regionManager.Regions[Infrastructure.Models.Regions.Main].Activate( v );

            //_regionManager.RegisterViewWithRegion(Infrastructure.Models.Regions.SideBar, typeof(OGV2P.Admin.Views.CameraView));
            _regionManager.Regions[Infrastructure.Models.Regions.SideBar].Add(new Views.CameraView(_regionManager, _devices,  _user, _container), "CameraView");
        }
    }
}
