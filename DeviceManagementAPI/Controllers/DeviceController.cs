using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _repository;

        public DeviceController(IDeviceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _repository.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _repository.GetDeviceByIdAsync(id);
            if (device == null)
                return NotFound();

            return Ok(device);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Device device)
        {
            var newId = await _repository.AddDeviceAsync(device);
            return CreatedAtAction(nameof(GetById), new { id = newId }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Device device)
        {
            if (id != device.DeviceId)
                return BadRequest("Device ID mismatch.");

            await _repository.UpdateDeviceAsync(device);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteDeviceAsync(id);
            return NoContent();
        }
    }
}
