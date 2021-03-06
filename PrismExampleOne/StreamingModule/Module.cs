﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace OGV.Streaming
{
    public class ModuleStreaming : IModule
    {
        RegionManager _regionManager;

        public ModuleStreaming(IRegionManager regionManager)
        {
            _regionManager = (RegionManager)regionManager;
        }

        public void Initialize()
        {
            
            //_regionManager.RegisterViewWithRegion("SideNavBarRegion", typeof(OGV.Streaming.Views.PublishingPointManagerNavView));
            _regionManager.RegisterViewWithRegion("SidebarRegion", typeof(OGV.Streaming.Views.PublishingPointView));
            //_regionManager.RegisterViewWithRegion("SideNavBarRegion", typeof(OGV.Streaming.Views.StreamerNavView));
            _regionManager.RegisterViewWithRegion("SidebarRegion", typeof(OGV.Streaming.Views.StreamerSplashScreen));

            Uri nn = new Uri(typeof(Views.StreamerSplashScreen).FullName, UriKind.RelativeOrAbsolute);

            _regionManager.RequestNavigate("SidebarRegion", nn);

            

           
        }
    }
}
