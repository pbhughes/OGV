using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV2P.AgendaModule
{

    public class AgendaModule: IModule
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
