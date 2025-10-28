using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface ISignalMeasurementRepository
    {
        List<SignalMeasurement> GetAllSignals();
        void AddSignal(SignalMeasurement signal);
        void UpdateSignal(SignalMeasurement signal);
        void DeleteSignal(int signalId);
    }
}
