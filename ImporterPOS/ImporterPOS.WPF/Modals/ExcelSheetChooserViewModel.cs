using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing;
using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.States;
using ImporterPOS.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;
using System.IO;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class ExcelSheetChooserViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private readonly IExcelService _excelService;
        private readonly ArticlesViewModel articlesViewModel;
        private readonly DiscountViewModel discountViewModel;
        private readonly string filePath;

        [ObservableProperty]
        private string fileName;

        public ExcelSheetChooserViewModel(IExcelService excelService, ArticlesViewModel _articlesViewModel, Notifier notifier, string _filePath)
        {
            _excelService = excelService;
            articlesViewModel = _articlesViewModel;
            _notifier = notifier;
            filePath = _filePath;
            LoadSheet(filePath);
        }

        public ExcelSheetChooserViewModel(IExcelService excelService, DiscountViewModel _discountViewModel, Notifier notifier, string _filePath)
        {
            _excelService = excelService;
            discountViewModel = _discountViewModel;
            _notifier = notifier;
            filePath = _filePath;
            LoadSheet(filePath);
        }

        [ObservableProperty]
        private List<string> currentSheets;

        [ObservableProperty]
        private string selectedSheet;


        public void LoadSheet(string excelFile)
        {
            try
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(excelFile);
                CurrentSheets = _excelService.GetListOfSheets(excelFile).Result;
            }
            catch
            {
                _notifier.ShowError(Translations.SelectSheetError);
                Cancel();
            }
        }


        [RelayCommand]
        public void Cancel()
        {
            SelectedSheet = null;
            if(articlesViewModel != null)
                articlesViewModel.Cancel();

            if (discountViewModel != null)
                discountViewModel.Close();
        }


        [RelayCommand]
        public void Save()
        {
            if(SelectedSheet != null)
            {
                if (articlesViewModel != null)
                {
                    articlesViewModel.LoadData(filePath);
                    articlesViewModel.Cancel();
                }
                else if(discountViewModel != null)
                {
                    discountViewModel.LoadData(filePath);
                    discountViewModel.Close();
                }
            }
            else
                _notifier.ShowError(Translations.SelectSheetError);


        }
    }
}

