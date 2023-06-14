using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Generic;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Rules;
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
                services.AddTransient<IExcelService, ExcelService>();


                services.AddTransient<BaseInterface<Article>, ArticleService>();
                services.AddTransient<IArticleService, ArticleService>();

                services.AddTransient<BaseInterface<Storage>, StorageService>();
                services.AddTransient<IStorageService, StorageService>();

                services.AddTransient<BaseInterface<Supplier>, SupplierService>();
                services.AddTransient<ISupplierService, SupplierService>();


                services.AddTransient<BaseInterface<InventoryItemBasis>, InventoryItemBasisService>();
                services.AddTransient<IInventoryItemBasisService, InventoryItemBasisService>();


                services.AddTransient<BaseInterface<Good>, GoodService>();
                services.AddTransient<IGoodService, GoodService>();

                services.AddTransient<BaseInterface<Rule>, RuleService>();
                services.AddTransient<IRuleService, RuleService>();


                services.AddTransient<BaseInterface<InventoryDocument>, InventoryDocumentsService>();
                services.AddTransient<IInventoryDocumentsService, InventoryDocumentsService>();


            });

            return host;
        }
    }
}
