using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.ViewModels;
using ToastNotifications;

namespace ImporterPOS.WPF.States
{
    public partial class Store : ObservableObject, IStore
    {
        [ObservableProperty]
        private BaseViewModel? currentDataGrid;

        private Notifier _notifier;
        private readonly IArticleService _articleService;
        private readonly IStorageService _storageDataService;
        private readonly IInventoryItemBasisService _invItemService;
        private readonly IGoodService _goodService;
        private readonly IInventoryDocumentsService _inventoryService;

        public Store(Notifier notifier, IArticleService articleService, IStorageService storageDataService, IInventoryDocumentsService inventoryService, IGoodService goodService, IInventoryItemBasisService invItemService)
        {
            _notifier = notifier;
            _articleService = articleService;
            _storageDataService = storageDataService;
            _inventoryService = inventoryService;
            _goodService = goodService;
            _invItemService = invItemService;
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
                        this.CurrentDataGrid = new ArticleStorageViewModel(_articleService, _storageDataService, _notifier, _inventoryService, _goodService, _invItemService);
                        break;
                    case StorageType.Economato:
                        this.CurrentDataGrid = new ArticleStorageViewModel(_articleService, _storageDataService, _notifier, _inventoryService, _goodService, _invItemService);
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
