using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Web.Infrastructure
{
    /// <summary>

    /// Retry policy with exponential backoff for transient failures

    /// Essential for production resilience

    /// </summary>

    public static class RetryPolicy

    {

        private static readonly ILogger Logger = LoggerFactory.Create("RetryPolicy");

        /// <summary>

        /// Executes an async operation with retry logic

        /// </summary>

        /// <typeparam name="T">Return type</typeparam>

        /// <param name="operation">The operation to execute</param>

        /// <param name="maxRetries">Maximum number of retries (default: 3)</param>

        /// <param name="initialDelayMs">Initial delay in milliseconds (default: 100)</param>

        /// <param name="operationName">Name of operation for logging</param>

        /// <returns>Result of the operation</returns>

        public static async Task<T> ExecuteWithRetryAsync<T>(

            Func<Task<T>> operation,

            int maxRetries = 3,

            int initialDelayMs = 100,

            string operationName = "Operation")

        {

            Exception lastException = null;

            var delay = initialDelayMs;

            for (int attempt = 1; attempt <= maxRetries + 1; attempt++)

            {

                try

                {

                    return await operation();

                }

                catch (Exception ex) when (IsTransientException(ex) && attempt <= maxRetries)

                {

                    lastException = ex;

                    Logger.Warn($"{operationName} failed (attempt {attempt}/{maxRetries + 1}): {ex.Message}. Retrying in {delay}ms...");

                    await Task.Delay(delay);

                    delay *= 2; // Exponential backoff

                }

                catch (Exception ex)

                {

                    Logger.Error($"{operationName} failed permanently: {ex.Message}", ex);

                    throw;

                }

            }

            throw lastException ?? new InvalidOperationException($"{operationName} failed after {maxRetries} retries");

        }

        /// <summary>

        /// Executes a synchronous operation with retry logic

        /// </summary>

        public static T ExecuteWithRetry<T>(

            Func<T> operation,

            int maxRetries = 3,

            int initialDelayMs = 100,

            string operationName = "Operation")

        {

            Exception lastException = null;

            var delay = initialDelayMs;

            for (int attempt = 1; attempt <= maxRetries + 1; attempt++)

            {

                try

                {

                    return operation();

                }

                catch (Exception ex) when (IsTransientException(ex) && attempt <= maxRetries)

                {

                    lastException = ex;

                    Logger.Warn($"{operationName} failed (attempt {attempt}/{maxRetries + 1}): {ex.Message}. Retrying in {delay}ms...");

                    Thread.Sleep(delay);

                    delay *= 2; // Exponential backoff

                }

                catch (Exception ex)

                {

                    Logger.Error($"{operationName} failed permanently: {ex.Message}", ex);

                    throw;

                }

            }

            throw lastException ?? new InvalidOperationException($"{operationName} failed after {maxRetries} retries");

        }

        /// <summary>

        /// Determines if an exception is transient (retryable)

        /// DNS resolution failures are NOT transient - they won't resolve by retrying

        /// </summary>

        private static bool IsTransientException(Exception ex)

        {

            // SQL transient errors

            if (ex is System.Data.SqlClient.SqlException sqlEx)

            {

                switch (sqlEx.Number)

                {

                    case -2:    // Timeout

                    case 20:    // The instance of SQL Server does not support encryption

                    case 64:    // Connection was successfully established, but then an error occurred

                    case 233:   // Connection initialization error

                    case 10053: // Software caused connection abort

                    case 10054: // Connection reset by peer

                    case 10060: // Connection timed out

                    case 40143: // Connection could not be initialized

                    case 40197: // Service error processing request

                    case 40501: // Service is busy

                    case 40613: // Database not currently available

                    case 49918: // Not enough resources to process request

                    case 49919: // Cannot process create or update request

                    case 49920: // Cannot process request due to too many operations

                        return true;

                }

            }

            // Network transient errors (but NOT DNS failures - those won't resolve by retrying)

            if (ex is System.Net.WebException webEx)

            {

                switch (webEx.Status)

                {

                    case System.Net.WebExceptionStatus.Timeout:

                    case System.Net.WebExceptionStatus.ConnectFailure:

                    case System.Net.WebExceptionStatus.ConnectionClosed:

                    case System.Net.WebExceptionStatus.KeepAliveFailure:

                    case System.Net.WebExceptionStatus.ReceiveFailure:

                    case System.Net.WebExceptionStatus.SendFailure:

                        return true;

                    // DNS failures are NOT transient - don't retry

                    case System.Net.WebExceptionStatus.NameResolutionFailure:

                        return false;

                }

            }

            // Check inner exception

            if (ex.InnerException != null)

            {

                return IsTransientException(ex.InnerException);

            }

            return false;

        }

    }

}