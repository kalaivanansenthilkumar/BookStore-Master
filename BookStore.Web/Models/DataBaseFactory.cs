using BookStore.Web.Configuration;
using BookStore.Web.Infrastructure;
using Google.Cloud.SecretManager.V1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

namespace BookStore.Web.Models
{
    public class DataBaseFactory : IDisposable
    {
        private readonly IGcpSecretService _secretService;
        private readonly ILogger _logger;
        private string _connectionString;
        private readonly object _lockObject = new object();
 
        public DataBaseFactory(IGcpSecretService secretService)
        {
            _secretService = secretService ?? throw new ArgumentNullException(nameof(secretService));
            _logger = LoggerFactory.Create<DataBaseFactory>();
        }
 
        /// <summary>
        /// Gets connection string from GCP secrets (cached)
        /// </summary>
        private string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    lock (_lockObject)
                    {
                        if (string.IsNullOrEmpty(_connectionString))
                        {
                            _logger.Info("Building EF connection string from GCP secrets...");
                            var secrets = _secretService.GetDatabaseSecrets();
                            //_connectionString = BuildConnectionString(secrets);
                            _connectionString = BuildConnectionStringFromRemoteGCP();
                        }
                    }
                }
                return _connectionString;
            }
        }
 
        private string BuildConnectionString(DatabaseSecrets secrets)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = $"{secrets.Server},{secrets.Port}",
                InitialCatalog = secrets.Database,
                UserID = secrets.Username,
                Password = secrets.Password,
                Encrypt = true,
                TrustServerCertificate = false,
                Pooling = true,
                MinPoolSize = 5,
                MaxPoolSize = 100,
                ConnectTimeout = 30,
                ApplicationName = "BookStore API (EF)"
            };
            return builder.ConnectionString;
        }
        private string BuildConnectionStringFromRemoteGCP()
        {
            string connectionString = "";

            try
            {
                // Replace with your GCP project ID and secret name
                string projectId = Environment.GetEnvironmentVariable("GCP_PROJECT_ID") ?? "prj-unicr-dev-01";
                string secretId = "db-credentials"; // Example secret name
                string versionId = "latest"; // Can also be a specific version number

                // Retrieve secret value
                string secretValue = AccessSecretVersion(projectId, secretId, versionId);

                // Example: secretValue is JSON like:
                // { "username": "dbuser", "password": "dbpass", "host": "127.0.0.1", "database": "mydb" }
                var dbConfig = JsonSerializer.Deserialize<DbCredentials>(secretValue);

                // Build connection string dynamically
                 connectionString = $"Server={dbConfig.Host};Database={dbConfig.Database};User Id={dbConfig.Username};Password={dbConfig.Password};";

                Console.WriteLine("✅ Connection string built successfully (not printing for security).");
                return connectionString;
                // Use connectionString with your DB client here...
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error: {ex.Message}");
            }
            return connectionString;
        }
         
            // Function to access secret from GSM
    static string AccessSecretVersion(string projectId, string secretId, string versionId)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, versionId);

        AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

        return result.Payload.Data.ToStringUtf8();
    }
    public BookStoreDBContext CreateContext()
    {
        return new BookStoreDBContext(ConnectionString);
    }
    public void Dispose()
    {
        _logger.Debug("BookRepositoryEF disposed");
    }
  }
}
public class DbCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public string Database { get; set; }
}
