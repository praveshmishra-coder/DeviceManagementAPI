using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _repo;
        public DeviceController(IDeviceRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetDevices() => Ok(_repo.GetAllDevices());

        [HttpPost]
        public IActionResult AddDevice(Device device)
        {
            _repo.AddDevice(device);
            return Ok("Device Added Successfully");
        }

        [HttpPut]
        public IActionResult UpdateDevice(Device device)
        {
            _repo.UpdateDevice(device);
            return Ok("Device Updated Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(int id)
        {
            _repo.DeleteDevice(id);
            return Ok("Device Deleted Successfully");
        }
    }
}
