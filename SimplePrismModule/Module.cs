using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;


namespace OGV.Admin
{
    public class Module : IModule
    {
        RegionManager _regionManager;

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(OGV.Admin.Views.LoginView));
        }

        public Module(IRegionManager regionManager)
        {
            this._regionManager = (RegionManager) regionManager;
        }
    }
}
