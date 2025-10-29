using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalMeasurementController : ControllerBase
    {
        private readonly ISignalMeasurementRepository _signalRepository;
        private readonly ILogger<SignalMeasurementController> _logger;

        public SignalMeasurementController(ISignalMeasurementRepository signalRepository, ILogger<SignalMeasurementController> logger)
        {
            _signalRepository = signalRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalMeasurement>>> GetAllSignals()
        {
            try
            {
                var signals = await _signalRepository.GetAllSignalsAsync();
                return Ok(signals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving signals.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SignalMeasurement>> GetSignalById(int id)
        {
            try
            {
                var signal = await _signalRepository.GetSignalByIdAsync(id);
                if (signal == null)
                {
                    _logger.LogWarning("Signal with ID {Id} not found.", id);
                    return NotFound($"Signal with ID {id} not found.");
                }

                return Ok(signal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving signal with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SignalMeasurement>> AddSignal([FromBody] SignalMeasurement signal)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newId = await _signalRepository.AddSignalAsync(signal);
                signal.SignalId = newId;

                _logger.LogInformation("Signal created with ID {Id}.", newId);
                return CreatedAtAction(nameof(GetSignalById), new { id = signal.SignalId }, signal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding signal.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSignal(int id, [FromBody] SignalMeasurement signal)
        {
            try
            {
                if (id != signal.SignalId)
                    return BadRequest("Signal ID mismatch.");

                var existingSignal = await _signalRepository.GetSignalByIdAsync(id);
                if (existingSignal == null)
                    return NotFound($"Signal with ID {id} not found.");

                await _signalRepository.UpdateSignalAsync(signal);
                _logger.LogInformation("Signal with ID {Id} updated successfully.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating signal with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

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
