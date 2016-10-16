using System.Collections.Generic;

namespace AmeCommon.Model
{
    public class Device
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }
        public List<DeviceCaptureInfo> Captures { get; set; }

        public override string ToString()
        {
            return UniqueName;
        }
    }
}