using System.Collections.Generic;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public interface IDeviceManager
    {
        Device GetDevice(string name);
        Device GetDevice(int id);
        List<Device> GetAllDevices();
        void UpdateDevice(Device device);
        void RemoveDevice(int id);
    }
}
