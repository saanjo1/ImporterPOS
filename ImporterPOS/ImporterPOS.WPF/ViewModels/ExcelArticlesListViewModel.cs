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
        private string? subCategoryName;

        [ObservableProperty]
        private string? articlePrice;

        [ObservableProperty]
        private string? totalPrice;

        [ObservableProperty]
        private string? quantity;

        [ObservableProperty]
        private string? unit;

        [ObservableProperty]
        private string? pricePerUnit;

        [ObservableProperty]
        private string? tax;


    }
}
