using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AmeWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Settings.Default["Setting1"] = "abc";
            Settings.Default.Save();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
