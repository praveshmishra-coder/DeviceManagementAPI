using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IDeviceRepository
    {
        List<Device> GetAllDevices();
        void AddDevice(Device device);
        void UpdateDevice(Device device);
        void DeleteDevice(int deviceId);
    }
}
