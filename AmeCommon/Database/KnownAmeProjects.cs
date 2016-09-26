using System.Collections.Generic;

namespace AmeCommon.Database
{
    public class KnownAmeProjects
    {
        public string ProjectsRootPath { get; set; }
        public string SelectedProject { get; set; }
        public List<string> KnownProjects { get; set; }
    }

    public class AmeProjectPath
    {
        public string ProjectsRootPath { get; set; }
        public string SelectedProject { get; set; }
        public List<string> KnownProjects { get; set; }
    }
}