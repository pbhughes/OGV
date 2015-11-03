using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV2P.Streaming
{
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;

    public class StreamingModule : IModule
    {
        private readonly IRegionViewRegistry _regionViewRegistry;

        public StreamingModule(IRegionViewRegistry registry)
        {
            _regionViewRegistry = registry;
        }
        public void Initialize()
        {
            _regionViewRegistry.RegisterViewWithRegion("SideBarRegion", typeof(Views.StreamingView));
        }
    }
}
