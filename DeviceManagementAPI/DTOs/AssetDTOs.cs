namespace DeviceManagementAPI.DTOs
{
    public class AssetResponseDTO
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public int DeviceId { get; set; }
    }

    public class AssetRequestDTO
    {
        public string AssetName { get; set; } = string.Empty;
        public int DeviceId { get; set; }
    }
}
