using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.Services.Interfaces;

namespace DeviceManagementAPI.Services
{
    public class AssetRepository : IAssetRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<AssetRepository> _logger;

        public AssetRepository(SqlConnection connection, ILogger<AssetRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        // ✅ Check if a Device exists before adding/updating Asset
        private async Task<bool> DeviceExistsAsync(int deviceId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "SELECT COUNT(1) FROM Devices WHERE DeviceId = @DeviceId";
                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceId", deviceId);

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if device {DeviceId} exists", deviceId);
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // ✅ Get All Assets
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            var assets = new List<Asset>();

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "SELECT AssetId, DeviceId, AssetName FROM Assets";
                await using var command = new SqlCommand(query, _connection);
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
            finally
            {
                await _connection.CloseAsync();
            }

            return assets;
        }

        // ✅ Get Asset By ID
        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "SELECT AssetId, DeviceId, AssetName FROM Assets WHERE AssetId = @AssetId";
                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@AssetId", assetId);

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
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // ✅ Add New Asset
        public async Task<int> AddAssetAsync(Asset asset)
        {
            try
            {
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to add asset with invalid DeviceId {DeviceId}", asset.DeviceId);
                    throw new ApplicationException($"Device with ID {asset.DeviceId} does not exist.");
                }

                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = @"
                    INSERT INTO Assets (DeviceId, AssetName)
                    OUTPUT INSERTED.AssetId
                    VALUES (@DeviceId, @AssetName);";

                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
                command.Parameters.AddWithValue("@AssetName", asset.AssetName ?? (object)DBNull.Value);

                var newId = await command.ExecuteScalarAsync();
                _logger.LogInformation("Asset added successfully with ID {AssetId}", newId);

                return Convert.ToInt32(newId);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding asset: {@Asset}", asset);
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // ✅ Update Existing Asset
        public async Task UpdateAssetAsync(Asset asset)
        {
            try
            {
                if (!await DeviceExistsAsync(asset.DeviceId))
                {
                    _logger.LogWarning("Attempted to update asset {AssetId} with invalid DeviceId {DeviceId}", asset.AssetId, asset.DeviceId);
                    throw new ApplicationException($"Device with ID {asset.DeviceId} does not exist.");
                }

                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = @"
                    UPDATE Assets 
                    SET DeviceId = @DeviceId, AssetName = @AssetName
                    WHERE AssetId = @AssetId;";

                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@AssetId", asset.AssetId);
                command.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
                command.Parameters.AddWithValue("@AssetName", asset.AssetName ?? (object)DBNull.Value);

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
            finally
            {
                await _connection.CloseAsync();
            }
        }

        // ✅ Delete Asset
        public async Task DeleteAssetAsync(int assetId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                const string query = "DELETE FROM Assets WHERE AssetId = @AssetId";
                await using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@AssetId", assetId);

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
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
