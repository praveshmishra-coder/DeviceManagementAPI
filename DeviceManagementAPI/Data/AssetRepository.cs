using System.Data;
using Microsoft.Data.SqlClient;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Data.Interfaces;

namespace DeviceManagementAPI.Data
{
    public class AssetRepository : IAssetRepository
    {
        private readonly string _connectionString;

        public AssetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            var assets = new List<Asset>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT AssetId, DeviceId, AssetName FROM Assets", conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        assets.Add(new Asset
                        {
                            AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                            DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                            AssetName = reader["AssetName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }

            return assets;
        }

        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT AssetId, DeviceId, AssetName FROM Assets WHERE AssetId=@AssetId", conn))
            {
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId;

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Asset
                        {
                            AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                            DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                            AssetName = reader["AssetName"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }

            return null;
        }

        public async Task<int> AddAssetAsync(Asset asset)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "INSERT INTO Assets (DeviceId, AssetName) OUTPUT INSERTED.AssetId VALUES (@DeviceId, @AssetName)", conn))
            {
                cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                cmd.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value = asset.AssetName ?? (object)DBNull.Value;

                await conn.OpenAsync();
                var newId = (int)await cmd.ExecuteScalarAsync();
                return newId;
            }
        }

        public async Task UpdateAssetAsync(Asset asset)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "UPDATE Assets SET DeviceId=@DeviceId, AssetName=@AssetName WHERE AssetId=@AssetId", conn))
            {
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = asset.AssetId;
                cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                cmd.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value = asset.AssetName ?? (object)DBNull.Value;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAssetAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DELETE FROM Assets WHERE AssetId=@AssetId", conn))
            {
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = id;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
