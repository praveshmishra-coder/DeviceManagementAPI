using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ILogger<AssetController> _logger;

        public AssetController(IAssetRepository assetRepository, ILogger<AssetController> logger)
        {
            _assetRepository = assetRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAllAssets()
        {
            try
            {
                var assets = await _assetRepository.GetAllAssetsAsync();
                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetAssetById(int id)
        {
            try
            {
                var asset = await _assetRepository.GetAssetByIdAsync(id);
                if (asset == null)
                {
                    _logger.LogWarning("Asset with ID {Id} not found.", id);
                    return NotFound($"Asset with ID {id} not found.");
                }

                return Ok(asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Asset>> AddAsset([FromBody] Asset asset)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newId = await _assetRepository.AddAssetAsync(asset);
                asset.AssetId = newId;

                _logger.LogInformation("Asset created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetAssetById), new { id = asset.AssetId }, asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new asset.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] Asset asset)
        {
            try
            {
                if (id != asset.AssetId)
                    return BadRequest("Asset ID mismatch.");

                var existingAsset = await _assetRepository.GetAssetByIdAsync(id);
                if (existingAsset == null)
                    return NotFound($"Asset with ID {id} not found.");

                await _assetRepository.UpdateAssetAsync(asset);
                _logger.LogInformation("Asset with ID {Id} updated successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                var existingAsset = await _assetRepository.GetAssetByIdAsync(id);
                if (existingAsset == null)
                    return NotFound($"Asset with ID {id} not found.");

                await _assetRepository.DeleteAssetAsync(id);
                _logger.LogInformation("Asset with ID {Id} deleted successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
