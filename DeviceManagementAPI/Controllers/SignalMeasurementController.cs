using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalMeasurementController : ControllerBase
    {
        private readonly ISignalMeasurementRepository _repository;

        public SignalMeasurementController(ISignalMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var signals = await _repository.GetAllSignalsAsync();
            return Ok(signals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var signal = await _repository.GetSignalByIdAsync(id);
            if (signal == null)
                return NotFound();

            return Ok(signal);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SignalMeasurement signal)
        {
            var newId = await _repository.AddSignalAsync(signal);
            return CreatedAtAction(nameof(GetById), new { id = newId }, signal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SignalMeasurement signal)
        {
            if (id != signal.SignalId)
                return BadRequest("Signal ID mismatch.");

            await _repository.UpdateSignalAsync(signal);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteSignalAsync(id);
            return NoContent();
        }
    }
}
