using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using ImporterPOS.WPF.Helpers;
using ImporterPOS.WPF.HostBuilders;
using ImporterPOS.WPF.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Xml;

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
            UpdateConnectionString();

            _host = Host.CreateDefaultBuilder()
                .AddConfiguration()
                .AddDbContext()
                .AddServices()
                .AddViewModels()
                .AddViews()
                .Build();
        }

        private void UpdateConnectionString()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            int index = appDataPath.IndexOf("Roaming");
            appDataPath = appDataPath.Substring(0, index) + Informations.POSFolderPath;

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataPath);

            XmlNode dataSource = doc.SelectSingleNode(Informations.DataSourcePath);



            string json = File.ReadAllText("appsettings.json");
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(json);
            settings.ConnectionStrings.sqlstring = dataSource.InnerText + Informations.Encrypt;

            json = JsonConvert.SerializeObject(settings);
            File.WriteAllText("appsettings.json", json);
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

