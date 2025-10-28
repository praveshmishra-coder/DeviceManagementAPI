using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DeviceManagementAPI.Data
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DatabaseHelper _db;
        public DeviceRepository(DatabaseHelper db) => _db = db;

        public IEnumerable<Device> GetAllDevices()
        {
            var list = new List<Device>();
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Devices", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Device
                {
                    DeviceId = (int)reader["DeviceId"],
                    DeviceName = reader["Name"].ToString(),   // fixed column name
                    Description = reader["Description"].ToString()
                });
            }
            return list;
        }

        public Device GetDeviceById(int deviceId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Devices WHERE DeviceId=@DeviceId", con);
            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Device
                {
                    DeviceId = (int)reader["DeviceId"],
                    DeviceName = reader["Name"].ToString(),  // fixed column name
                    Description = reader["Description"].ToString()
                };
            }
            return null;
        }

        public void AddDevice(Device device)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Devices (Name, Description) VALUES (@Name, @Description)", con);
            cmd.Parameters.AddWithValue("@Name", device.DeviceName);
            cmd.Parameters.AddWithValue("@Description", device.Description);
            cmd.ExecuteNonQuery();
        }

        public void UpdateDevice(Device device)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand(
                "UPDATE Devices SET Name=@Name, Description=@Description WHERE DeviceId=@DeviceId", con);
            cmd.Parameters.AddWithValue("@Name", device.DeviceName);
            cmd.Parameters.AddWithValue("@Description", device.Description);
            cmd.Parameters.AddWithValue("@DeviceId", device.DeviceId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDevice(int deviceId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM Devices WHERE DeviceId=@DeviceId", con);
            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
            cmd.ExecuteNonQuery();
        }
    }
}
