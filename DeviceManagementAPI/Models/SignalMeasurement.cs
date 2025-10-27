namespace DeviceManagementAPI.Models
{
    public class SignalMeasurement
    {
        public int SignalId { get; set; }
        public int AssetId { get; set; }
        public string SignalTag { get; set; }
        public string RegisterAddress { get; set; }
    }
}
