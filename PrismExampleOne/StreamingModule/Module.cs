using System;
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
            _regionManager.RegisterViewWithRegion("SidebarRegion", typeof(OGV.Streaming.Views.PublishingPointView));
        }
    }
}
