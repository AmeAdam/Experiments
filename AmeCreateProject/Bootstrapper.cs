﻿using System.Windows;
using AmeCreateProject.Utils;
using AmeCreateProject.ViewModel;
using Prism.Modularity;
using Prism.Unity;

namespace AmeCreateProject
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypeIfMissing(typeof(IRemovableMediaUtils), typeof(RemovableMediaUtils), true);
            RegisterTypeIfMissing(typeof(AmeProjectViewModel), typeof(AmeProjectViewModel), false);
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