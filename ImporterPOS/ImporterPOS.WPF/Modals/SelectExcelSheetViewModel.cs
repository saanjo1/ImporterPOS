using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.States;
using ImporterPOS.WPF.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class SelectExcelSheetViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private readonly IExcelService _excelService;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly Store _store;
        private readonly ConcurrentDictionary<string, string> _myDictionary;

        public SelectExcelSheetViewModel(IExcelService excelService, SettingsViewModel settingsViewModel, Notifier notifier, ConcurrentDictionary<string, string> myDictionary)
        {
            _excelService = excelService;
            _settingsViewModel = settingsViewModel;
            _notifier = notifier;
            _myDictionary = myDictionary;
            LoadSheet();
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


        public void LoadSheet(string excelFile = "")
        {
           if(excelFile == "")
            {
                if (_myDictionary != null && _myDictionary.TryGetValue(Translations.CurrentExcelFile, out string value))
                {
                    CurrentSheets = _excelService.GetListOfSheets(value).Result;
                    SelectedSheet = CurrentSheets[0];
                }
                else
                {
                    _notifier.ShowError(Translations.SelectSheetError);
                    Cancel();
                }
            } else
            {
                CurrentSheets = _excelService.GetListOfSheets(excelFile).Result;
                SelectedSheet = CurrentSheets[0];
            }
        }


        [RelayCommand]
        public void Cancel()
        {
            SelectedSheet = null;
            if(_settingsViewModel != null) 
                _settingsViewModel.Cancel();
        }


        [RelayCommand]
        public void Save()
        {
            if (SelectedSheet != null)
            {
                if (_myDictionary.TryGetValue(Translations.CurrentExcelSheet, out string value1) == false)
                {
                    bool success = _myDictionary.TryAdd(Translations.CurrentExcelSheet, SelectedSheet);
                    if (success)
                    {
                        _notifier.ShowInformation(Translations.SelectSheetSuccess);
                        if(_settingsViewModel != null)
                            _settingsViewModel.SelectSheetSuccess = true;
                    }
                    else
                        _notifier.ShowError(Translations.SelectSheetError);
                }
                else
                {
                    _myDictionary.TryGetValue(Translations.CurrentExcelSheet, out string value);

                    bool success = _myDictionary.TryUpdate(Translations.CurrentExcelSheet, SelectedSheet, value);
                    if (success)
                    {
                        _notifier.ShowInformation(Translations.SelectSheetSuccess);
                        if (_settingsViewModel != null)
                            _settingsViewModel.SelectSheetSuccess = true;
                    }
                    else
                        _notifier.ShowError(Translations.SelectSheetError);
                }

            }
            else
            {
                _notifier.ShowError(Translations.SelectSheetError);
            }

            Cancel();
        }
    }
}

