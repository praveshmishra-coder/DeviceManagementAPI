using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DeviceManagementAPI.Data
{
    public class AssetRepository : IAssetRepository
    {
        private readonly DatabaseHelper _db;
        public AssetRepository(DatabaseHelper db) => _db = db;

        public IEnumerable<Asset> GetAllAssets()
        {
            var list = new List<Asset>();
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Assets", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Asset
                {
                    AssetId = (int)reader["AssetId"],
                    DeviceId = (int)reader["DeviceId"],
                    AssetName = reader["AssetName"].ToString()
                });
            }
            return list;
        }

        public Asset GetAssetById(int assetId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Assets WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", assetId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Asset
                {
                    AssetId = (int)reader["AssetId"],
                    DeviceId = (int)reader["DeviceId"],
                    AssetName = reader["AssetName"].ToString()
                };
            }
            return null;
        }

        public void AddAsset(Asset asset)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("INSERT INTO Assets (DeviceId, AssetName) VALUES (@DeviceId, @AssetName)", con);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAsset(Asset asset)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand(
                "UPDATE Assets SET DeviceId=@DeviceId, AssetName=@AssetName WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@DeviceId", asset.DeviceId);
            cmd.Parameters.AddWithValue("@AssetName", asset.AssetName);
            cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAsset(int assetId)
        {
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM Assets WHERE AssetId=@AssetId", con);
            cmd.Parameters.AddWithValue("@AssetId", assetId);
            cmd.ExecuteNonQuery();
        }
    }
}
