using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
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

        [ObservableProperty]
        private List<string> currentStorages;

        [ObservableProperty]
        private string selectedStorage;

        public Store(Notifier notifier, IArticleService articleService, IStorageService storageDataService, IInventoryDocumentsService inventoryService, IGoodService goodService, IInventoryItemBasisService invItemService)
        {
            _notifier = notifier;
            _articleService = articleService;
            _storageDataService = storageDataService;
            _inventoryService = inventoryService;
            _goodService = goodService;
            _invItemService = invItemService;
            LoadStorages();

        }

        private void LoadStorages()
        {
            CurrentStorages = new List<string>();
            CurrentStorages.Add("Glavno skladište");

            SelectedStorage = CurrentStorages[0];
        }

        [RelayCommand]
        public void EditCurrentDataGrid(object? parameter)
        {
            if (parameter is string)
            {
                string storeType = (string)parameter;
                switch (storeType)
                {
                    case "Glavno skladište":
                        this.CurrentDataGrid = new ArticleStorageViewModel(_articleService, storeType, _storageDataService, _notifier, _inventoryService, _goodService, _invItemService);
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
