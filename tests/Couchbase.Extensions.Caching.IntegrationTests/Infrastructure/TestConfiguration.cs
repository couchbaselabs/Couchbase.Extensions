using System;
using System.Collections.Generic;
using System.IO;
using Couchbase.Configuration.Client;
using Microsoft.Extensions.Configuration;

namespace Couchbase.Extensions.Caching.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Provides the configurations defined in app.config.
    /// </summary>
    public static class TestConfiguration
    {
        private static IConfigurationRoot _jsonConfiguration;
        private static TestSettings _settings;

        public static TestSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    EnsureConfigurationLoaded();

                    _settings = new TestSettings();
                    _jsonConfiguration.GetSection("testSettings").Bind(_settings);
                }

                return _settings;
            }
        }

        public static ClientConfiguration GetDefaultConfiguration()
        {
            return new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    BuildBootStrapUrl()
                }
            };
        }

        /// <summary>
        /// Gets the configuration for the "current" appSettings setting. The hostname and port will be pulled from the appsettings as well.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">A configuration file could not be loaded.</exception>
        public static ClientConfiguration GetCurrentConfiguration()
        {
            return GetConfiguration(Settings.Current);
        }

        /// <summary>
        /// Gets the configuration for a config section. The hostname and port will be pulled from the appsettings.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">A configuration file could not be loaded.</exception>
        public static ClientConfiguration GetConfiguration(string sectionName)
        {
            EnsureConfigurationLoaded();

            var definition = new CouchbaseClientDefinition();
            _jsonConfiguration.GetSection("couchbase:" + sectionName).Bind(definition);

            var configuration = new ClientConfiguration(definition)
            {
                Servers = new List<Uri>
                {
                    BuildBootStrapUrl()
                }
            };

            return configuration;
        }

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("config.json");

            return builder.Build().GetSection("couchbase");
        }

        public static Uri BuildBootStrapUrl()
        {
            EnsureConfigurationLoaded();

            return new Uri(string.Format("http://{0}:{1}/", Settings.Hostname, Settings.BootPort));
        }

        private static void EnsureConfigurationLoaded()
        {
            if (_jsonConfiguration == null)
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("config.json");
                _jsonConfiguration = builder.Build();
            }
        }
    }
}
