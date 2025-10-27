using System.Data;
using System.Data.SqlClient;
using DeviceManagementAPI.Models;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementAPI.Data
{
    public class DeviceRepository
    {
        private readonly string _connectionString;

        public DeviceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Device> GetAllDevices()
        {
            List<Device> devices = new();
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("SELECT * FROM Devices", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                devices.Add(new Device
                {
                    DeviceId = (int)reader["DeviceId"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString()
                });
            }
            return devices;
        }

        public void AddDevice(Device device)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("INSERT INTO Devices (Name, Description) VALUES (@Name, @Description)", con);
            cmd.Parameters.AddWithValue("@Name", device.Name);
            cmd.Parameters.AddWithValue("@Description", device.Description);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateDevice(Device device)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("UPDATE Devices SET Name=@Name, Description=@Description WHERE DeviceId=@DeviceId", con);
            cmd.Parameters.AddWithValue("@DeviceId", device.DeviceId);
            cmd.Parameters.AddWithValue("@Name", device.Name);
            cmd.Parameters.AddWithValue("@Description", device.Description);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteDevice(int deviceId)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("DELETE FROM Devices WHERE DeviceId=@DeviceId", con);
            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
