using System.Collections.Generic;

namespace AmeCommon.Database
{
    public class AmeConfig
    {
        public string LiteDbDatabaseFilePath { get; set; }
        public List<string> ProjectDirectories { get; set; }
        public string SvnRoot { get; set; }
        public string SvnUser { get; set; }
        public string SvnPassword { get; set; }
    }
}
