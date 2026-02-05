using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BookStore.Web.Configuration
{
    /// <summary>

    /// Environment configuration that determines which GCP secrets to use

    /// 

    /// SECURE CONFIGURATION FLOW:

    /// ==========================

    /// 1. LOCAL DEV: Set USE_LOCAL_DEV_SECRETS=true (no GCP config needed)

    /// 2. JENKINS CI/CD: Environment variables injected from Jenkins Credentials Store

    /// 3. GCP PRODUCTION: Uses Workload Identity, GCP_PROJECT_ID from metadata

    /// 

    /// Environment variables (set by Jenkins or runtime):

    /// - APP_ENVIRONMENT: DEV, UAT, or PROD

    /// - GCP_PROJECT_ID: Your GCP project ID

    /// - GCP_SERVICE_ACCOUNT_KEY_JSON: Service account JSON content (injected by Jenkins)

    /// - USE_LOCAL_DEV_SECRETS: Set to "true" for local development without GCP

    /// </summary>

    public static class EnvironmentConfig

    {

        private const string DefaultProjectId = "prj-unicr-dev-01";

        private const string DefaultEnvironment = "DEV";

        public static string CurrentEnvironment

        {

            get

            {

                try

                {

                    // Priority: Environment variable (set by Jenkins) > App Setting > Default to DEV

                    var env = Environment.GetEnvironmentVariable("APP_ENVIRONMENT")

                        ?? ConfigurationManager.AppSettings["Environment"]

                        ?? DefaultEnvironment;

                    return env.ToUpperInvariant();

                }

                catch

                {

                    return DefaultEnvironment;

                }

            }

        }

        public static bool IsDevelopment => CurrentEnvironment == "DEV";

        public static bool IsUAT => CurrentEnvironment == "UAT";

        public static bool IsProduction => CurrentEnvironment == "PROD";

        /// <summary>

        /// Checks if running in local development mode (no GCP required)

        /// </summary>

        public static bool IsLocalDevMode

        {

            get

            {

                try

                {

                    var useLocalDev = Environment.GetEnvironmentVariable("USE_LOCAL_DEV_SECRETS");

                    return string.Equals(useLocalDev, "true", StringComparison.OrdinalIgnoreCase);

                }

                catch

                {

                    return false;

                }

            }

        }

        /// <summary>

        /// GCP Project ID - returns default value if not configured instead of throwing

        /// Priority: Environment variable > App Setting > GCP Metadata > Default

        /// </summary>

        public static string GcpProjectId

        {

            get

            {

                try

                {

                    // Skip GCP config if using local dev mode

                    if (IsLocalDevMode)

                    {

                        return "local-dev-project";

                    }

                    // 1. From environment variable (set by Jenkins)

                    var projectId = Environment.GetEnvironmentVariable("GCP_PROJECT_ID");

                    if (!string.IsNullOrEmpty(projectId))

                    {

                        return projectId;

                    }

                    // 2. From app settings (Web.config)

                    projectId = ConfigurationManager.AppSettings["GcpProjectId"];

                    if (!string.IsNullOrEmpty(projectId) && projectId != "prj-unicr-dev-01")

                    {

                        return projectId;

                    }

                    // 3. Try to get from GCP metadata (when running on GCP with Workload Identity)

                    projectId = GetProjectIdFromMetadata();

                    if (!string.IsNullOrEmpty(projectId))

                    {

                        return projectId;

                    }

                    // 4. Return default instead of throwing

                    return DefaultProjectId;

                }

                catch

                {

                    // If anything fails, return default

                    return DefaultProjectId;

                }

            }

        }

        /// <summary>

        /// Gets the secret name prefix based on environment

        /// Secrets in GCP should be named like: DEV_DB_SERVER, UAT_DB_SERVER, PROD_DB_SERVER

        /// </summary>

        public static string SecretPrefix

        {

            get

            {

                try

                {

                    return CurrentEnvironment;

                }

                catch

                {

                    return DefaultEnvironment;

                }

            }

        }

        /// <summary>

        /// Attempts to get project ID from GCP metadata server (for Workload Identity)

        /// </summary>

        private static string GetProjectIdFromMetadata()

        {

            try

            {

                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(

                    "http://metadata.google.internal/computeMetadata/v1/project/project-id");

                request.Headers.Add("Metadata-Flavor", "Google");

                request.Timeout = 2000;

                using (var response = (System.Net.HttpWebResponse)request.GetResponse())

                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))

                {

                    return reader.ReadToEnd().Trim();

                }

            }

            catch

            {

                return null;

            }

        }

    }

}