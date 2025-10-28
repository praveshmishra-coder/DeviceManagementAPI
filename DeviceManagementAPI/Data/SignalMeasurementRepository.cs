using System.Data.SqlClient;
using DeviceManagementAPI.Models;
using Microsoft.Extensions.Configuration;
using DeviceManagementAPI.Data.Interfaces;

namespace DeviceManagementAPI.Data
{
    public class SignalMeasurementRepository : ISignalMeasurementRepository
    {
        private readonly string _connectionString;
        public SignalMeasurementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

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
