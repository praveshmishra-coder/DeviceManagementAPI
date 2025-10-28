using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _repository;

        public AssetController(IAssetRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assets = await _repository.GetAllAssetsAsync();
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asset = await _repository.GetAssetByIdAsync(id);
            if (asset == null)
                return NotFound();

            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Asset asset)
        {
            var newId = await _repository.AddAssetAsync(asset);
            return CreatedAtAction(nameof(GetById), new { id = newId }, asset);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Asset asset)
        {
            if (id != asset.AssetId)
                return BadRequest("Asset ID mismatch.");

            await _repository.UpdateAssetAsync(asset);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAssetAsync(id);
            return NoContent();
        }
    }
}
