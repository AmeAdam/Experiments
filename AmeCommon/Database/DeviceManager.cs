using System.Collections.Generic;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;

namespace AmeCommon.Database
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IDatabase database;

        public DeviceManager(IDatabase database)
        {
            this.database = database;
        }

        public Device GetDevice(string name)
        {
            return database.Devices.FindOne(d => d.UniqueName == name);
        }

        public Device GetDevice(int id)
        {
            return database.Devices.FindById(id);
        }

        public List<Device> GetAllDevices()
        {
            return database.Devices.FindAll().ToList();
        }

        public void UpdateDevice(Device device)
        {
            if (device.Id == 0)
                database.Devices.Insert(device);
            else
                database.Devices.Update(device);
        }

        public void RemoveDevice(int id)
        {
            database.Devices.Delete(id);
        }
    }
}