using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.DTOs
{
    //  sending data to clients
    public class DeviceResponseDTO
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    //  Request DTO 
    public class DeviceRequestDTO
    {
        [Required(ErrorMessage = "Device name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Device name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-\s]+$", ErrorMessage = "Device name must be alphanumeric and may include hyphen, underscore, or space.")]
        public string DeviceName { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [RegularExpression(@"^[A-Za-z0-9.,\-\s_]*$", ErrorMessage = "Description may contain letters, numbers, spaces, dots, commas, hyphens, and underscores.")]
        public string? Description { get; set; }
    }
}
