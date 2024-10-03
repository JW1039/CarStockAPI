using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace OracleCMS_DealerAPI.Data
{
    /// <summary>
    /// Provides a context for creating a database connection using SQLite.
    /// </summary>
    public class DatabaseContext
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="configuration">The application's configuration for accessing connection strings.</param>
        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Creates and returns a new SQLite database connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> to the SQLite database.</returns>
        public IDbConnection CreateConnection()
        {
            // Retrieve the connection string from the configuration file
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Create and return a new SQLite connection
            return new SQLiteConnection(connectionString);
        }
    }
}
