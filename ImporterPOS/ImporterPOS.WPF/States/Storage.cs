using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.ViewModels;
using ToastNotifications;

namespace ImporterPOS.WPF.States
{
    public partial class Storage : ObservableObject, IStorage
    {
        [ObservableProperty]
        private BaseViewModel? currentDataGrid;

        private Notifier _notifier;
        private readonly IArticleService _articleService;
        private readonly IStorageService _storageDataService;
        private readonly IInventoryDocumentsService _inventoryService;

        public Storage(Notifier notifier, IArticleService articleService, IStorageService storageDataService)
        {
            _notifier = notifier;
            _articleService = articleService;
            _storageDataService = storageDataService;
        }

        [RelayCommand]
        public void EditCurrentDataGrid(object? parameter)
        {
            if (parameter is StorageType)
            {
                StorageType storeType = (StorageType)parameter;
                switch (storeType)
                {
                    case StorageType.Articles:
                        this.CurrentDataGrid = new StoreViewModel(_articleService, _storageDataService, _notifier, _inventoryService);
                        break;
                    case StorageType.Economato:
                        this.CurrentDataGrid = new StoreViewModel(_articleService, _storageDataService, _notifier, _inventoryService);
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
