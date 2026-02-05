using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Web.Infrastructure
{
    public class SecretRetrievalException : Exception
    {
        public string SecretName { get; }
        public string Environment { get; }

        public SecretRetrievalException(string secretName, string environment, string message)
            : base(message)
        {
            SecretName = secretName;
            Environment = environment;
        }

        public SecretRetrievalException(string secretName, string environment, string message, Exception innerException)
            : base(message, innerException)
        {
            SecretName = secretName;
            Environment = environment;
        }
    }

    /// <summary>
    /// Custom exception for database connection failures
    /// </summary>
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message)
            : base(message)
        {
        }

        public DatabaseConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Custom exception for configuration errors
    /// </summary>
    public class ConfigurationException : Exception
    {
        public string ConfigurationKey { get; }

        public ConfigurationException(string configurationKey, string message)
            : base(message)
        {
            ConfigurationKey = configurationKey;
        }
    }

    /// <summary>
    /// Custom exception for validation errors
    /// </summary>
    public class ValidationException : Exception
    {
        public string FieldName { get; }

        public ValidationException(string fieldName, string message)
            : base(message)
        {
            FieldName = fieldName;
        }
    }

    /// <summary>
    /// Custom exception for entity not found
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; }
        public object EntityId { get; }

        public EntityNotFoundException(string entityType, object entityId)
            : base($"{entityType} with ID '{entityId}' was not found.")
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}