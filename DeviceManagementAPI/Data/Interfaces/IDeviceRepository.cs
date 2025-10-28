using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task<Device?> GetDeviceByIdAsync(int deviceId);
        Task<int> AddDeviceAsync(Device device);
        Task UpdateDeviceAsync(Device device);
        Task DeleteDeviceAsync(int deviceId);
    }
}
