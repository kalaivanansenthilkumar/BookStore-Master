using BookStore.Web.Infrastructure;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Web.Configuration
{
        /// <summary>

        /// Service to retrieve secrets from GCP Secret Manager using REST API

        /// 

        /// SECURE CREDENTIAL FLOW:

        /// ========================

        /// 1. LOCAL DEV: Use USE_LOCAL_DEV_SECRETS=true (no GCP credentials needed)

        /// 2. JENKINS CI/CD: Credentials injected from Jenkins Credentials Store via environment variables

        /// 3. GCP PRODUCTION: Uses Workload Identity (metadata server) - no credentials needed

        /// 

        /// PRODUCTION FEATURES:

        /// - Retry with exponential backoff

        /// - Logging for monitoring

        /// - Token caching and refresh

        /// - Thread-safe operations

        /// 

        /// NEVER store service account JSON files in the codebase or on disk!

        /// </summary>

        public interface IGcpSecretService

        {

            Task<string> GetSecretAsync(string secretName);

            Task<DatabaseSecrets> GetDatabaseSecretsAsync();

            string GetSecret(string secretName);

            DatabaseSecrets GetDatabaseSecrets();

            bool IsHealthy();

        }

        public class GcpSecretService : IGcpSecretService, IDisposable
        {

            private readonly string _projectId;

            private readonly string _environmentPrefix;

            private string _accessToken;

            private DateTime _tokenExpiry;

            private DatabaseSecrets _cachedSecrets;

            private readonly object _lockObject = new object();

            private bool _useLocalDevMode;  // Changed from readonly to allow fallback

            private readonly Infrastructure.ILogger _logger;

            private bool _isHealthy = true;

            private const string SecretManagerApiBaseUrl = "https://secretmanager.googleapis.com/v1";

            private const int MaxRetries = 3;

            private const int InitialRetryDelayMs = 100;

            public GcpSecretService()

            {

                _logger = LoggerFactory.Create<GcpSecretService>();

                // Try to get project ID, use default if it fails

                try

                {

                    _projectId = EnvironmentConfig.GcpProjectId;

                }

                catch (Exception ex)

                {

                    _logger.Warn($"Failed to get GCP Project ID: {ex.Message}. Using default value.");

                    _projectId = "prj-unicr-dev-01";

                }

                // Try to get environment prefix, use default if it fails

                try

                {

                    _environmentPrefix = EnvironmentConfig.SecretPrefix;

                }

                catch (Exception ex)

                {

                    _logger.Warn($"Failed to get Secret Prefix: {ex.Message}. Using default value.");

                    _environmentPrefix = "DEV";

                }

                // Check if local dev mode is explicitly enabled

                _useLocalDevMode = IsLocalDevModeEnabled();

                _logger.Info($"Initializing GcpSecretService for environment: {_environmentPrefix}");

                _logger.Info($"Project ID: {_projectId}");

                _logger.Info($"Local dev mode (explicit): {_useLocalDevMode}");

                // If not explicitly in local dev mode, try to connect to GCP

                if (!_useLocalDevMode)

                {

                    try

                    {

                        RefreshAccessToken();

                        _logger.Info("Successfully obtained GCP access token");

                    }

                    catch (Exception ex)

                    {

                        // If we can't get GCP token, automatically fall back to local dev mode

                        _logger.Warn($"Failed to initialize GCP access token: {ex.Message}");

                        _logger.Warn("Automatically enabling local development mode");

                        _useLocalDevMode = true;

                        _isHealthy = true; // Mark as healthy since we're using local mode

                    }

                }

                _logger.Info($"Final mode: {(_useLocalDevMode ? "LOCAL DEV" : "GCP")}");

            }

            /// <summary>
            /// Health check for monitoring
            /// </summary>
            public bool IsHealthy()
            {
                if (_useLocalDevMode) return true;
                return _isHealthy && !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;
            }

            private bool IsLocalDevModeEnabled()
            {
                var useLocalDev = Environment.GetEnvironmentVariable("USE_LOCAL_DEV_SECRETS");
                return string.Equals(useLocalDev, "true", StringComparison.OrdinalIgnoreCase);
            }

            private DatabaseSecrets GetLocalDevSecrets()
            {
                _logger.Debug("Returning local development secrets");
                return new DatabaseSecrets
                {
                    Server = "localhost",
                    Database = "BookStoreDB",
                    Username = "sa",
                    Password = "YourLocalPassword123!",
                    Port = 1433
                };
            }

            private string GetLocalDevSecret(string secretName)
            {
                switch (secretName.ToUpperInvariant())
                {
                    case "DB_SERVER": return "localhost";
                    case "DB_DATABASE": return "BookStoreDB";
                    case "DB_USERNAME": return "sa";
                    case "DB_PASSWORD": return "YourLocalPassword123!";
                    case "DB_PORT": return "1433";
                    default: return $"LOCAL_PLACEHOLDER_{secretName}";
                }
            }

            private void RefreshAccessToken()
            {
                _logger.Debug("Refreshing GCP access token...");
                // Don't use retry for GetAccessToken since it already handles fallbacks internally
                // and DNS failures shouldn't be retried
                _accessToken = GetAccessToken();
                _tokenExpiry = DateTime.UtcNow.AddMinutes(55);
                _isHealthy = true;
                _logger.Debug($"Access token refreshed, expires at {_tokenExpiry:HH:mm:ss}");
            }

            private void EnsureValidToken()
            {
                if (_useLocalDevMode) return;

                if (DateTime.UtcNow >= _tokenExpiry.AddMinutes(-5)) // Refresh 5 minutes before expiry
                {
                    lock (_lockObject)
                    {
                        if (DateTime.UtcNow >= _tokenExpiry.AddMinutes(-5))
                        {
                            _logger.Info("Access token expiring soon, refreshing...");
                            RefreshAccessToken();
                        }
                    }
                }
            }

            private string GetAccessToken()
            {
                // OPTION 1: GCP Metadata Server (Workload Identity)
                var metadataToken = GetTokenFromMetadataServer();
                if (!string.IsNullOrEmpty(metadataToken))
                {
                    _logger.Info("Using Workload Identity (metadata server) for authentication");
                    return metadataToken;
                }

                // OPTION 2: Service Account JSON from environment variable
                var serviceAccountJson = Environment.GetEnvironmentVariable("GCP_SERVICE_ACCOUNT_KEY_JSON");
                if (!string.IsNullOrEmpty(serviceAccountJson))
                {
                    _logger.Info("Using service account JSON from environment variable");
                    return GetTokenFromServiceAccountJson(serviceAccountJson);
                }

                // OPTION 3: File path (backward compatibility)
                var keyFilePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
                if (!string.IsNullOrEmpty(keyFilePath) && File.Exists(keyFilePath))
                {
                    _logger.Warn("Using file-based credentials (not recommended for production)");
                    return GetTokenFromServiceAccountKeyFile(keyFilePath);
                }

                throw new Infrastructure.ConfigurationException("GCP_CREDENTIALS",
                    "Unable to obtain GCP access token. Secure options:\n" +
                    "1. Run on GCP infrastructure with Workload Identity\n" +
                    "2. Set GCP_SERVICE_ACCOUNT_KEY_JSON environment variable\n" +
                    "3. Set USE_LOCAL_DEV_SECRETS=true for local development");
            }

            private string GetTokenFromMetadataServer()
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(
                        "http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token");
                    request.Headers.Add("Metadata-Flavor", "Google");
                    request.Timeout = 2000;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var json = reader.ReadToEnd();
                        var tokenResponse = JObject.Parse(json);
                        return tokenResponse["access_token"]?.ToString();
                    }
                }
                catch (WebException)
                {
                    return null; // Not on GCP infrastructure
                }
                catch (Exception)
                {
                    return null;
                }
            }

            private string GetTokenFromServiceAccountJson(string serviceAccountJson)
            {
                try
                {
                    var keyData = JObject.Parse(serviceAccountJson);
                    string clientEmail = keyData["client_email"]?.ToString();
                    string privateKey = keyData["private_key"]?.ToString();
                    string tokenUri = keyData["token_uri"]?.ToString() ?? "https://oauth2.googleapis.com/token";

                    if (string.IsNullOrEmpty(clientEmail) || string.IsNullOrEmpty(privateKey))
                    {
                        throw new Infrastructure.ConfigurationException("GCP_SERVICE_ACCOUNT_KEY_JSON",
                            "Invalid service account JSON: missing client_email or private_key");
                    }

                    var jwt = CreateJwt(clientEmail, privateKey);
                    return ExchangeJwtForAccessToken(jwt, tokenUri);
                }
                catch (JsonException ex)
                {
                    throw new Infrastructure.ConfigurationException("GCP_SERVICE_ACCOUNT_KEY_JSON",
                        $"Failed to parse service account JSON: {ex.Message}");
                }
            }

            private string GetTokenFromServiceAccountKeyFile(string keyFilePath)
            {
                var keyFileContent = File.ReadAllText(keyFilePath);
                return GetTokenFromServiceAccountJson(keyFileContent);
            }

            private string ExchangeJwtForAccessToken(string jwt, string tokenUri)
            {
                var request = (HttpWebRequest)WebRequest.Create(tokenUri);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 30000; // 30 seconds

                var postData = $"grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={jwt}";
                var postBytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postBytes.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();
                    var tokenResponse = JObject.Parse(json);
                    return tokenResponse["access_token"]?.ToString();
                }
            }

            private string CreateJwt(string clientEmail, string privateKey)
            {
                var now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                var exp = now + 3600;

                var header = new { alg = "RS256", typ = "JWT" };
                var payload = new
                {
                    iss = clientEmail,
                    scope = "https://www.googleapis.com/auth/cloud-platform",
                    aud = "https://oauth2.googleapis.com/token",
                    iat = now,
                    exp = exp
                };

                var headerJson = JsonConvert.SerializeObject(header);
                var payloadJson = JsonConvert.SerializeObject(payload);

                var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
                var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

                var signatureInput = $"{headerBase64}.{payloadBase64}";
                var signature = SignWithRsa(signatureInput, privateKey);

                return $"{headerBase64}.{payloadBase64}.{signature}";
            }

            private string SignWithRsa(string input, string privateKeyPem)
            {
                var privateKeyBase64 = privateKeyPem
                    .Replace("-----BEGIN PRIVATE KEY-----", "")
                    .Replace("-----END PRIVATE KEY-----", "")
                    .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                    .Replace("-----END RSA PRIVATE KEY-----", "")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Trim();

                var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
                var rsaParams = ParsePkcs8PrivateKey(privateKeyBytes);

                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaParams);
                    var inputBytes = Encoding.UTF8.GetBytes(input);
                    var signatureBytes = rsa.SignData(inputBytes, new SHA256CryptoServiceProvider());
                    return Base64UrlEncode(signatureBytes);
                }
            }

            private RSAParameters ParsePkcs8PrivateKey(byte[] pkcs8)
            {
                using (var ms = new MemoryStream(pkcs8))
                using (var reader = new BinaryReader(ms))
                {
                    SkipAsn1Header(reader);
                    SkipAsn1Element(reader);
                    SkipAsn1Element(reader);
                    reader.ReadByte();
                    var length = ReadAsn1Length(reader);
                    var privateKeyData = reader.ReadBytes(length);
                    return ParseRsaPrivateKey(privateKeyData);
                }
            }


            private RSAParameters ParseRsaPrivateKey(byte[] rsaPrivateKey)

            {

                using (var ms = new MemoryStream(rsaPrivateKey))

                using (var reader = new BinaryReader(ms))

                {

                    SkipAsn1Header(reader);

                    SkipAsn1Element(reader);

                    return new RSAParameters

                    {

                        Modulus = ReadAsn1Integer(reader),

                        Exponent = ReadAsn1Integer(reader),

                        D = ReadAsn1Integer(reader),

                        P = ReadAsn1Integer(reader),

                        Q = ReadAsn1Integer(reader),

                        DP = ReadAsn1Integer(reader),

                        DQ = ReadAsn1Integer(reader),

                        InverseQ = ReadAsn1Integer(reader)

                    };

                }

            }

            private void SkipAsn1Header(BinaryReader reader) { reader.ReadByte(); ReadAsn1Length(reader); }

            private void SkipAsn1Element(BinaryReader reader) { reader.ReadByte(); reader.ReadBytes(ReadAsn1Length(reader)); }

            private int ReadAsn1Length(BinaryReader reader)

            {

                var length = (int)reader.ReadByte();

                if ((length & 0x80) != 0)

                {

                    var numBytes = length & 0x7F;

                    length = 0;

                    for (int i = 0; i < numBytes; i++)

                        length = (length << 8) | reader.ReadByte();

                }

                return length;

            }

            private byte[] ReadAsn1Integer(BinaryReader reader)

            {

                if (reader.ReadByte() != 0x02) throw new InvalidOperationException("Expected INTEGER tag");

                var data = reader.ReadBytes(ReadAsn1Length(reader));

                int start = 0;

                while (start < data.Length - 1 && data[start] == 0) start++;

                if (start > 0) { var trimmed = new byte[data.Length - start]; Array.Copy(data, start, trimmed, 0, trimmed.Length); return trimmed; }

                return data;

            }

            private string Base64UrlEncode(byte[] input) =>

                Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').TrimEnd('=');

            public async Task<string> GetSecretAsync(string secretName)

            {

                if (_useLocalDevMode)

                {

                    return await Task.FromResult(GetLocalDevSecret(secretName));

                }

                EnsureValidToken();

                var fullSecretName = $"{_environmentPrefix}_{secretName}";

                _logger.Debug($"Retrieving secret: {fullSecretName}");

                return await Infrastructure.RetryPolicy.ExecuteWithRetryAsync(

                    async () => await FetchSecretFromGcpAsync(fullSecretName),

                    maxRetries: MaxRetries,

                    initialDelayMs: InitialRetryDelayMs,

                    operationName: $"GetSecret({fullSecretName})");

            }

            private async Task<string> FetchSecretFromGcpAsync(string fullSecretName)

            {

                var url = $"{SecretManagerApiBaseUrl}/projects/{_projectId}/secrets/{fullSecretName}/versions/latest:access";

                try

                {

                    var request = (HttpWebRequest)WebRequest.Create(url);

                    request.Method = "GET";

                    request.Headers.Add("Authorization", $"Bearer {_accessToken}");

                    request.Accept = "application/json";

                    request.Timeout = 30000; // 30 seconds

                    using (var response = await Task.Factory.FromAsync(

                        request.BeginGetResponse,

                        request.EndGetResponse, null) as HttpWebResponse)

                    using (var reader = new StreamReader(response.GetResponseStream()))

                    {

                        var json = await reader.ReadToEndAsync();

                        var secretResponse = JObject.Parse(json);

                        string base64Data = secretResponse["payload"]?["data"]?.ToString();

                        if (string.IsNullOrEmpty(base64Data))

                        {

                            throw new SecretRetrievalException(fullSecretName, _environmentPrefix, "Secret payload is empty");

                        }

                        var secretBytes = Convert.FromBase64String(base64Data);

                        _logger.Debug($"Successfully retrieved secret: {fullSecretName}");

                        return Encoding.UTF8.GetString(secretBytes);

                    }

                }

                catch (WebException ex)

                {

                    _isHealthy = false;

                    var errorMessage = $"Failed to retrieve secret '{fullSecretName}'. ";

                    if (ex.Response is HttpWebResponse errorResponse)

                    {

                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))

                        {

                            errorMessage += $"Status: {errorResponse.StatusCode}, Details: {reader.ReadToEnd()}";

                        }

                    }

                    throw new SecretRetrievalException(fullSecretName, _environmentPrefix, errorMessage, ex);

                }

            }

            public string GetSecret(string secretName)

            {

                return GetSecretAsync(secretName).GetAwaiter().GetResult();

            }

            public async Task<DatabaseSecrets> GetDatabaseSecretsAsync()

            {

                if (_cachedSecrets != null)

                {

                    _logger.Debug("Returning cached database secrets");

                    return _cachedSecrets;

                }

                if (_useLocalDevMode)

                {

                    var localSecrets = GetLocalDevSecrets();

                    lock (_lockObject) { _cachedSecrets = localSecrets; }

                    return await Task.FromResult(localSecrets);

                }

                _logger.Info("Fetching database secrets from GCP Secret Manager...");

                var secrets = new DatabaseSecrets

                {

                    Server = await GetSecretAsync("DB_SERVER"),

                    Database = await GetSecretAsync("DB_DATABASE"),

                    Username = await GetSecretAsync("DB_USERNAME"),

                    Password = await GetSecretAsync("DB_PASSWORD")

                };

                try

                {

                    var portString = await GetSecretAsync("DB_PORT");

                    if (int.TryParse(portString, out int port))

                        secrets.Port = port;

                }

                catch

                {

                    secrets.Port = 1433;

                }

                lock (_lockObject) { _cachedSecrets = secrets; }

                _logger.Info("Successfully retrieved and cached database secrets");

                return secrets;

            }

            public DatabaseSecrets GetDatabaseSecrets()

            {

                return GetDatabaseSecretsAsync().GetAwaiter().GetResult();

            }

            public void Dispose()

            {

                _logger.Info("GcpSecretService disposed");

            }


        }
    }