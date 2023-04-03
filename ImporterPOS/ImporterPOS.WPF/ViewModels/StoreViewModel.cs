using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.States;
using ToastNotifications;

namespace ImporterPOS.WPF.ViewModels
{

    [ObservableObject]
    public partial class StoreViewModel : BaseViewModel
    {
        private Notifier _notifier;

        public IStore Store { get; set; }


        public StoreViewModel(IArticleService articleService, Notifier notifier, IStorageService storageDataService, IInventoryDocumentsService _inventoryService, IGoodService goodService, IInventoryItemBasisService itemBasisService)
        {
            _notifier = notifier;
            Store = new Store(_notifier, articleService, storageDataService, _inventoryService, goodService, itemBasisService);
        }



    }
}