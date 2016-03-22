using System.Windows;
using AmeCreateProject.ViewModel;
using Prism.Modularity;
using Prism.Unity;
using AmeCommon.MediaTasks;
using AmeCreateProject.Model;

namespace AmeCreateProject
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypeIfMissing(typeof(AmeProjectViewModel), typeof(AmeProjectViewModel), false);
            RegisterTypeIfMissing(typeof(AmeProjectModel), typeof(AmeProjectModel), false);            
            RegisterTypeIfMissing(typeof(MediaService), typeof(MediaService), true);            
        }

        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<PrismAppShell>();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Application.Current.MainWindow = (PrismAppShell)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)ModuleCatalog;
            moduleCatalog.AddModule(typeof(CreateAmeProjectModule)); 
        }
    }
}
