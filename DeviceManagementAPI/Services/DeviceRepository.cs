using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DeviceManagementAPI.Services
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(SqlConnection connection, ILogger<DeviceRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        // GET ALL DEVICES
        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            var devices = new List<Device>();

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "SELECT DeviceId, DeviceName, Description FROM Devices";
                await using var command = new SqlCommand(query, _connection);

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
            finally
            {
                await _connection.CloseAsync();
            }

            return devices;
        }

        // GET DEVICE BY ID
        public async Task<Device?> GetDeviceByIdAsync(int deviceId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "SELECT DeviceId, DeviceName, Description FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceId", deviceId);

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
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // ADD DEVICE
        public async Task<int> AddDeviceAsync(Device device)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = @"
                    INSERT INTO Devices (DeviceName, Description)
                    VALUES (@DeviceName, @Description);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceName", device.DeviceName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", device.Description ?? (object)DBNull.Value);

                var newId = await command.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new device: {DeviceName}", device.DeviceName);
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // UPDATE DEVICE
        public async Task UpdateDeviceAsync(Device device)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = @"
                    UPDATE Devices
                    SET DeviceName = @DeviceName, Description = @Description
                    WHERE DeviceId = @DeviceId";

                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceName", device.DeviceName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", device.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeviceId", device.DeviceId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device with ID {DeviceId}", device.DeviceId);
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // DELETE DEVICE
        public async Task DeleteDeviceAsync(int deviceId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "DELETE FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceId", deviceId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device with ID {DeviceId}", deviceId);
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
