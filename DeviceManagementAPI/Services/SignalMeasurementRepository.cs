using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DeviceManagementAPI.Services
{
    public class SignalMeasurementRepository : ISignalMeasurementRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SignalMeasurementRepository> _logger;

        public SignalMeasurementRepository(IConfiguration configuration, ILogger<SignalMeasurementRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found in appsettings.json");
            _logger = logger;
        }

        // ✅ Helper method: Create connection
        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        // ✅ Helper: Check if Asset exists
        private async Task<bool> AssetExistsAsync(int assetId)
        {
            try
            {
                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT COUNT(1) FROM Assets WHERE AssetId = @AssetId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId;

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if Asset {AssetId} exists.", assetId);
                throw;
            }
        }

        // ✅ Get all Signals
        public async Task<IEnumerable<SignalMeasurement>> GetAllSignalsAsync()
        {
            var signals = new List<SignalMeasurement>();

            try
            {
                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT SignalId, AssetId, SignalTag, RegisterAddress FROM SignalMeasurements";
                await using var command = new SqlCommand(query, connection);

                await using var reader = await command.ExecuteReaderAsync();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all signal measurements.");
                throw;
            }

            return signals;
        }

        // ✅ Get Signal by ID
        public async Task<SignalMeasurement?> GetSignalByIdAsync(int signalId)
        {
            try
            {
                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT SignalId, AssetId, SignalTag, RegisterAddress FROM SignalMeasurements WHERE SignalId = @SignalId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@SignalId", SqlDbType.Int).Value = signalId;

                await using var reader = await command.ExecuteReaderAsync();
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

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching signal with ID {SignalId}", signalId);
                throw;
            }
        }

        // ✅ Add new Signal
        public async Task<int> AddSignalAsync(SignalMeasurement signal)
        {
            try
            {
                // Validate Asset existence
                if (!await AssetExistsAsync(signal.AssetId))
                {
                    _logger.LogWarning("Attempted to add signal with invalid AssetId {AssetId}", signal.AssetId);
                    throw new ApplicationException($"Cannot add signal. Asset with ID {signal.AssetId} does not exist.");
                }

                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO SignalMeasurements (AssetId, SignalTag, RegisterAddress)
                    OUTPUT INSERTED.SignalId
                    VALUES (@AssetId, @SignalTag, @RegisterAddress);";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = signal.AssetId;
                command.Parameters.Add("@SignalTag", SqlDbType.NVarChar, 200).Value =
                    signal.SignalTag ?? (object)DBNull.Value;
                command.Parameters.Add("@RegisterAddress", SqlDbType.NVarChar, 200).Value =
                    signal.RegisterAddress ?? (object)DBNull.Value;

                var newId = await command.ExecuteScalarAsync();
                _logger.LogInformation("Signal measurement added successfully with ID {SignalId}", newId);

                return Convert.ToInt32(newId);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding signal measurement: {@Signal}", signal);
                throw;
            }
        }

        // ✅ Update Signal
        public async Task UpdateSignalAsync(SignalMeasurement signal)
        {
            try
            {
                if (!await AssetExistsAsync(signal.AssetId))
                {
                    _logger.LogWarning("Attempted to update signal {SignalId} with invalid AssetId {AssetId}",
                        signal.SignalId, signal.AssetId);
                    throw new ApplicationException($"Cannot update signal. Asset with ID {signal.AssetId} does not exist.");
                }

                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    UPDATE SignalMeasurements 
                    SET AssetId = @AssetId, SignalTag = @SignalTag, RegisterAddress = @RegisterAddress
                    WHERE SignalId = @SignalId;";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@SignalId", SqlDbType.Int).Value = signal.SignalId;
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = signal.AssetId;
                command.Parameters.Add("@SignalTag", SqlDbType.NVarChar, 200).Value =
                    signal.SignalTag ?? (object)DBNull.Value;
                command.Parameters.Add("@RegisterAddress", SqlDbType.NVarChar, 200).Value =
                    signal.RegisterAddress ?? (object)DBNull.Value;

                var rows = await command.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No signal found with ID {SignalId} to update.", signal.SignalId);
                else
                    _logger.LogInformation("Signal with ID {SignalId} updated successfully.", signal.SignalId);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating signal measurement: {@Signal}", signal);
                throw;
            }
        }

        // ✅ Delete Signal
        public async Task DeleteSignalAsync(int signalId)
        {
            try
            {
                await using var connection = GetConnection();
                await connection.OpenAsync();

                const string query = "DELETE FROM SignalMeasurements WHERE SignalId = @SignalId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@SignalId", SqlDbType.Int).Value = signalId;

                var rows = await command.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No signal found with ID {SignalId} to delete.", signalId);
                else
                    _logger.LogInformation("Signal with ID {SignalId} deleted successfully.", signalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting signal measurement with ID {SignalId}", signalId);
                throw;
            }
        }
    }
}
