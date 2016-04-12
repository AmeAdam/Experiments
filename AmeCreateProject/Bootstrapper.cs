using System.Windows;
using AmeCreateProject.ViewModel;
using Prism.Modularity;
using Prism.Unity;
using AmeCommon.MediaTasks;
using AmeCreateProject.Model;
using AmeCommon.Settings;
using AmeCommon.Svn;

namespace AmeCreateProject
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypeIfMissing(typeof(AmeProjectViewModel), typeof(AmeProjectViewModel), false);
            RegisterTypeIfMissing(typeof(AmeProjectModel), typeof(AmeProjectModel), false);

            RegisterTypeIfMissing(typeof(ISettingsProvider), typeof(SettingsProvider), false);
            RegisterTypeIfMissing(typeof(IMediaTaskFactory), typeof(MediaTaskFactory), false);
            RegisterTypeIfMissing(typeof(IMediaService), typeof(MediaService), true);
            RegisterTypeIfMissing(typeof(ISvnUtils), typeof(SvnUtils), true);
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
