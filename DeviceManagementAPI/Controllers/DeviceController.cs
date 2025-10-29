using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DeviceManagementAPI.DTOs;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceRepository deviceRepository, IMapper mapper, ILogger<DeviceController> logger)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ GET: api/device
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetAllDevices()
        {
            try
            {
                var devices = await _deviceRepository.GetAllDevicesAsync();
                var deviceDtos = _mapper.Map<IEnumerable<DeviceResponseDTO>>(devices);
                return Ok(deviceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all devices.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ GET: api/device/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceResponseDTO>> GetDeviceById(int id)
        {
            try
            {
                var device = await _deviceRepository.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    _logger.LogWarning("Device with ID {Id} not found.", id);
                    return NotFound($"Device with ID {id} not found.");
                }

                var deviceDto = _mapper.Map<DeviceResponseDTO>(device);
                return Ok(deviceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ POST: api/device
        [HttpPost]
        public async Task<ActionResult<DeviceResponseDTO>> AddDevice([FromBody] DeviceRequestDTO deviceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var device = _mapper.Map<Device>(deviceDto);
                var newId = await _deviceRepository.AddDeviceAsync(device);

                device.DeviceId = newId;
                var response = _mapper.Map<DeviceResponseDTO>(device);

                _logger.LogInformation("Device created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetDeviceById), new { id = response.DeviceId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new device.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ PUT: api/device/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] DeviceRequestDTO deviceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                {
                    _logger.LogWarning("Device with ID {Id} not found for update.", id);
                    return NotFound($"Device with ID {id} not found.");
                }

                // Map DTO onto existing entity
                _mapper.Map(deviceDto, existingDevice);
                await _deviceRepository.UpdateDeviceAsync(existingDevice);

                _logger.LogInformation("Device with ID {Id} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ DELETE: api/device/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                {
                    _logger.LogWarning("Device with ID {Id} not found for deletion.", id);
                    return NotFound($"Device with ID {id} not found.");
                }

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
