using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementAPI.Data
{
    public class DatabaseHelper
    {
        private readonly IConfiguration _config;
        public DatabaseHelper(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
    }
}
