using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Categories;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.RuleItems;
using ImporterPOS.Domain.Services.Rules;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.Domain.Services.Taxes;
using ImporterPOS.Domain.Services.Units;
using ImporterPOS.WPF.Services.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class ServicesHostBuilder
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<IExcelService, ExcelService>();
                services.AddTransient<IArticleService, ArticleService>();
                services.AddTransient<IStorageService, StorageService>();
                services.AddTransient<ISupplierService, SupplierService>();
                services.AddTransient<IInventoryItemBasisService, InventoryItemBasisService>();
                services.AddTransient<IGoodService, GoodService>();
                services.AddTransient<IRuleService, RuleService>();
                services.AddTransient<IInventoryDocumentsService, InventoryDocumentsService>();
                services.AddTransient<ICategoryService, CategoryService>();
                services.AddTransient<ISubCategoryService, SubCategoryService>();
                services.AddTransient<IUnitService, UnitService>();
                services.AddTransient<ITaxService, TaxService>();
                services.AddTransient<IRuleItemsService, RuleItemsService>();
            });

            return host;
        }
    }
}
