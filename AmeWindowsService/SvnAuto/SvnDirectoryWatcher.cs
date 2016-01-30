using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeWindowsService.SvnAuto
{
    class SvnDirectoryWatcher
    {
        FileSystemWatcher watcher;

        public SvnDirectoryWatcher()
        {
            watcher = new FileSystemWatcher("e:\\projekty");
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = true;
            watcher.Filter = "*.prproj";

        }
    }
}
