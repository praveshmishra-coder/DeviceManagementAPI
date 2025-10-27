using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceRepository _repo;

        public DeviceController(DeviceRepository repo)
        {
            _repo = repo;
        }

        // ---------------- DEVICE ----------------
        [HttpGet("devices")]
        public IActionResult GetDevices() => Ok(_repo.GetAllDevices());

        [HttpPost("devices")]
        public IActionResult AddDevice(Device device)
        {
            _repo.AddDevice(device);
            return Ok("Device Added");
        }

        [HttpPut("devices")]
        public IActionResult UpdateDevice(Device device)
        {
            _repo.UpdateDevice(device);
            return Ok("Device Updated");
        }

        [HttpDelete("devices/{id}")]
        public IActionResult DeleteDevice(int id)
        {
            _repo.DeleteDevice(id);
            return Ok("Device Deleted");
        }

        // ---------------- ASSET ----------------
        [HttpGet("assets")]
        public IActionResult GetAssets() => Ok(_repo.GetAllAssets());

        [HttpPost("assets")]
        public IActionResult AddAsset(Asset asset)
        {
            _repo.AddAsset(asset);
            return Ok("Asset Added");
        }

        [HttpPut("assets")]
        public IActionResult UpdateAsset(Asset asset)
        {
            _repo.UpdateAsset(asset);
            return Ok("Asset Updated");
        }

        [HttpDelete("assets/{id}")]
        public IActionResult DeleteAsset(int id)
        {
            _repo.DeleteAsset(id);
            return Ok("Asset Deleted");
        }

        // ---------------- SIGNAL ----------------
        [HttpGet("signals")]
        public IActionResult GetSignals() => Ok(_repo.GetAllSignals());

        [HttpPost("signals")]
        public IActionResult AddSignal(SignalMeasurement signal)
        {
            _repo.AddSignal(signal);
            return Ok("Signal Added");
        }

        [HttpPut("signals")]
        public IActionResult UpdateSignal(SignalMeasurement signal)
        {
            _repo.UpdateSignal(signal);
            return Ok("Signal Updated");
        }

        [HttpDelete("signals/{id}")]
        public IActionResult DeleteSignal(int id)
        {
            _repo.DeleteSignal(id);
            return Ok("Signal Deleted");
        }
    }
}
