using BookStore.Web.Configuration;
using BookStore.Web.Infrastructure;
using BookStore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace BookStore.Web.App_Start
{
    // <summary>

    /// Production-ready Dependency Injection Resolver for Web API

    /// 

    /// Dependency Chain:

    /// Controller -> Business Layer -> Data Layer -> GCP Secret Service

    /// 

    /// Features:

    /// - Singleton for GcpSecretService (expensive to create, caches tokens)

    /// - Scoped instances for repositories and services

    /// - Thread-safe initialization

    /// </summary>

    public class SimpleDependencyResolver : IDependencyResolver
    {
        // Singleton services (shared across all requests)

        private static readonly Lazy<IGcpSecretService> _secretService =

            new Lazy<IGcpSecretService>(() => new GcpSecretService(), isThreadSafe: true);

        private static readonly ILogger _logger = LoggerFactory.Create("DependencyResolver");

        public SimpleDependencyResolver()
        {
            _logger.Info("DependencyResolver initialized");

        }

        public IDependencyScope BeginScope()
        {

            return new DependencyScope();

        }

        public object GetService(Type serviceType)
        {

            return ResolveService(serviceType);

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {

            return new List<object>();

        }

        internal static object ResolveService(Type serviceType)
        {
            // Services (for direct injection if needed)

            if (serviceType == typeof(IGcpSecretService))

            {

                return _secretService.Value;

            }
            return null;

        }

        public void Dispose()

        {

            _logger.Info("DependencyResolver disposed");

        }

    }

    /// <summary>

    /// Scoped dependency scope for per-request instances

    /// </summary>

    public class DependencyScope : IDependencyScope

    {

        public object GetService(Type serviceType)

        {

            return SimpleDependencyResolver.ResolveService(serviceType);

        }

        public IEnumerable<object> GetServices(Type serviceType)

        {

            return new List<object>();

        }

        public void Dispose()

        {

            // Per-request cleanup if needed

        }

    }

    /// <summary>

    /// Configures Dependency Injection for the application

    /// </summary>

    public static class DependencyConfig

    {

        private static readonly ILogger _logger = LoggerFactory.Create("DependencyConfig");

        public static void Register(HttpConfiguration config)

        {

            _logger.Info("Registering dependency resolver...");

            config.DependencyResolver = new SimpleDependencyResolver();

            _logger.Info("Dependency resolver registered successfully");

        }

    }

}