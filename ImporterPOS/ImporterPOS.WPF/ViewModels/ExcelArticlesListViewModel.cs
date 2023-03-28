using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class ExcelArticlesListViewModel
    {
        private readonly Notifier _notifier;
        private readonly IExcelService _excelService;
        private readonly ConcurrentDictionary<string, string> _myDictionary;

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? category;

        [ObservableProperty]
        private string? supplier;

        [ObservableProperty]
        private string? storage;

        [ObservableProperty]
        private string? barCode;

        [ObservableProperty]
        private string? price;

        [ObservableProperty]
        private string? quantity;

        [ObservableProperty]
        private string? pricePerUnit;


    }
}
