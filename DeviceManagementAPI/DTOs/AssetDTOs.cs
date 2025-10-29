using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.DTOs
{
    // ✅ Response DTO (for returning data to client)
    public class AssetResponseDTO
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public int DeviceId { get; set; }
    }

    // ✅ Request DTO (for POST / PUT requests)
    public class AssetRequestDTO
    {
        [Required(ErrorMessage = "AssetName is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "AssetName must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-\s]+$", ErrorMessage = "AssetName must be alphanumeric and may include hyphens, underscores, or spaces.")]
        public string AssetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "DeviceId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "DeviceId must be a positive integer greater than 0.")]
        public int DeviceId { get; set; }
    }
}
