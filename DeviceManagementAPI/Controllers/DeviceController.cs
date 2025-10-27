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

        [HttpGet]
        public IActionResult GetDevices() => Ok(_repo.GetAllDevices());

        [HttpPost]
        public IActionResult AddDevice(Device device)
        {
            _repo.AddDevice(device);
            return Ok("Device Added");
        }

        [HttpPut]
        public IActionResult UpdateDevice(Device device)
        {
            _repo.UpdateDevice(device);
            return Ok("Device Updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(int id)
        {
            _repo.DeleteDevice(id);
            return Ok("Device Deleted");
        }
    }
}
