using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Generic;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Services.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class ServicesHostBuilder
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton<IExcelService, ExcelService>();


                services.AddSingleton<BaseInterface<Article>, ArticleService>();
                services.AddSingleton<IArticleService, ArticleService>();

                services.AddSingleton<BaseInterface<Storage>, StorageService>();
                services.AddSingleton<IStorageService, StorageService>();

                services.AddSingleton<BaseInterface<Supplier>, SupplierService>();
                services.AddSingleton<ISupplierService, SupplierService>();


                services.AddSingleton<BaseInterface<InventoryItemBasis>, InventoryItemBasisService>();
                services.AddSingleton<IInventoryItemBasisService, InventoryItemBasisService>();


                services.AddSingleton<BaseInterface<Good>, GoodService>();
                services.AddSingleton<IGoodService, GoodService>();


                services.AddSingleton<BaseInterface<InventoryDocument>, InventoryDocumentsService>();
                services.AddSingleton<IInventoryDocumentsService, InventoryDocumentsService>();


            });

            return host;
        }
    }
}
