using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using DeviceManagementAPI.Data;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;

namespace DeviceManagementAPI.Services
{
    public class AssetRepository : IAssetRepository
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<AssetRepository> _logger;

        public AssetRepository(DatabaseHelper db, ILogger<AssetRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ✅ Helper: Check if a Device exists
        private async Task<bool> DeviceExistsAsync(int deviceId)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT COUNT(1) FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if device {DeviceId} exists", deviceId);
                throw;
            }
        }

        // ✅ Get all Assets
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            var assets = new List<Asset>();

            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT AssetId, DeviceId, AssetName FROM Assets";
                await using var command = new SqlCommand(query, connection);

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    assets.Add(new Asset
                    {
                        AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                        DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                        AssetName = reader["AssetName"].ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all assets.");
                throw;
            }

            return assets;
        }

        // ✅ Get Asset by ID
        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "SELECT AssetId, DeviceId, AssetName FROM Assets WHERE AssetId = @AssetId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId;

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Asset
                    {
                        AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                        DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                        AssetName = reader["AssetName"].ToString() ?? string.Empty
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching asset with ID {AssetId}", assetId);
                throw;
            }
        }

        // ✅ Add new Asset
        public async Task<int> AddAssetAsync(Asset asset)
        {
            try
            {
                // Check if referenced Device exists
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to add asset with invalid DeviceId {DeviceId}", asset.DeviceId);
                    throw new ApplicationException($"Cannot add Asset. Device with ID {asset.DeviceId} does not exist.");
                }

                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO Assets (DeviceId, AssetName)
                    OUTPUT INSERTED.AssetId
                    VALUES (@DeviceId, @AssetName);";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                command.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value =
                    asset.AssetName ?? (object)DBNull.Value;

                var newId = await command.ExecuteScalarAsync();
                _logger.LogInformation("Asset added successfully with ID {AssetId}", newId);

                return Convert.ToInt32(newId);
            }
            catch (ApplicationException)
            {
                throw; // Custom validation exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding asset: {@Asset}", asset);
                throw;
            }
        }

        // ✅ Update existing Asset
        public async Task UpdateAssetAsync(Asset asset)
        {
            try
            {
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to update asset {AssetId} with invalid DeviceId {DeviceId}",
                        asset.AssetId, asset.DeviceId);
                    throw new ApplicationException($"Cannot update asset. Device with ID {asset.DeviceId} does not exist.");
                }

                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    UPDATE Assets 
                    SET DeviceId = @DeviceId, AssetName = @AssetName
                    WHERE AssetId = @AssetId;";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = asset.AssetId;
                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = asset.DeviceId;
                command.Parameters.Add("@AssetName", SqlDbType.NVarChar, 200).Value =
                    asset.AssetName ?? (object)DBNull.Value;

                var rows = await command.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No asset found with ID {AssetId} to update.", asset.AssetId);
                else
                    _logger.LogInformation("Asset with ID {AssetId} updated successfully.", asset.AssetId);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset: {@Asset}", asset);
                throw;
            }
        }

        // ✅ Delete Asset
        public async Task DeleteAssetAsync(int assetId)
        {
            try
            {
                await using var connection = _db.GetConnection();
                await connection.OpenAsync();

                const string query = "DELETE FROM Assets WHERE AssetId = @AssetId";
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId;

                var rows = await command.ExecuteNonQueryAsync();

                if (rows == 0)
                    _logger.LogWarning("No asset found with ID {AssetId} to delete.", assetId);
                else
                    _logger.LogInformation("Asset with ID {AssetId} deleted successfully.", assetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset with ID {AssetId}", assetId);
                throw;
            }
        }
    }
}
