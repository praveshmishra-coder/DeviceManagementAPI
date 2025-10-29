namespace DeviceManagementAPI.DTOs
{
    // For returning data to clients
    public class DeviceResponseDTO
    {
        public int DeviceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // For accepting data from client (POST/PUT)
    public class DeviceRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
