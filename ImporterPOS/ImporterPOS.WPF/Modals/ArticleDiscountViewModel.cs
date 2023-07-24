using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class ArticleDiscountViewModel : BaseViewModel
    {
        private Notifier _notifier;
        public IExcelService? _excelDataService;

        [ObservableProperty]
        private string? name;


        [ObservableProperty]
        private string? barCode;

        [ObservableProperty]
        private string? price;

        [ObservableProperty]
        private string? discount;

        [ObservableProperty]
        private decimal newPrice;

        [ObservableProperty]
        ArticleDiscountViewModel selectedItems;

        [ObservableProperty]
        ConcurrentDictionary<string, string> _myDictionary;

        [ObservableProperty]
        ObservableCollection<ArticleDiscountViewModel>? articleList;


        [ObservableProperty]
        List<string> columnNames;

        private DiscountViewModel _discountViewModel;

        public ArticleDiscountViewModel(DiscountViewModel discountViewModel, IExcelService? excelDataService, Notifier notifier)
        {
            _discountViewModel = discountViewModel;
            _excelDataService = excelDataService;
            _notifier = notifier;
            LoadColumnNames();
        }

        public ArticleDiscountViewModel()
        {

        }

        [RelayCommand]
        public void CloseModal()
        {
            _discountViewModel.Close();

        }

        [RelayCommand]
        public void Submit()
        {
            try
            {   
                ObservableCollection<ArticleDiscountViewModel>? excelDataList;
                excelDataList = _excelDataService.ReadDiscountColumns(null, this).Result;
                _discountViewModel.LoadData(excelDataList);
            }
            catch (Exception)
            {
                _notifier.ShowError("Please check your input and try again.");
            }
        }


        public void LoadColumnNames()
        {
            ColumnNames = _excelDataService.ListColumnNames("Sheet1").Result;
        }
    }
}
