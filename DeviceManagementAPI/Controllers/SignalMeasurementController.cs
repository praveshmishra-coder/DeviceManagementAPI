using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.DTOs;
using DeviceManagementAPI.Models;
using AutoMapper;
using DeviceManagementAPI.Services.Interfaces;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalMeasurementController : ControllerBase
    {
        private readonly ISignalMeasurementRepository _signalRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SignalMeasurementController> _logger;

        public SignalMeasurementController(
            ISignalMeasurementRepository signalRepository,
            IMapper mapper,
            ILogger<SignalMeasurementController> logger)
        {
            _signalRepository = signalRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ GET: api/signalmeasurement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalMeasurementResponseDTO>>> GetAllSignals()
        {
            try
            {
                var signals = await _signalRepository.GetAllSignalsAsync();
                var response = _mapper.Map<IEnumerable<SignalMeasurementResponseDTO>>(signals);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving signals.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ GET: api/signalmeasurement/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SignalMeasurementResponseDTO>> GetSignalById(int id)
        {
            try
            {
                var signal = await _signalRepository.GetSignalByIdAsync(id);
                if (signal == null)
                {
                    _logger.LogWarning("Signal with ID {Id} not found.", id);
                    return NotFound($"Signal with ID {id} not found.");
                }

                var response = _mapper.Map<SignalMeasurementResponseDTO>(signal);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving signal with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ POST: api/signalmeasurement
        [HttpPost]
        public async Task<ActionResult<SignalMeasurementResponseDTO>> AddSignal([FromBody] SignalMeasurementRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map DTO to entity
                var signal = _mapper.Map<SignalMeasurement>(request);

                var newId = await _signalRepository.AddSignalAsync(signal);
                signal.SignalId = newId;

                var response = _mapper.Map<SignalMeasurementResponseDTO>(signal);

                _logger.LogInformation("Signal created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetSignalById), new { id = response.SignalId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding signal.");
                return StatusCode(500, "Cannot add signal because AssetId does not exist.");
            }
        }

        // ✅ PUT: api/signalmeasurement/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSignal(int id, [FromBody] SignalMeasurementRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingSignal = await _signalRepository.GetSignalByIdAsync(id);
                if (existingSignal == null)
                    return NotFound($"Signal with ID {id} not found.");

                // Map updated fields from DTO
                _mapper.Map(request, existingSignal);

                await _signalRepository.UpdateSignalAsync(existingSignal);
                _logger.LogInformation("Signal with ID {Id} updated successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating signal with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // ✅ DELETE: api/signalmeasurement/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSignal(int id)
        {
            try
            {
                var existingSignal = await _signalRepository.GetSignalByIdAsync(id);
                if (existingSignal == null)
                    return NotFound($"Signal with ID {id} not found.");

                await _signalRepository.DeleteSignalAsync(id);
                _logger.LogInformation("Signal with ID {Id} deleted successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting signal with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
