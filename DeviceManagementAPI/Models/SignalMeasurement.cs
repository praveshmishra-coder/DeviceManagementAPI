namespace DeviceManagementAPI.Models
{
    public class SignalMeasurement
    {
        public int SignalId { get; set; }
        public int AssetId { get; set; }
        public string SignalTag { get; set; } = string.Empty;
        public string RegisterAddress { get; set; } = string.Empty;
    }
}
