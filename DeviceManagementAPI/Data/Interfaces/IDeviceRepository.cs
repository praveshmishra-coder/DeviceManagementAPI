using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IDeviceRepository
    {
        IEnumerable<Device> GetAllDevices();
        Device GetDeviceById(int deviceId);
        void AddDevice(Device device);
        void UpdateDevice(Device device);
        void DeleteDevice(int deviceId);
    }
}
