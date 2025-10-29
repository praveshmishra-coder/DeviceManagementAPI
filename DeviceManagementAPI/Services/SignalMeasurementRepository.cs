using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DeviceManagementAPI.Services
{
    public class SignalMeasurementRepository : ISignalMeasurementRepository
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<SignalMeasurementRepository> _logger;

        public SignalMeasurementRepository(DatabaseHelper db, ILogger<SignalMeasurementRepository> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SignalMeasurement>> GetAllSignalsAsync()
        {
            var signals = new List<SignalMeasurement>();

            try
            {
                await using var con = _db.GetConnection();
                await con.OpenAsync();

                const string query = "SELECT SignalId, AssetId, SignalTag, RegisterAddress FROM SignalMeasurements";
                await using var cmd = new SqlCommand(query, con);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    signals.Add(new SignalMeasurement
                    {
                        SignalId = reader.GetInt32(reader.GetOrdinal("SignalId")),
                        AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                        SignalTag = reader["SignalTag"]?.ToString() ?? string.Empty,
                        RegisterAddress = reader["RegisterAddress"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while fetching all signal measurements.");
                throw new ApplicationException("An error occurred while retrieving signal measurements.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching all signal measurements.");
                throw;
            }

            return signals;
        }

        public async Task<SignalMeasurement?> GetSignalByIdAsync(int signalId)
        {
            try
            {
                await using var con = _db.GetConnection();
                await con.OpenAsync();

                const string query = "SELECT SignalId, AssetId, SignalTag, RegisterAddress FROM SignalMeasurements WHERE SignalId=@SignalId";
                await using var cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@SignalId", SqlDbType.Int).Value = signalId;

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SignalMeasurement
                    {
                        SignalId = reader.GetInt32(reader.GetOrdinal("SignalId")),
                        AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                        SignalTag = reader["SignalTag"]?.ToString() ?? string.Empty,
                        RegisterAddress = reader["RegisterAddress"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while fetching signal with ID {SignalId}.", signalId);
                throw new ApplicationException($"Error retrieving signal {signalId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching signal with ID {SignalId}.", signalId);
                throw;
            }

            return null;
        }

        public async Task<int> AddSignalAsync(SignalMeasurement signal)
        {
            try
            {
                await using var con = _db.GetConnection();
                await con.OpenAsync();

                const string query = @"
                    INSERT INTO SignalMeasurements (AssetId, SignalTag, RegisterAddress)
                    VALUES (@AssetId, @SignalTag, @RegisterAddress);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await using var cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = signal.AssetId;
                cmd.Parameters.Add("@SignalTag", SqlDbType.NVarChar, 200).Value = signal.SignalTag ?? (object)DBNull.Value;
                cmd.Parameters.Add("@RegisterAddress", SqlDbType.NVarChar, 200).Value = signal.RegisterAddress ?? (object)DBNull.Value;

                var newId = await cmd.ExecuteScalarAsync();
                _logger.LogInformation("Signal measurement added successfully with ID {SignalId}", newId);
                return Convert.ToInt32(newId);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation
                {
                    _logger.LogWarning("Attempted to add signal with non-existing AssetId {AssetId}.", signal.AssetId);
                    throw new ApplicationException($"Cannot add signal because AssetId {signal.AssetId} does not exist.", ex);
                }

                _logger.LogError(ex, "SQL error while adding signal measurement: {@Signal}", signal);
                throw new ApplicationException("Error adding signal measurement.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding signal measurement: {@Signal}", signal);
                throw;
            }
        }

        public async Task UpdateSignalAsync(SignalMeasurement signal)
        {
            try
            {
                await using var con = _db.GetConnection();
                await con.OpenAsync();

                const string query = @"
                    UPDATE SignalMeasurements
                    SET AssetId=@AssetId, SignalTag=@SignalTag, RegisterAddress=@RegisterAddress
                    WHERE SignalId=@SignalId";

                await using var cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@SignalId", SqlDbType.Int).Value = signal.SignalId;
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = signal.AssetId;
                cmd.Parameters.Add("@SignalTag", SqlDbType.NVarChar, 200).Value = signal.SignalTag ?? (object)DBNull.Value;
                cmd.Parameters.Add("@RegisterAddress", SqlDbType.NVarChar, 200).Value = signal.RegisterAddress ?? (object)DBNull.Value;

                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No signal measurement found with ID {SignalId} to update.", signal.SignalId);
                else
                    _logger.LogInformation("Signal measurement with ID {SignalId} updated successfully.", signal.SignalId);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    _logger.LogWarning("Attempted to update signal with non-existing AssetId {AssetId}.", signal.AssetId);
                    throw new ApplicationException($"Cannot update signal because AssetId {signal.AssetId} does not exist.", ex);
                }

                _logger.LogError(ex, "SQL error while updating signal measurement: {@Signal}", signal);
                throw new ApplicationException($"Error updating signal measurement {signal.SignalId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating signal measurement: {@Signal}", signal);
                throw;
            }
        }

        public async Task DeleteSignalAsync(int signalId)
        {
            try
            {
                await using var con = _db.GetConnection();
                await con.OpenAsync();

                const string query = "DELETE FROM SignalMeasurements WHERE SignalId=@SignalId";
                await using var cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@SignalId", SqlDbType.Int).Value = signalId;

                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No signal measurement found with ID {SignalId} to delete.", signalId);
                else
                    _logger.LogInformation("Signal measurement with ID {SignalId} deleted successfully.", signalId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while deleting signal measurement with ID {SignalId}.", signalId);
                throw new ApplicationException($"Error deleting signal measurement {signalId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting signal measurement {SignalId}.", signalId);
                throw;
            }
        }
    }
}
