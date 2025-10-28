using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DeviceManagementAPI.Data
{
    public class SignalMeasurementRepository : ISignalMeasurementRepository
    {
        private readonly DatabaseHelper _db;
        public SignalMeasurementRepository(DatabaseHelper db) => _db = db;

        public IEnumerable<SignalMeasurement> GetAllSignals()
        {
            var list = new List<SignalMeasurement>();
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM SignalMeasurements", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SignalMeasurement
                {
                    SignalId = (int)reader["SignalId"],
                    AssetId = (int)reader["AssetId"],
                    SignalTag = reader["SignalTag"].ToString(),
                    RegisterAddress = reader["RegisterAddress"].ToString()
                });
            }
            return list;
        }

        public SignalMeasurement GetSignalById(int signalId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM SignalMeasurements WHERE SignalId=@SignalId", con);
            cmd.Parameters.AddWithValue("@SignalId", signalId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new SignalMeasurement
                {
                    SignalId = (int)reader["SignalId"],
                    AssetId = (int)reader["AssetId"],
                    SignalTag = reader["SignalTag"].ToString(),
                    RegisterAddress = reader["RegisterAddress"].ToString()
                };
            }
            return null;
        }

        public void AddSignal(SignalMeasurement signal)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO SignalMeasurements (AssetId, SignalTag, RegisterAddress) VALUES (@AssetId, @SignalTag, @RegisterAddress)", con);
            cmd.Parameters.AddWithValue("@AssetId", signal.AssetId);
            cmd.Parameters.AddWithValue("@SignalTag", signal.SignalTag);
            cmd.Parameters.AddWithValue("@RegisterAddress", signal.RegisterAddress);
            cmd.ExecuteNonQuery();
        }

        public void UpdateSignal(SignalMeasurement signal)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand(
                "UPDATE SignalMeasurements SET AssetId=@AssetId, SignalTag=@SignalTag, RegisterAddress=@RegisterAddress WHERE SignalId=@SignalId", con);
            cmd.Parameters.AddWithValue("@SignalId", signal.SignalId);
            cmd.Parameters.AddWithValue("@AssetId", signal.AssetId);
            cmd.Parameters.AddWithValue("@SignalTag", signal.SignalTag);
            cmd.Parameters.AddWithValue("@RegisterAddress", signal.RegisterAddress);
            cmd.ExecuteNonQuery();
        }

        public void DeleteSignal(int signalId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM SignalMeasurements WHERE SignalId=@SignalId", con);
            cmd.Parameters.AddWithValue("@SignalId", signalId);
            cmd.ExecuteNonQuery();
        }
    }
}
