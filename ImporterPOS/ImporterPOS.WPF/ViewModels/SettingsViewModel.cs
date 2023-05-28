using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private IExcelService _excelService;
        private IArticleService _articleService;
        private IGoodService _goodService;
        private IInventoryDocumentsService _invService;
        private IInventoryItemBasisService _invItemService;

        [ObservableProperty]
        private string databaseConnection;

        [ObservableProperty]
        private string serverInstance;

        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private bool isOpen;



        public SettingsViewModel(Notifier notifier, IExcelService excelService, IArticleService articleService, IGoodService goodService, IInventoryDocumentsService invService, IInventoryItemBasisService invItemsService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _articleService = articleService;
            _goodService = goodService;
            _invService = invService;
            _invItemService = invItemsService;
            GetDatabaseInfo();
        }

        [RelayCommand]
        private async void StockCorrectionFromExcel()
        {
            ObservableCollection<StockCorrectionViewModel> stockCorrectionViewModels = new ObservableCollection<StockCorrectionViewModel>();

            try
            {
                string excelFile = _excelService.OpenDialog().Result;
                if (excelFile != null)
                {
                    stockCorrectionViewModels = _excelService.ReadStockCorrectionDocument(excelFile).Result;


                    InventoryDocument inventoryDocument = new InventoryDocument()
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.Now,
                        Order = _invService.GetInventoryOrderNumber().Result,
                        IsActivated = true,
                        IsDeleted = false,
                        StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                        Type = 2
                    };

                    _invService.Create(inventoryDocument);

                    foreach (var item in stockCorrectionViewModels)
                    {
                        Guid _good = _goodService.GetGoodByName(item.Name, true).Result;

                        decimal itemTotalPrice = Helpers.Extensions.GetDecimal(item.TotalPrice);
                        decimal itemNewQty = Helpers.Extensions.GetDecimal(item.NewQuantity);
                        decimal Qty = itemNewQty;




                        if (_good != Guid.Empty && Qty > 0)
                        {
                            Good goodEntity = await _goodService.Get(_good.ToString());

                            InventoryItemBasis inventoryItemBasis = new InventoryItemBasis
                            {
                                Id = Guid.NewGuid(),
                                StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                                Created = DateTime.Now,
                                Quantity = Qty,
                                CurrentQuantity = Qty,
                                Tax = 0,
                                Discriminator = "InventoryDocumentItem",
                                InventoryDocumentId = inventoryDocument.Id,
                                GoodId = goodEntity.Id,
                                Price = itemTotalPrice / Qty,
                                Total = itemTotalPrice,
                                IsDeleted = false,
                                Refuse = 0
                            };
                            _invItemService.Create(inventoryItemBasis);
                        }

                    }

                    _notifier.ShowSuccess(Translations.StockCorrectionDone);
                }
            }
            catch
            {
                _notifier.ShowError(Translations.ImportArticlesError);
                throw;
            }



        }

        [RelayCommand]
        private async void ReadBarcodeTxtFile()
        {
            ObservableCollection<StockCorrectionViewModel> stockCorrectionViewModels = new ObservableCollection<StockCorrectionViewModel>();

            try
            {
                string path = _excelService.OpenDialog().Result;

                if (path != null)
                {
                    stockCorrectionViewModels = _excelService.ReadFromTxtFile(path).Result;


                    InventoryDocument inventoryDocument = new InventoryDocument()
                    {
                        Id = Guid.NewGuid(),
                        Created = new DateTime(2023, 04, 05, 00, 30, 00),
                        Order = _invService.GetInventoryOrderNumber().Result,
                        IsActivated = true,
                        IsDeleted = false,
                        StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                        Type = 2
                    };

                    _invService.Create(inventoryDocument);

                    foreach (var item in stockCorrectionViewModels)
                    {

                        Guid _good = _goodService.GetGoodByName(item.Name, false).Result;

                        decimal itemCurrentQty = Helpers.Extensions.GetDecimal(item.TotalPrice);
                        decimal itemNewQty = Helpers.Extensions.GetDecimal(item.NewQuantity);
                        decimal Qty = itemNewQty - itemCurrentQty;

                        if (_good != Guid.Empty && Qty != 0)
                        {
                            Good goodEntity = await _goodService.Get(_good.ToString());

                            InventoryItemBasis inventoryItemBasis = new InventoryItemBasis
                            {
                                Id = Guid.NewGuid(),
                                StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                                Created = new DateTime(2023, 04, 05, 00, 30, 00),
                                Quantity = Qty,
                                CurrentQuantity = Qty,
                                Tax = 0,
                                Discriminator = "InventoryDocumentItem",
                                InventoryDocumentId = inventoryDocument.Id,
                                GoodId = goodEntity.Id,
                                Price = goodEntity.LatestPrice,
                                Total = Qty * goodEntity.LatestPrice,
                                IsDeleted = false,
                                Refuse = 0
                            };
                            _invItemService.Create(inventoryItemBasis);
                        }

                    }

                    _notifier.ShowSuccess(Translations.InventoryDone);
                }
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }


        }

        [RelayCommand]
        public async Task CreateGoodsBasedOnArticleName()
        {
            try
            {
                await _articleService.CreateGoodsBasedOnArticleName();
                _notifier.ShowError(Translations.Success);

            }
            catch 
            {
                _notifier.ShowError(Translations.ErrorMessage);
                
            }
        }

        [RelayCommand]
        public async Task SetMainStockToZero()
        {
            try
            {
                await _goodService.SetMainStockToZero();
                _notifier.ShowError(Translations.Success);
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                throw;
            }
        }


        public void GetDatabaseInfo()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            int index = appDataPath.IndexOf("Roaming");
            appDataPath = appDataPath.Substring(0, index) + Informations.POSFolderPath;

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataPath);

            XmlNode databaseNode = doc.SelectSingleNode(Informations.DatabasePath);
            XmlNode serverInstanceNode = doc.SelectSingleNode(Informations.ServerInstancePath);
            XmlNode portNode = doc.SelectSingleNode(Informations.PortPath);
            DatabaseConnection = databaseNode.InnerText;
            ServerInstance = serverInstanceNode.InnerText;
            Port = portNode.InnerText;
        }

    }
}
