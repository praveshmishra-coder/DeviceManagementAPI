using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceRepository deviceRepository, ILogger<DeviceController> logger)
        {
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetAllDevices()
        {
            try
            {
                var devices = await _deviceRepository.GetAllDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all devices.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDeviceById(int id)
        {
            try
            {
                var device = await _deviceRepository.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    _logger.LogWarning("Device with ID {Id} not found.", id);
                    return NotFound($"Device with ID {id} not found.");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Device>> AddDevice([FromBody] Device device)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newId = await _deviceRepository.AddDeviceAsync(device);
                device.DeviceId = newId;

                _logger.LogInformation("Device created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetDeviceById), new { id = device.DeviceId }, device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new device.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] Device device)
        {
            try
            {
                if (id != device.DeviceId)
                    return BadRequest("Device ID mismatch.");

                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                    return NotFound($"Device with ID {id} not found.");

                await _deviceRepository.UpdateDeviceAsync(device);
                _logger.LogInformation("Device with ID {Id} updated successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                    return NotFound($"Device with ID {id} not found.");

                await _deviceRepository.DeleteDeviceAsync(id);
                _logger.LogInformation("Device with ID {Id} deleted successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
