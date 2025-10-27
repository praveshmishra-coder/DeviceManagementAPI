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

        // ---------------- DEVICE ----------------
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

        // ---------------- ASSET ----------------
        public List<Asset> GetAllAssets()
        {
            List<Asset> assets = new();
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("SELECT * FROM Assets", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                assets.Add(new Asset
                {
                    AssetId = (int)reader["AssetId"],
                    DeviceId = (int)reader["DeviceId"],
                    AssetName = reader["AssetName"].ToString()
                });
            }
            return assets;
        }

        public void AddAsset(Asset asset)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("INSERT INTO Assets (DeviceId, AssetName) VALUES (@DeviceId, @AssetName)", con);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateAsset(Asset asset)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("UPDATE Assets SET DeviceId=@DeviceId, AssetName=@AssetName WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteAsset(int assetId)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("DELETE FROM Assets WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", assetId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- SIGNAL MEASUREMENTS ----------------
        public List<SignalMeasurement> GetAllSignals()
        {
            List<SignalMeasurement> signals = new();
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("SELECT * FROM SignalMeasurements", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                signals.Add(new SignalMeasurement
                {
                    SignalId = (int)reader["SignalId"],
                    AssetId = (int)reader["AssetId"],
                    SignalTag = reader["SignalTag"].ToString(),
                    RegisterAddress = reader["RegisterAddress"].ToString()
                });
            }
            return signals;
        }

        public void AddSignal(SignalMeasurement signal)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("INSERT INTO SignalMeasurements (AssetId, SignalTag, RegisterAddress) VALUES (@AssetId, @SignalTag, @RegisterAddress)", con);
            cmd.Parameters.AddWithValue("@AssetId", signal.AssetId);
            cmd.Parameters.AddWithValue("@SignalTag", signal.SignalTag);
            cmd.Parameters.AddWithValue("@RegisterAddress", signal.RegisterAddress);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateSignal(SignalMeasurement signal)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("UPDATE SignalMeasurements SET AssetId=@AssetId, SignalTag=@SignalTag, RegisterAddress=@RegisterAddress WHERE SignalId=@SignalId", con);
            cmd.Parameters.AddWithValue("@SignalId", signal.SignalId);
            cmd.Parameters.AddWithValue("@AssetId", signal.AssetId);
            cmd.Parameters.AddWithValue("@SignalTag", signal.SignalTag);
            cmd.Parameters.AddWithValue("@RegisterAddress", signal.RegisterAddress);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteSignal(int signalId)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("DELETE FROM SignalMeasurements WHERE SignalId=@SignalId", con);
            cmd.Parameters.AddWithValue("@SignalId", signalId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
