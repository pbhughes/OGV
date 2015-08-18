
namespace OGV2P.Admin
{
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;

    public class AdminModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;

        public AdminModule(IRegionViewRegistry registry)
        {
            _regionViewRegistry = registry;
        }

        public void Initialize()
        {
            _regionViewRegistry.RegisterViewWithRegion("SideBarRegion", typeof(Views.ServicesView));
        }
    }
}
