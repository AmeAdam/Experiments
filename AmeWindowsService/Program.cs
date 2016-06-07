using Microsoft.Practices.Unity;
using System.ServiceProcess;

namespace AmeWindowsService
{
    static class Program 
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<Service>();
                var service = container.Resolve<Service>();
                service.RunStandalone(args);

                //var servicesToRun = new ServiceBase[] {service};
                //ServiceBase.Run(servicesToRun);
            }
        }
    }
}
