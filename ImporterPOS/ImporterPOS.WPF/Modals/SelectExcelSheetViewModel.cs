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
    public partial class SelectExcelSheetViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private readonly IExcelService _excelService;
        private readonly ArticlesViewModel articlesViewModel;
        private readonly Store _store;
        private readonly string filePath;

        [ObservableProperty]
        private string fileName;

        public SelectExcelSheetViewModel(IExcelService excelService, ArticlesViewModel _articlesViewModel, Notifier notifier, string _filePath)
        {
            _excelService = excelService;
            articlesViewModel = _articlesViewModel;
            _notifier = notifier;
            filePath = _filePath;
            LoadSheet(filePath);
        }

        public SelectExcelSheetViewModel(IExcelService excelService, Store store, Notifier notifier, string excelFile)
        {
            _excelService = excelService;
            _store = store;
            _notifier = notifier;
            LoadSheet(excelFile);
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
        }


        [RelayCommand]
        public void Save()
        {
            if(SelectedSheet != null)
            {
                articlesViewModel.LoadData(filePath);
                articlesViewModel.Cancel();
            }
            else
                _notifier.ShowError(Translations.SelectSheetError);
        }
    }
}

