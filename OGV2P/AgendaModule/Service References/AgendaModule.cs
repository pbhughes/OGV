using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace OGV2P.AgendaModule
{
    public class AgendaModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;

        public AgendaModule(IRegionViewRegistry registry)
        {
            _regionViewRegistry = registry;
        }

        public void Initialize()
        {
            _regionViewRegistry.RegisterViewWithRegion("MainRegion", typeof(Views.PlaceHolder));
        }
    }
}
