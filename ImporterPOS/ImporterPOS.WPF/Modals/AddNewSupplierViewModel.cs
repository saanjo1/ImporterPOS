using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models1;
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
    public partial class AddNewSupplierViewModel
    {
        private readonly Notifier _notifier;
        private readonly ISupplierService _supplierService;
        private  SettingsViewModel _settings;
        private readonly string filePath;

        [ObservableProperty]
        private string supplierName;

        [ObservableProperty]
        private string vatNumber;

        [ObservableProperty]
        private string address;

        public AddNewSupplierViewModel(ISupplierService supplierService, Notifier notifier, SettingsViewModel settings)
        {
            _supplierService = supplierService;
            _settings = settings;
            _notifier = notifier;
        }

        [RelayCommand]
        public void ClosePopUpWithoutSaving()
        {
            _settings.ClosePopUp(false);
        }


        [RelayCommand]
        public void Save()
        {
            try
            {
                if(SupplierName != null) {
                    _supplierService.Create(new Supplier
                    {
                        Id = Guid.NewGuid(),
                        Name = SupplierName,
                        VatNumber = VatNumber,
                        Address = Address,
                        IsDeleted = false
                    });

                    _notifier.ShowSuccess(Translations.Success);
                    _settings.ClosePopUp(true);
                }
                else
                {
                    _notifier.ShowInformation(Translations.SupplierNameRequired);
                }
            }
            catch 
            {
                _notifier.ShowInformation(Translations.ErrorMessage);
            }
        }
    }
}
