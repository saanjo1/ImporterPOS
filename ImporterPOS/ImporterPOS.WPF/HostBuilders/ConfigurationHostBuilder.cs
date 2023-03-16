using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class ConfigurationHostBuilder
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            });

            return host;
        }
    }
}
