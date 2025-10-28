namespace DeviceManagementAPI.Models
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
