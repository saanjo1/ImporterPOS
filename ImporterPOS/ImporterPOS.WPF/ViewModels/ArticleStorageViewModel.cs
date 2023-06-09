using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.Helpers;
using ImporterPOS.WPF.Modals;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Messages;
using ImporterPOS.WPF.Resources;
using ImporterPOS.Domain.Models1;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class ArticleStorageViewModel : BaseViewModel
    {
        private readonly IArticleService _articleService;
        private readonly IStorageService _storageService;
        private readonly IInventoryDocumentsService _inventoryService;
        private readonly IInventoryItemBasisService _invItemService;
        private readonly IGoodService _goodService;
        private Notifier _notifier;

        public ArticleStorageViewModel(IArticleService articleService, IStorageService storageService, Notifier notifier, IInventoryDocumentsService inventoryService, IGoodService goodService, IInventoryItemBasisService invItemService)
        {
            _articleService = articleService;
            _storageService = storageService;
            _notifier = notifier;
            _inventoryService = inventoryService;
            _goodService = goodService;
            _invItemService = invItemService;
            LoadData();
        }

        [ObservableProperty]
        private string storageName;

        [ObservableProperty]
        private int count;

        [ObservableProperty]
        private bool isEditOpen;

        [ObservableProperty]
        private bool isEnabled;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private StorageItemsViewModel editArticleViewModel;

        [ObservableProperty]
        private ICollection<GoodsArticlesViewModel> articleList;

        [ObservableProperty]
        private ICollectionView articleCollection;

        private InventoryDocument inventoryDocument;

        [ObservableProperty]
        private List<InventoryItemBasis> listOfItems = new List<InventoryItemBasis>();

        private string textToFilter;

        public string TextToFilter
        {
            get { return textToFilter; }
            set
            {
                textToFilter = value;
                OnPropertyChanged(nameof(TextToFilter));
                ArticleCollection.Filter = FilterFunction;
            }
        }

        private bool FilterFunction(object obj)
        {
            if (!string.IsNullOrEmpty(TextToFilter))
            {
                var filt = obj as GoodsArticlesViewModel;
                return filt != null && (filt.Name.Contains(TextToFilter));
            }
            return true;
        }

        [RelayCommand]
        public async void LoadData()
        {
            IsLoading = true;

            await Task.Run(() =>
            {
                ArticleList = StorageQuantityCounter(Informations.Storage).Result;
                ArticleCollection = CollectionViewSource.GetDefaultView(ArticleList);
                Count = ArticleList.Count;
            });

            IsLoading = false;
        }

        [RelayCommand]
        public void Cancel()
        {
            if (IsEditOpen)
                IsEditOpen = false;
        }

        [RelayCommand]
        public void EditArticle(GoodsArticlesViewModel parameter)
        {
            IsEditOpen = true;
            this.EditArticleViewModel = new StorageItemsViewModel(parameter, _notifier, _storageService, inventoryDocument, _invItemService, this);
        }


        [RelayCommand]
        public void MultipleEdit()
        {
            IsEnabled = true;
            inventoryDocument = new InventoryDocument
            {
                Created = DateTime.Now,
                Order = _inventoryService.GetInventoryOrderNumber().Result,
                Id = Guid.NewGuid(),
                StorageId = _storageService.GetStorageByName(StorageName).Result,
                SupplierId = null,
                Type = 2,
                IsActivated = true,
                IsDeleted = false
            };
            _inventoryService.Create(inventoryDocument);
        }

        [RelayCommand]
        public void SaveChanges()
        {

            int counter = 0;
            if (ListOfItems.Count > 0)
            {
                foreach (var item in ListOfItems)
                {
                    counter++;
                }
            }

            _notifier.ShowSuccess(counter + " corrections successfully saved.");
            ListOfItems.Clear();
            LoadData();
            IsEnabled = false;

        }

        [RelayCommand]
        public void DiscardChanges()
        {

            if (inventoryDocument == null)
            {
                _notifier.ShowInformation("Nothing to discard.");

            }
            else
            {
                try
                {
                    if (ListOfItems.Count == 1)
                    {
                        var inventoryItemId = ListOfItems[0].Id;

                        _invItemService.Delete((Guid)inventoryItemId);

                    }
                    else if (ListOfItems.Count > 1)
                    {
                        var inventoryItemId = ListOfItems[0].Id;

                        foreach (var item in ListOfItems)
                        {
                            _invItemService.Delete(item.Id);

                        }
                    }

                    _inventoryService.Delete(inventoryDocument.Id);
                    IsEnabled = false;
                    _notifier.ShowSuccess("Successfully deleted!");
                }
                catch (Exception)
                {
                    _notifier.ShowError("An error occurred. Please try again.");
                    throw;
                }
            }


        }

        public async Task<ICollection<GoodsArticlesViewModel>> StorageQuantityCounter(string storageName)
        {
            ICollection<Good> goods = await _goodService.GetAll();
            Guid storage = _storageService.GetStorageByName(storageName).Result;
            ICollection<GoodsArticlesViewModel> tempList = new List<GoodsArticlesViewModel>();
            foreach (var item in goods)
            {
                decimal quantity = _goodService.SumQuantityOfGoodsById(item.Id, storage).Result;
                decimal _totalPurchasePrice = Math.Round((item.LatestPrice * quantity), 2);
                decimal _soldPrice = _articleService.GetPriceByGood(item.Id).Result.Price * quantity;
                decimal _totalTaxes = Math.Round(_soldPrice * 0.25m, 2);
                decimal _totalBase = Math.Round(_soldPrice - _totalTaxes, 2);
                decimal _ruc = _totalBase - _totalPurchasePrice;
                if (quantity > 0)
                {
                    tempList.Add(new GoodsArticlesViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name,
                        GoodId = _goodService.GetGoodByName(item.Name, false).Result,
                        Quantity = quantity,
                        Storage = storage,
                        TotalPurchasePrice = _totalPurchasePrice,
                        TotalSoldPrice = Math.Round(_soldPrice, 2),
                        TotalBasePrice = _totalBase,
                        TotalTaxes = _totalTaxes,
                        Ruc = _ruc
                    });

                }
            }

            return await Task.FromResult(tempList);
        }

        //staviti u helper da se izbjegne ponavljanje
        [RelayCommand]
        public void ExportData()
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveDialog.FileName = "Stanje skladista - " + DateTime.Now.ToString("yyyyMMdd");
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveDialog.ShowDialog() == true)
            {
                using (Document pdfDocument = new Document())
                {
                    PdfWriter.GetInstance(pdfDocument, new FileStream(saveDialog.FileName, FileMode.Create));

                    pdfDocument.Open();

                    //Define additional info
                    Paragraph header = new Paragraph();
                    header.SpacingAfter = 20f; // 10pt spacing after the paragraph
                    header.Add(new Chunk("Stanje skladista - " + DateTime.Now.ToString("dd.MM.yyyy"), new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    pdfDocument.Add(header);

                    // Define table columns
                    PdfPTable table = new PdfPTable(7);
                    Font font = new Font(Font.FontFamily.TIMES_ROMAN, 8, Font.NORMAL);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f });

                    PdfPCell nameHeader = new PdfPCell(new Phrase("Artikal", font));
                    nameHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    nameHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(nameHeader);

                    PdfPCell quantityHeader = new PdfPCell(new Phrase("Kolicina", font));
                    quantityHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    quantityHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(quantityHeader);

                    PdfPCell purchasePriceCellHeader = new PdfPCell(new Phrase("Ulazna cijena", font));
                    purchasePriceCellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    purchasePriceCellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(purchasePriceCellHeader);

                    PdfPCell soldPriceCellHeader = new PdfPCell(new Phrase("Izlazna cijena", font));
                    soldPriceCellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    soldPriceCellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(soldPriceCellHeader);

                    PdfPCell basePriceHeader = new PdfPCell(new Phrase("Osnovica", font));
                    basePriceHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    basePriceHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(basePriceHeader);

                    PdfPCell taxesHeader = new PdfPCell(new Phrase("Porez", font));
                    taxesHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    taxesHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(taxesHeader);

                    PdfPCell rucHeader = new PdfPCell(new Phrase("RUC", font));
                    rucHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                    rucHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(rucHeader);

                    foreach (var doc in articleList)
                    {
                        PdfPCell idCell = new PdfPCell(new Phrase(doc.Name.ToString(), font));
                        idCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(idCell);

                        PdfPCell quantityCell = new PdfPCell(new Phrase(doc.Quantity.ToString(), font));
                        quantityCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(quantityCell);


                        PdfPCell purchasePriceCell = new PdfPCell(new Phrase(doc.TotalPurchasePrice.ToString() + " kn", font));
                        purchasePriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(purchasePriceCell);

                        PdfPCell soldPriceCell = new PdfPCell(new Phrase(doc.TotalSoldPrice.ToString() + " kn", font));
                        soldPriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(soldPriceCell);

                        PdfPCell basePriceCell = new PdfPCell(new Phrase(doc.TotalBasePrice.ToString() + " kn", font));
                        basePriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(basePriceCell);


                        PdfPCell taxesCell = new PdfPCell(new Phrase(doc.TotalTaxes.ToString() + " kn", font));
                        taxesCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(taxesCell);


                        PdfPCell rucCell = new PdfPCell(new Phrase(doc.Ruc.ToString() + " kn", font));
                        rucCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(rucCell);

                    }


                    PdfPCell totalCell = new PdfPCell(new Phrase("TOTAL", font));
                    totalCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    totalCell.Colspan = 2;

                    PdfPCell sumCell = new PdfPCell(new Phrase(articleList.Sum(a => a.Quantity).ToString(), font));
                    sumCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell purchasePriceTotalCell = new PdfPCell(new Phrase(articleList.Sum(a => a.TotalPurchasePrice).ToString() + " kn", font));
                    purchasePriceTotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell soldPriceTotalCell = new PdfPCell(new Phrase(articleList.Sum(a => a.TotalSoldPrice).ToString() + " kn", font));
                    soldPriceTotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell basePriceTotalCell = new PdfPCell(new Phrase(articleList.Sum(a => a.TotalBasePrice).ToString() + " kn", font));
                    basePriceTotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell taxesTotalCell = new PdfPCell(new Phrase(articleList.Sum(a => a.TotalTaxes).ToString() + " kn", font));
                    taxesTotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell rucTotalCell = new PdfPCell(new Phrase(articleList.Sum(a => a.Ruc).ToString() + " kn", font));
                    rucTotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    table.AddCell(totalCell);
                    table.AddCell(sumCell);
                    table.AddCell(purchasePriceTotalCell);
                    table.AddCell(soldPriceTotalCell);
                    table.AddCell(basePriceTotalCell);
                    table.AddCell(taxesTotalCell);
                    table.AddCell(rucTotalCell);
                    pdfDocument.Add(table);
                    // Add space for signature and stamp
                    Paragraph signature = new Paragraph();
                    signature.SpacingBefore = 100f;
                    signature.Add(new Chunk("Potpis: _____________________________      Pecat: _____________________________"));

                    pdfDocument.Add(signature);
                    pdfDocument.Close();
                }
            }
        }

    }
}