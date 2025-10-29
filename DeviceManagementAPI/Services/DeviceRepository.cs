using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DeviceManagementAPI.Services
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DatabaseHelper _db;

        public DeviceRepository(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            var devices = new List<Device>();

            await using var con = _db.GetConnection();
            await con.OpenAsync();

            // ✅ Updated column name from 'Name' → 'DeviceName'
            const string query = "SELECT DeviceId, DeviceName, Description FROM Devices";
            await using var cmd = new SqlCommand(query, con);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                devices.Add(new Device
                {
                    DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                    DeviceName = reader["DeviceName"].ToString() ?? string.Empty,
                    Description = reader["Description"].ToString() ?? string.Empty
                });
            }

            return devices;
        }

        public async Task<Device?> GetDeviceByIdAsync(int deviceId)
        {
            await using var con = _db.GetConnection();
            await con.OpenAsync();

            const string query = "SELECT DeviceId, DeviceName, Description FROM Devices WHERE DeviceId=@DeviceId";
            await using var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

            await using var reader = await cmd.ExecuteReaderAsync();
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

        public async Task<int> AddDeviceAsync(Device device)
        {
            await using var con = _db.GetConnection();
            await con.OpenAsync();

            // ✅ Updated 'DeviceName'
            const string query = @"
                INSERT INTO Devices (DeviceName, Description)
                VALUES (@DeviceName, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            await using var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 200).Value = device.DeviceName ?? (object)DBNull.Value;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = device.Description ?? (object)DBNull.Value;

            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task UpdateDeviceAsync(Device device)
        {
            await using var con = _db.GetConnection();
            await con.OpenAsync();

            // ✅ Updated 'DeviceName'
            const string query = @"
                UPDATE Devices 
                SET DeviceName=@DeviceName, Description=@Description 
                WHERE DeviceId=@DeviceId";

            await using var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 200).Value = device.DeviceName ?? (object)DBNull.Value;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = device.Description ?? (object)DBNull.Value;
            cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = device.DeviceId;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteDeviceAsync(int deviceId)
        {
            await using var con = _db.GetConnection();
            await con.OpenAsync();

            const string query = "DELETE FROM Devices WHERE DeviceId=@DeviceId";
            await using var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
