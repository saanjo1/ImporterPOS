using ImporterPOS.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.HostBuilders
{
    public static class ViewModelsHostBuilder
    {
        public static IHostBuilder AddViewModels(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<StoreViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<ArticlesViewModel>();
                services.AddTransient<DiscountViewModel>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<SettingsViewModel>();
            });

            return host;

        }
    }
}
