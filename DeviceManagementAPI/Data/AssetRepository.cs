using System.Data.SqlClient;
using DeviceManagementAPI.Models;
using Microsoft.Extensions.Configuration;
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

        public List<Asset> GetAllAssets()
        {
            List<Asset> assets = new();
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("SELECT * FROM Assets", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                assets.Add(new Asset
                {
                    AssetId = (int)reader["AssetId"],
                    DeviceId = (int)reader["DeviceId"],
                    AssetName = reader["AssetName"].ToString()
                });
            }
            return assets;
        }

        public void AddAsset(Asset asset)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("INSERT INTO Assets (DeviceId, AssetName) VALUES (@DeviceId, @AssetName)", con);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateAsset(Asset asset)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("UPDATE Assets SET DeviceId=@DeviceId, AssetName=@AssetName WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteAsset(int assetId)
        {
            using SqlConnection con = new(_connectionString);
            SqlCommand cmd = new("DELETE FROM Assets WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", assetId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
