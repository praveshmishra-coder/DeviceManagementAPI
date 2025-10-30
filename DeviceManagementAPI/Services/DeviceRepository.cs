using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DeviceManagementAPI.Services
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(DatabaseHelper db, ILogger<DeviceRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ✅ GET all devices
        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            var devices = new List<Device>();

            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT DeviceId, DeviceName, Description FROM Devices";
                await using var command = new SqlCommand(query, connection);

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    devices.Add(new Device
                    {
                        DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                        DeviceName = reader["DeviceName"].ToString() ?? string.Empty,
                        Description = reader["Description"].ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all devices from database.");
                throw;
            }

            return devices;
        }

        // ✅ GET device by ID
        public async Task<Device?> GetDeviceByIdAsync(int deviceId)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT DeviceId, DeviceName, Description FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Device
                    {
                        DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                        DeviceName = reader["DeviceName"].ToString() ?? string.Empty,
                        Description = reader["Description"].ToString() ?? string.Empty
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching device with ID {DeviceId}", deviceId);
                throw;
            }
        }

        // ✅ ADD new device
        public async Task<int> AddDeviceAsync(Device device)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO Devices (DeviceName, Description)
                    VALUES (@DeviceName, @Description);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 200).Value =
                    device.DeviceName ?? (object)DBNull.Value;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                    device.Description ?? (object)DBNull.Value;

                var newId = await command.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new device: {DeviceName}", device.DeviceName);
                throw;
            }
        }

        // ✅ UPDATE existing device
        public async Task UpdateDeviceAsync(Device device)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    UPDATE Devices
                    SET DeviceName = @DeviceName, Description = @Description
                    WHERE DeviceId = @DeviceId";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 200).Value =
                    device.DeviceName ?? (object)DBNull.Value;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                    device.Description ?? (object)DBNull.Value;
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = device.DeviceId;

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device with ID {DeviceId}", device.DeviceId);
                throw;
            }
        }

        // ✅ DELETE device
        public async Task DeleteDeviceAsync(int deviceId)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "DELETE FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device with ID {DeviceId}", deviceId);
                throw;
            }
        }
    }
}
