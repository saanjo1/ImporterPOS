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
        private ObservableCollection<WriteOffViewModel> writeOffVM;

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


        [RelayCommand]
        public async void WriteOffFromExcel()
        {
            try
            {
                ExcelFile = _excelService.OpenDialog().Result;
                Guid _storageId = _storageDataService.GetStorageByName("Glavno skladište").Result;

                if (ExcelFile != null)
                {
                    IsLoading = true;
                    await Task.Run(() =>
                    {
                        List<string> listOfSheets = _excelService.GetListOfSheets(ExcelFile).Result;
                        if (listOfSheets.Count > 0)
                            SelectedSheet = listOfSheets[0];

                        WriteOffViewModel vm = new WriteOffViewModel();

                        WriteOffVM = _excelService.ReadFromWriteOff(ExcelFile, SelectedSheet).Result;
                        InventoryDocument invDocument = new InventoryDocument
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            IsDeleted = false,
                            StorageId = _storageId,
                            SupplierId = null,
                            Type = 2
                        };

                        foreach (var item in WriteOffVM)
                        {
                            _inventoryService.Create(invDocument);

                            string name = item.Item + " " + item.Color_number + " " + item.Item_size;
                          
                            Good? _good = _articleService.GetGoodFromArticleByName(name).Result;

                            if (_good.Id != Guid.Empty)
                            {
                                InventoryItemBasis inventoryItemBasis = new InventoryItemBasis
                                {
                                    Id = Guid.NewGuid(),
                                    StorageId = _storageId,
                                    Created = DateTime.Now,
                                    Quantity = 0 - Helpers.Extensions.GetDecimal(item.Quantity),
                                    CurrentQuantity = 0 - Helpers.Extensions.GetDecimal(item.Quantity),
                                    Tax = 0,
                                    Discriminator = "InventoryDocumentItem",
                                    InventoryDocumentId = invDocument.Id,
                                    GoodId = _good.Id,
                                    Price = _good.LatestPrice,
                                    Total = Helpers.Extensions.GetDecimal(item.Quantity) * _good.LatestPrice,
                                    IsDeleted = false,
                                    Refuse = 0
                                };

                                _invItemService.Create(inventoryItemBasis);
                                countedGoods++;
                            }

                        }
                    });
                    IsLoading = false;
                    _notifier.ShowInformation("Successfully updated " + CountedGoods + " goods.");
                }
                else
                {
                    _notifier.ShowWarning("Please provide Excel file first.");
                }


            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }


        }
    }
}

