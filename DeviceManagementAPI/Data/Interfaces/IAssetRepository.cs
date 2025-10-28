using DeviceManagementAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAssetsAsync();
        Task<Asset?> GetAssetByIdAsync(int assetId);
        Task<int> AddAssetAsync(Asset asset);
        Task UpdateAssetAsync(Asset asset);
        Task DeleteAssetAsync(int assetId);
    }
}
