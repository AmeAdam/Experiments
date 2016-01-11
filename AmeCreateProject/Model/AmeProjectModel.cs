using System;

namespace AmeCreateProject.Model
{
    public class AmeProjectModel
    {
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public string Name { get; set; } = "Unknown";
        public string DirName => Date.ToString("yyyy-MM-dd") + " " + Name;
    }
}
