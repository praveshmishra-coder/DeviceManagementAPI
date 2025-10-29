using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DeviceManagementAPI.Services.Interfaces;
using DeviceManagementAPI.DTOs;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetController> _logger;

        public AssetController(IAssetRepository assetRepository, IMapper mapper, ILogger<AssetController> logger)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ GET: api/asset
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetResponseDTO>>> GetAllAssets()
        {
            try
            {
                var assets = await _assetRepository.GetAllAssetsAsync();
                var assetDtos = _mapper.Map<IEnumerable<AssetResponseDTO>>(assets);
                return Ok(assetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ GET: api/asset/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetResponseDTO>> GetAssetById(int id)
        {
            try
            {
                var asset = await _assetRepository.GetAssetByIdAsync(id);
                if (asset == null)
                {
                    _logger.LogWarning("Asset with ID {Id} not found.", id);
                    return NotFound($"Asset with ID {id} not found.");
                }

                var assetDto = _mapper.Map<AssetResponseDTO>(asset);
                return Ok(assetDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ POST: api/asset
        [HttpPost]
        public async Task<ActionResult<AssetResponseDTO>> AddAsset([FromBody] AssetRequestDTO assetDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var asset = _mapper.Map<Asset>(assetDto);
                var newId = await _assetRepository.AddAssetAsync(asset);

                asset.AssetId = newId;
                var response = _mapper.Map<AssetResponseDTO>(asset);

                _logger.LogInformation("Asset created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetAssetById), new { id = response.AssetId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new asset.");
                return StatusCode(500, "Cannot add Asset because DeviceId does not exist.");
            }
        }

        // ✅ PUT: api/asset/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] AssetRequestDTO assetDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingAsset = await _assetRepository.GetAssetByIdAsync(id);
                if (existingAsset == null)
                {
                    _logger.LogWarning("Asset with ID {Id} not found for update.", id);
                    return NotFound($"Asset with ID {id} not found.");
                }

                // Map changes from DTO → entity
                _mapper.Map(assetDto, existingAsset);
                await _assetRepository.UpdateAssetAsync(existingAsset);

                _logger.LogInformation("Asset with ID {Id} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ DELETE: api/asset/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                var existingAsset = await _assetRepository.GetAssetByIdAsync(id);
                if (existingAsset == null)
                {
                    _logger.LogWarning("Asset with ID {Id} not found for deletion.", id);
                    return NotFound($"Asset with ID {id} not found.");
                }

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
