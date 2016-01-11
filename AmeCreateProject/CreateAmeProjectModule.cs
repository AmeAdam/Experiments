using AmeCreateProject.Views;
using Prism.Modularity;
using Prism.Regions;

namespace AmeCreateProject
{
    public class CreateAmeProjectModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;
        public CreateAmeProjectModule(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }
        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion("CenterRegion", typeof(AmeProjectView));
        }
    }
}
