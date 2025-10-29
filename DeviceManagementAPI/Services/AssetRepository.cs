using System.Data;
using Microsoft.Data.SqlClient;
using DeviceManagementAPI.Models;
using Microsoft.Extensions.Logging;
using DeviceManagementAPI.Services.Interfaces;

namespace DeviceManagementAPI.Services
{
    public class AssetRepository : IAssetRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<AssetRepository> _logger;

        public AssetRepository(IConfiguration configuration, ILogger<AssetRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string not configured.");
            _logger = logger;
        }

        // ✅ Helper method to verify device existence
        private async Task<bool> DeviceExistsAsync(int deviceId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Devices WHERE DeviceId=@DeviceId", conn);
            cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

            await conn.OpenAsync();
            var count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            var assets = new List<Asset>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT AssetId, DeviceId, AssetName FROM Assets", conn);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

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
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while fetching all assets.");
                throw new ApplicationException("An error occurred while retrieving assets.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching all assets.");
                throw;
            }

            return assets;
        }

        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT AssetId, DeviceId, AssetName FROM Assets WHERE AssetId=@AssetId", conn);
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

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
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while fetching asset with ID {AssetId}", assetId);
                throw new ApplicationException($"Error fetching asset {assetId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching asset {AssetId}", assetId);
                throw;
            }

            return null;
        }

        public async Task<int> AddAssetAsync(Asset asset)
        {
            try
            {
                // ✅ Step 1: Verify Device exists
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to add asset with non-existent DeviceId {DeviceId}", asset.DeviceId);
                    throw new ApplicationException($"Cannot add asset. Device with ID {asset.DeviceId} does not exist.");
                }

                // ✅ Step 2: Proceed with insertion
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    "INSERT INTO Assets (DeviceId, AssetName) OUTPUT INSERTED.AssetId VALUES (@DeviceId, @AssetName)", conn);

                cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                cmd.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value = asset.AssetName ?? (object)DBNull.Value;

                await conn.OpenAsync();
                var newId = (int)await cmd.ExecuteScalarAsync();

                _logger.LogInformation("Asset added successfully with ID {AssetId}", newId);
                return newId;
            }
            catch (ApplicationException)
            {
                throw; // Re-throw custom message (don’t wrap again)
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while adding asset: {@Asset}", asset);
                throw new ApplicationException("Error adding asset.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding asset: {@Asset}", asset);
                throw;
            }
        }

        public async Task UpdateAssetAsync(Asset asset)
        {
            try
            {
                // ✅ Validate Device ID
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to update asset {AssetId} with non-existent DeviceId {DeviceId}", asset.AssetId, asset.DeviceId);
                    throw new ApplicationException($"Cannot update asset. Device with ID {asset.DeviceId} does not exist.");
                }

                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    "UPDATE Assets SET DeviceId=@DeviceId, AssetName=@AssetName WHERE AssetId=@AssetId", conn);

                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = asset.AssetId;
                cmd.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                cmd.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value = asset.AssetName ?? (object)DBNull.Value;

                await conn.OpenAsync();
                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No asset found with ID {AssetId} to update.", asset.AssetId);
                else
                    _logger.LogInformation("Asset with ID {AssetId} updated successfully.", asset.AssetId);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while updating asset: {@Asset}", asset);
                throw new ApplicationException($"Error updating asset {asset.AssetId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating asset: {@Asset}", asset);
                throw;
            }
        }

        public async Task DeleteAssetAsync(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM Assets WHERE AssetId=@AssetId", conn);
                cmd.Parameters.Add("@AssetId", SqlDbType.Int).Value = id;

                await conn.OpenAsync();
                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No asset found with ID {AssetId} to delete.", id);
                else
                    _logger.LogInformation("Asset with ID {AssetId} deleted successfully.", id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while deleting asset {AssetId}", id);
                throw new ApplicationException($"Error deleting asset {id}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting asset {AssetId}", id);
                throw;
            }
        }
    }
}
