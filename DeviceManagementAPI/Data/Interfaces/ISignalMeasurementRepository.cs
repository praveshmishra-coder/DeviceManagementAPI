using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface ISignalMeasurementRepository
    {
        IEnumerable<SignalMeasurement> GetAllSignals();
        SignalMeasurement GetSignalById(int signalId);
        void AddSignal(SignalMeasurement signal);
        void UpdateSignal(SignalMeasurement signal);
        void DeleteSignal(int signalId);
    }
}
