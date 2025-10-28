using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _repo;
        public AssetController(IAssetRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAssets() => Ok(_repo.GetAllAssets());

        [HttpPost]
        public IActionResult AddAsset(Asset asset)
        {
            _repo.AddAsset(asset);
            return Ok("Asset Added Successfully");
        }

        [HttpPut]
        public IActionResult UpdateAsset(Asset asset)
        {
            _repo.UpdateAsset(asset);
            return Ok("Asset Updated Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAsset(int id)
        {
            _repo.DeleteAsset(id);
            return Ok("Asset Deleted Successfully");
        }
    }
}
