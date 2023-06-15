using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2019.Drawing.Diagram11;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.States
{
    public partial class Store : ObservableObject, IStore
    {
        [ObservableProperty]
        private BaseViewModel? currentDataGrid;

        private Notifier _notifier;
        private readonly IArticleService _articleService;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageDataService;
        private readonly IInventoryItemBasisService _invItemService;
        private readonly IGoodService _goodService;
        private readonly IInventoryDocumentsService _inventoryService;

        [ObservableProperty]
        private List<string> currentStorages;


        [ObservableProperty]
        private string selectedStorage;

        [ObservableProperty]
        private string excelFile;

        [ObservableProperty]
        private string selectedSheet;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int countedGoods;
        public Store(Notifier notifier, IArticleService articleService, IStorageService storageDataService, IInventoryDocumentsService inventoryService, IGoodService goodService, IInventoryItemBasisService invItemService, IExcelService excelService)
        {
            _notifier = notifier;
            _articleService = articleService;
            _storageDataService = storageDataService;
            _inventoryService = inventoryService;
            _goodService = goodService;
            _invItemService = invItemService;
            _excelService = excelService;
            LoadStorages();
        }

        private void LoadStorages()
        {
            this.CurrentDataGrid = new ArticleStorageViewModel(_articleService, _storageDataService, _notifier, _inventoryService, _goodService, _invItemService);
        }


    }
}

