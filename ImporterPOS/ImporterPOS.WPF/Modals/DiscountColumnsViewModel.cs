using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class DiscountColumnsViewModel : BaseViewModel
    {
        private Notifier _notifier;
        public IExcelService? _excelDataService;
        private string _excelPath;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private bool isrbOption2Checked;

        [ObservableProperty]
        private bool isrbOption1Checked;


        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? category;

        [ObservableProperty]
        private string? storage;

        [ObservableProperty]
        private string? barCode;

        [ObservableProperty]
        private string? price;

        [ObservableProperty]
        private string? discount;

        [ObservableProperty]
        private decimal newPrice;

        [ObservableProperty]
        DiscountColumnsViewModel selectedItems;

        [ObservableProperty]
        ConcurrentDictionary<string, string> _myDictionary;

        [ObservableProperty]
        ObservableCollection<DiscountColumnsViewModel>? articleList;


        [ObservableProperty]
        List<string> columnNames;

        private DiscountViewModel _discountViewModel;

        public DiscountColumnsViewModel(DiscountViewModel discountViewModel, IExcelService? excelDataService, string excelPath, Notifier notifier)
        {
            _discountViewModel = discountViewModel;
            _excelDataService = excelDataService;
            _excelPath = excelPath;
            _notifier = notifier;
            LoadColumnNames();
        }

        public DiscountColumnsViewModel()
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
                ObservableCollection<DiscountColumnsViewModel>? excelDataList;
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
