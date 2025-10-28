using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IAssetRepository
    {
        IEnumerable<Asset> GetAllAssets();
        Asset GetAssetById(int assetId);
        void AddAsset(Asset asset);
        void UpdateAsset(Asset asset);
        void DeleteAsset(int assetId);
    }
}
