using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Modals;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class InventoryDocumentsDetails
    {
        private readonly IInventoryItemBasisService _invItemsService;
        private readonly IArticleService _articleService;
        private readonly IInventoryDocumentsService _invService;
        private readonly ISupplierService _supplierService;
        private readonly HomeViewModel _viewModel;
        private readonly string inventoryId;

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public decimal purchasePrice;

        [ObservableProperty]
        public decimal soldPrice;

        [ObservableProperty]
        public decimal basePrice;

        [ObservableProperty]
        public decimal taxes;

        [ObservableProperty]
        public decimal ruc;

        [ObservableProperty]
        public decimal quantity;


        [ObservableProperty]
        public decimal totalPurchasePrice;

        [ObservableProperty]
        public decimal totalSoldPrice;

        [ObservableProperty]
        public decimal totalBasePrice;

        [ObservableProperty]
        public decimal totalTaxesPrice;

        [ObservableProperty]
        public decimal totalRuc;
        
        [ObservableProperty]
        public string inventoryDocumentTitle;

        [ObservableProperty]
        public ICollection<InventoryDocumentsDetails> listOfItems;

        [ObservableProperty]
        private ICollectionView inventoryItemsCollection;

        public InventoryDocumentsDetails(string id, IInventoryItemBasisService invItemsService, IArticleService articleService, IInventoryDocumentsService invService, ISupplierService supplierService, HomeViewModel viewModel)
        {
            inventoryId = id;
            _invItemsService = invItemsService;
            _articleService = articleService;
            _invService = invService;
            _supplierService = supplierService;
            _viewModel = viewModel;
            LoadInventoryItemBases(id);
        }

        public InventoryDocumentsDetails()
        {
            
        }
        private string textToFilter;

        public string TextToFilter
        {
            get { return textToFilter; }
            set
            {
                textToFilter = value;
                OnPropertyChanged(nameof(TextToFilter));
                InventoryItemsCollection.Filter = FilterFunction;
            }
        }

        private bool FilterFunction(object obj)
        {
            if (!string.IsNullOrEmpty(TextToFilter))
            {
                var filt = obj as InventoryDocumentsDetails;
                return filt != null && (filt.Name.Contains(TextToFilter, StringComparison.OrdinalIgnoreCase));
            }
            return true;
        }

        private void LoadInventoryItemBases(string id)
        {
            ICollection<InventoryItemBasis> invItemBases = _invItemsService.GetItemsByInventoryId(id).Result;

            ListOfItems = new ObservableCollection<InventoryDocumentsDetails>();

            foreach (var invitemBases in invItemBases.OrderBy(x => x.Created))
            {
                var article = _articleService.GetPriceByGood(invitemBases.GoodId).Result;

              if(article != null)
                {
                    var _soldPrice = article.Price * invitemBases.Quantity;
                    var _basePrice = Math.Round(Helpers.Extensions.GetBasePrice(_soldPrice, 25), 2);
                    var _purchasePrice = Math.Round((decimal)invitemBases.Total, 2);
                    var _taxes = _soldPrice - _basePrice;
                    var _ruc = Math.Round((_basePrice - _purchasePrice), 2);
                    var _name = article.Name;

                    listOfItems.Add(new InventoryDocumentsDetails
                    {
                        Name = _name,
                        Quantity = invitemBases.Quantity,
                        PurchasePrice = _purchasePrice,
                        Taxes = Math.Round(_taxes, 2),
                        BasePrice = _basePrice,
                        SoldPrice = Math.Round(_soldPrice, 2),
                        Ruc = _ruc
                    });

                    TotalPurchasePrice += _purchasePrice;
                    TotalSoldPrice += _soldPrice;
                    TotalTaxesPrice += _taxes;
                    TotalBasePrice += _basePrice;
                    TotalRuc += _ruc;
                }

                
            }

            inventoryItemsCollection = CollectionViewSource.GetDefaultView(ListOfItems);
        }

        [RelayCommand]
        private async void GeneratePdf(object parameter)
        {
            InventoryDocument invDocument = await _invService.Get(inventoryId);
            var supplier = await _supplierService.Get(invDocument.SupplierId.ToString());
            if (invDocument == null || supplier == null)
                return;

            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveDialog.FileName = "DetaljiDokumentaBr-" + invDocument.Order;
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
                    header.Add(new Chunk("Broj primke: " + invDocument.Order , new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    header.Add(new Chunk("\nDatum primke: " + invDocument.Created, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    header.Add(new Chunk("\nDobavljac: " + supplier.Name, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
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

                    foreach (var doc in ListOfItems)
                    {
                        PdfPCell idCell = new PdfPCell(new Phrase(doc.Name.ToString(), font));
                        idCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(idCell);

                        PdfPCell quantityCell = new PdfPCell(new Phrase(doc.Quantity.ToString(), font));
                        quantityCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(quantityCell);


                        PdfPCell purchasePriceCell = new PdfPCell(new Phrase(doc.PurchasePrice.ToString(), font));
                        purchasePriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(purchasePriceCell);

                        PdfPCell soldPriceCell = new PdfPCell(new Phrase(doc.SoldPrice.ToString(), font));
                        soldPriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(soldPriceCell);

                        PdfPCell basePriceCell = new PdfPCell(new Phrase(doc.BasePrice.ToString(), font));
                        basePriceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(basePriceCell);


                        PdfPCell taxesCell = new PdfPCell(new Phrase(doc.Taxes.ToString(), font));
                        taxesCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(taxesCell);


                        PdfPCell rucCell = new PdfPCell(new Phrase(doc.Ruc.ToString(), font));
                        rucCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(rucCell);


                    }
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

        [RelayCommand]
        private void Cancel()
        {
            _viewModel.Cancel();
        }
    }
}
