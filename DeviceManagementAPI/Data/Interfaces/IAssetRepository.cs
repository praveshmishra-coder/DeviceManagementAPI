using DeviceManagementAPI.Models;
using System.Collections.Generic;

namespace DeviceManagementAPI.Data.Interfaces
{
    public interface IAssetRepository
    {
        List<Asset> GetAllAssets();
        void AddAsset(Asset asset);
        void UpdateAsset(Asset asset);
        void DeleteAsset(int assetId);
    }
}
