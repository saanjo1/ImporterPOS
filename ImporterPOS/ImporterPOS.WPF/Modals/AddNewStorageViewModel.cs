using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class AddNewStorageViewModel
    {
        private readonly Notifier _notifier;
        private IStorageService _storageService;
        private SettingsViewModel _settings;
        private readonly string filePath;

        [ObservableProperty]
        private string storageName;


        public AddNewStorageViewModel (IStorageService storageService, Notifier notifier, SettingsViewModel settings)
        {
            _storageService = storageService;
            _settings = settings;
            _notifier = notifier;
        }

        [RelayCommand]
        public void ClosePopUpWithoutSaving()
        {
            _settings.ClosePopUp(false);
        }


        [RelayCommand]
        public void SaveStorage()
        {
            try
            {
                if (StorageName != null)
                {
                    _storageService.Create(new Storage
                    {
                        Id = Guid.NewGuid(),
                        Name = StorageName,
                        Deleted = false
                    });

                    _notifier.ShowSuccess(Translations.Success);
                    _settings.ClosePopUp(true);
                }
                else
                {
                    _notifier.ShowInformation(Translations.StorageNameRequired);
                }
            }
            catch
            {
                _notifier.ShowInformation(Translations.ErrorMessage);
            }
        }
    }
}
