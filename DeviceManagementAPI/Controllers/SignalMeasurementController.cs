using Microsoft.AspNetCore.Mvc;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;

namespace DeviceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalMeasurementController : ControllerBase
    {
        private readonly ISignalMeasurementRepository _repo;
        public SignalMeasurementController(ISignalMeasurementRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetSignals() => Ok(_repo.GetAllSignals());

        [HttpPost]
        public IActionResult AddSignal(SignalMeasurement signal)
        {
            _repo.AddSignal(signal);
            return Ok("Signal Added Successfully");
        }

        [HttpPut]
        public IActionResult UpdateSignal(SignalMeasurement signal)
        {
            _repo.UpdateSignal(signal);
            return Ok("Signal Updated Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSignal(int id)
        {
            _repo.DeleteSignal(id);
            return Ok("Signal Deleted Successfully");
        }
    }
}
