using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalMeasurementController : ControllerBase
    {
        private readonly SignalMeasurementRepository _repo;
        public SignalMeasurementController(SignalMeasurementRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetSignals() => Ok(_repo.GetAllSignals());

        [HttpPost]
        public IActionResult AddSignal(SignalMeasurement signal)
        {
            _repo.AddSignal(signal);
            return Ok("Signal Added");
        }

        [HttpPut]
        public IActionResult UpdateSignal(SignalMeasurement signal)
        {
            _repo.UpdateSignal(signal);
            return Ok("Signal Updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSignal(int id)
        {
            _repo.DeleteSignal(id);
            return Ok("Signal Deleted");
        }
    }
}
