using System.Collections.Generic;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeWeb.Model
{
    public class HomeViewModel
    {
        public List<AmeFotoVideoProject> Projects { get; set; }
        public string DestiantionRoot { get; set; }
        public IList<BackgroundTask> Tasks { get; set; }
    }
}
