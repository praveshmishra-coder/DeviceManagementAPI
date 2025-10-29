namespace DeviceManagementAPI.DTOs
{
    public class SignalMeasurementResponseDTO
    {
        public int SignalId { get; set; }
        public string SignalTag { get; set; } = string.Empty;
        public string RegisterAddress { get; set; } = string.Empty;
        public int AssetId { get; set; }
    }

    public class SignalMeasurementRequestDTO
    {
        public string SignalTag { get; set; } = string.Empty;
        public string RegisterAddress { get; set; } = string.Empty;
        public int AssetId { get; set; }
    }
}
