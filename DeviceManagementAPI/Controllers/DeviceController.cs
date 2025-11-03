using AutoMapper;
using DeviceManagementAPI.DTOs;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        //GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetAllDevices()
        {
            try
            {
                var devices = await _deviceRepository.GetAllDevicesAsync();
                var response = _mapper.Map<IEnumerable<DeviceResponseDTO>>(devices);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching devices.");
                return StatusCode(500, "An error occurred while retrieving devices.");
            }
        }

        //GET BY ID
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

                var response = _mapper.Map<DeviceResponseDTO>(device);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching device with ID {DeviceId}", id);
                return StatusCode(500, "An error occurred while retrieving the device.");
            }
        }

        // POST
        [HttpPost]
        public async Task<ActionResult<DeviceResponseDTO>> AddDevice(DeviceRequestDTO deviceDto)
        {
            try
            {
                var device = _mapper.Map<Device>(deviceDto);
                var newId = await _deviceRepository.AddDeviceAsync(device);

                var createdDevice = await _deviceRepository.GetDeviceByIdAsync(newId);
                var response = _mapper.Map<DeviceResponseDTO>(createdDevice);

                return CreatedAtAction(nameof(GetDeviceById), new { id = newId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new device.");
                return StatusCode(500, "An error occurred while adding the device.");
            }
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, DeviceRequestDTO deviceDto)
        {
            try
            {
                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                    return NotFound($"Device with ID {id} not found.");

                _mapper.Map(deviceDto, existingDevice); // ✅ Copy DTO → Model
                existingDevice.DeviceId = id;

                await _deviceRepository.UpdateDeviceAsync(existingDevice);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device with ID {DeviceId}", id);
                return StatusCode(500, "An error occurred while updating the device.");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                var existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
                if (existingDevice == null)
                    return NotFound($"Device with ID {id} not found.");

                await _deviceRepository.DeleteDeviceAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device with ID {DeviceId}", id);
                return StatusCode(500, "An error occurred while deleting the device.");
            }
        }
    }
}
