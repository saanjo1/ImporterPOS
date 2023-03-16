using ImporterPOS.Domain.EF;
using ImporterPOS.WPF.HostBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImporterPOS.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static IHost _host { get; private set; }


        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddConfiguration()
                .AddDbContext()
                .AddViewModels()
                .AddViews()
                .Build();
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await _host.StartAsync();

            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await dbContext.Database.MigrateAsync();
            }

            Window window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host!.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

    }

}

