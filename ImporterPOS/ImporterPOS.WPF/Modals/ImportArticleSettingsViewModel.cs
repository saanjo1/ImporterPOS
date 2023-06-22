using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class ImportArticleSettingsViewModel
    {

        [ObservableProperty]
        public bool isConnectChecked;

        private readonly Notifier _notifier;
        private readonly ArticlesViewModel _articleViewModel;


        public ImportArticleSettingsViewModel(Notifier notifier, ArticlesViewModel articleViewModel)
        {
            _notifier = notifier;
            _articleViewModel = articleViewModel;
            Load();
        }


        public void Load()
        {
            try
            {
                // Provjeri postojanje datoteke supplierAndStorageData.json
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (data.ContainsKey("ConnectGoodsToArticles") && bool.TryParse(data["ConnectGoodsToArticles"].ToString(), out bool connectGoodsToArticles))
                    {
                        IsConnectChecked = connectGoodsToArticles;
                    }
                    
                }
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
            _articleViewModel.CloseSettingsPopUp();
        }

        [RelayCommand]
        public async Task Save()
        {
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                Dictionary<string, object> data;

                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                }
                else
                {
                    data = new Dictionary<string, object>();
                }

                // Ažurirajte ili dodajte vrijednost za "ConnectGoodsToArticles"
                data["ConnectGoodsToArticles"] = IsConnectChecked.ToString();

                string updatedJson = JsonSerializer.Serialize(data);
                await File.WriteAllTextAsync(filePath, updatedJson);

                _notifier.ShowSuccess(Translations.Success);
                _articleViewModel.CloseSettingsPopUp();
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
        }
    }
}
