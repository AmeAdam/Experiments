using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using MyXml;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IUnityContainer container = new UnityContainer())
            {
                ContainerInstaller.Install(container);
                container.RegisterType<Form1>();
                var form = container.Resolve<Form1>();
                Application.Run(form);
            }
        }
    }
}
