using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeWindowsService.Config
{
    internal class SvnAutoConfig
    {
        IMainConfig mainConfig;

        public SvnAutoConfig(IMainConfig mainConfig)
        {
            this.mainConfig = mainConfig;
        }

        public string[] ProjectsFolders { get; set; }

        public void Save()
        {
            mainConfig.Save();
        }
    }
}
