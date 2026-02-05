using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Web.Configuration
{
    public class DatabaseSecrets

    {

        public string Server { get; set; }

        public string Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Port { get; set; } = 1433;

        /// <summary>

        /// Builds a SQL Server connection string from the secrets

        /// </summary>

        public string BuildConnectionString()

        {

            return $"Server={Server},{Port};Database={Database};User Id={Username};Password={Password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        }

        /// <summary>

        /// Builds connection string with integrated security (for local dev if needed)

        /// </summary>

        public string BuildIntegratedConnectionString()

        {

            return $"Server={Server},{Port};Database={Database};Integrated Security=True;Connection Timeout=30;";

        }

    }

}