using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Web.Infrastructure
{
    /// <summary>
    /// Simple logging interface for production monitoring
    /// Replace with your preferred logging framework (Serilog, NLog, log4net, Application Insights)
    /// </summary>
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception ex = null);
    }

    /// <summary>
    /// Console/Debug logger for development
    /// In production, replace with structured logging (Serilog, Application Insights, etc.)
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public ConsoleLogger(string categoryName = "BookStore")
        {
            _categoryName = categoryName;
        }

        public void Debug(string message)
        {
            Log("DEBUG", message);
        }

        public void Info(string message)
        {
            Log("INFO", message);
        }

        public void Warn(string message)
        {
            Log("WARN", message);
        }

        public void Error(string message, Exception ex = null)
        {
            Log("ERROR", message);
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{_categoryName}] EXCEPTION: {ex}");
            }
        }

        private void Log(string level, string message)
        {
            var logMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{_categoryName}] [{level}] {message}";
            System.Diagnostics.Debug.WriteLine(logMessage);
            // Also write to trace for production
            System.Diagnostics.Trace.WriteLine(logMessage);
        }
    }

    /// <summary>
    /// Logger factory
    /// </summary>
    public static class LoggerFactory
    {
        public static ILogger Create(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }

        public static ILogger Create<T>()
        {
            return new ConsoleLogger(typeof(T).Name);
        }
    }
}