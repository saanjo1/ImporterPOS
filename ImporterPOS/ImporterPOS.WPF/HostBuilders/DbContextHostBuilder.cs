using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class DbContextHostBuilder
    {
        public static IHostBuilder AddDbContext(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                Action<DbContextOptionsBuilder> configureDbContext = o => o.UseSqlServer(context.Configuration.GetConnectionString("sqlstring"));

                services.AddDbContext<DatabaseContext>(configureDbContext);
                services.AddSingleton(new DatabaseContextFactory(configureDbContext));
            });


            return hostBuilder;
        }
    }
}
