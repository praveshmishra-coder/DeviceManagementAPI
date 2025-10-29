using System.ComponentModel.DataAnnotations;

namespace DeviceManagementAPI.DTOs
{
    // ✅ Response DTO (for returning data)
    public class SignalMeasurementResponseDTO
    {
        public int SignalId { get; set; }
        public string SignalTag { get; set; } = string.Empty;
        public string RegisterAddress { get; set; } = string.Empty;
        public int AssetId { get; set; }
    }

    // ✅ Request DTO (for POST/PUT requests)
    public class SignalMeasurementRequestDTO
    {
        [Required(ErrorMessage = "SignalTag is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "SignalTag must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-\s]+$", ErrorMessage = "SignalTag must be alphanumeric and may include hyphens, underscores, or spaces.")]
        public string SignalTag { get; set; } = string.Empty;

        [Required(ErrorMessage = "RegisterAddress is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "RegisterAddress must be between 1 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-]+$", ErrorMessage = "RegisterAddress must be alphanumeric and may include hyphen or underscore.")]
        public string RegisterAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "AssetId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "AssetId must be a positive integer greater than 0.")]
        public int AssetId { get; set; }
    }
}
