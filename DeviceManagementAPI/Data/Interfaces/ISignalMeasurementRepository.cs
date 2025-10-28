using DeviceManagementAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface ISignalMeasurementRepository
    {
        Task<IEnumerable<SignalMeasurement>> GetAllSignalsAsync();
        Task<SignalMeasurement?> GetSignalByIdAsync(int signalId);
        Task<int> AddSignalAsync(SignalMeasurement signal);
        Task UpdateSignalAsync(SignalMeasurement signal);
        Task DeleteSignalAsync(int signalId);
    }
}
