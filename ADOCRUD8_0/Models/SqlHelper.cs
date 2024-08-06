using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace ADOCRUD8_0.Helpers
{
    public class SqlHelper
    {
        private readonly IConfiguration _configuration;

        public SqlHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
                return connection;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // For example: Logger.LogError(ex, "Error while creating SQL connection");
                throw new Exception("Error while creating SQL connection: " + ex.Message, ex);
            }
        }
    }
}
