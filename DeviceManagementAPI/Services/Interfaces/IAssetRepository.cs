using DeviceManagementAPI.Models;
using System.Collections.Generic; //provides IEnumerable<T> (for lists/collections).
using System.Threading.Tasks;     //allows asynchronous operations using Task.

namespace DeviceManagementAPI.Services.Interfaces
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
