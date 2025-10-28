using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DeviceManagementAPI.Data
{
    public class SignalMeasurementRepository : ISignalMeasurementRepository
    {
        private readonly DatabaseHelper _db;

        public SignalMeasurementRepository(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SignalMeasurement>> GetAllSignalsAsync()
        {
            var signals = new List<SignalMeasurement>();

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
                    SignalTag = reader["SignalTag"].ToString() ?? string.Empty,
                    RegisterAddress = reader["RegisterAddress"].ToString() ?? string.Empty
                });
            }

            return signals;
        }

        public async Task<SignalMeasurement?> GetSignalByIdAsync(int signalId)
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
                    SignalTag = reader["SignalTag"].ToString() ?? string.Empty,
                    RegisterAddress = reader["RegisterAddress"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> AddSignalAsync(SignalMeasurement signal)
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
            return Convert.ToInt32(newId);
        }

        public async Task UpdateSignalAsync(SignalMeasurement signal)
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

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteSignalAsync(int signalId)
        {
            await using var con = _db.GetConnection();
            await con.OpenAsync();

            const string query = "DELETE FROM SignalMeasurements WHERE SignalId=@SignalId";
            await using var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@SignalId", SqlDbType.Int).Value = signalId;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
