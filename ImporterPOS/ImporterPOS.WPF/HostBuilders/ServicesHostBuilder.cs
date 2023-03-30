using ImporterPOS.Domain.Services.Generic;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Services.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class ServicesHostBuilder
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton<IExcelService, ExcelService>();
                services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                services.AddScoped<ISupplierService, SupplierService>();
                services.AddScoped<IInventoryDocumentsService, InventoryDocumentsService>();
                services.AddScoped<IStorageService, StorageService>();
            });

            return host;
        }
    }
}
