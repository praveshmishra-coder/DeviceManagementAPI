using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly AssetRepository _repo;
        public AssetController(AssetRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAssets() => Ok(_repo.GetAllAssets());

        [HttpPost]
        public IActionResult AddAsset(Asset asset)
        {
            _repo.AddAsset(asset);
            return Ok("Asset Added");
        }

        [HttpPut]
        public IActionResult UpdateAsset(Asset asset)
        {
            _repo.UpdateAsset(asset);
            return Ok("Asset Updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAsset(int id)
        {
            _repo.DeleteAsset(id);
            return Ok("Asset Deleted");
        }
    }
}
