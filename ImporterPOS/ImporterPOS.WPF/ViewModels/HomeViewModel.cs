using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Resources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ModalControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class HomeViewModel : BaseViewModel
    {

        private readonly IInventoryDocumentsService _invService;
        private readonly IInventoryItemBasisService _invItemService;
        private readonly ISupplierService _supplierDataService;
        private readonly IArticleService _articleService;


        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isShowInventoryDetails;

        [ObservableProperty]
        private ObservableCollection<InventoryDocumentsViewModel> listOfInventories;

        [ObservableProperty]
        private InventoryDocumentsDetails inventoryDocumentDetails;


        [ObservableProperty]
        private ICollectionView inventoryCollection;


        public HomeViewModel(IInventoryDocumentsService invService, ISupplierService supplierDataService, IArticleService articleService, IInventoryItemBasisService invItemService)
        {
            _invService = invService;
            _supplierDataService = supplierDataService;
            _articleService = articleService;
            this._invItemService = invItemService;
            Title = Translations.InventoryDocuments;
            LoadInventoryDocuments();
        }

        private async void LoadInventoryDocuments()
        {
            IsLoading = true;

            await Task.Run(() =>
            {

                ICollection<InventoryDocument> inventoryDocuments = _invService.GetAll().Result;

                ListOfInventories = new ObservableCollection<InventoryDocumentsViewModel>();

                foreach (var inventoryDocument in inventoryDocuments.OrderBy(x => x.Created))
                {
                    var soldPrice = _articleService.GetTotalSellingPrice(inventoryDocument).Result;
                    var basePrice = _articleService.GetTotalBasePrices(inventoryDocument).Result;
                    var purchasePrice = (decimal)GetTotalIncome(inventoryDocument);
                    var taxes = soldPrice - basePrice;

                    ListOfInventories.Add(new InventoryDocumentsViewModel
                    {
                        DateCreated = inventoryDocument.Created.ToString("dd.MM.yyyy hh:mm"),
                        Name = _supplierDataService.Get(inventoryDocument.SupplierId.ToString()).Result != null ? _supplierDataService.Get(inventoryDocument.SupplierId.ToString()).Result.Name : "Otpis robe",
                        PurchasePrice = purchasePrice.ToString() + " EUR",
                        SoldPrice = soldPrice.ToString() + " EUR",
                        BasePrice = basePrice.ToString() + " EUR",
                        Taxes = taxes.ToString() + " EUR",
                        Id = inventoryDocument.Id.ToString(),
                        Ruc = (basePrice - purchasePrice).ToString() + " EUR"
                    });
                }

                InventoryCollection = CollectionViewSource.GetDefaultView(ListOfInventories);
            });

            IsLoading = false;
        }

        [RelayCommand]
        public void ShowInventoryDetails(InventoryDocumentsViewModel parameter)
        {
            IsShowInventoryDetails = true;
            this.InventoryDocumentDetails = new InventoryDocumentsDetails(parameter.Id, _invItemService, _articleService, _invService, _supplierDataService, this);
        }


        [RelayCommand]
        private void ExportAsExcel()
        {
            // stvori novi SaveFileDialog
            var dialog = new Microsoft.Win32.SaveFileDialog();

            // postavi filter za odabir datoteke na Excel datoteke
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.FileName = "Spisak-inventurnih-dokumenata-" + DateTime.Now.ToString("ddMMyyyyhhmm");


            // prikaži dijalog za odabir mjesta za spremanje datoteke i dohvatimo rezultat
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                // kreiramo novu Excel datoteku i postavimo ime datoteke
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("InventoryDocuments");

                    worksheet.Column(1).Width = 20;
                    worksheet.Columns(2, 7).Width = 15;

                    var headerRow = worksheet.Row(1);
                    headerRow.Style.Font.Bold = true;


                    // postavimo zaglavlje tablice
                    worksheet.Cell(1, 1).Value = "Datum i vrijeme";
                    worksheet.Cell(1, 2).Value = "Dokument";
                    worksheet.Cell(1, 3).Value = "Ulazna cijena";
                    worksheet.Cell(1, 4).Value = "Izlazna cijena";
                    worksheet.Cell(1, 5).Value = "Iznos osnovice";
                    worksheet.Cell(1, 6).Value = "Iznos poreza";
                    worksheet.Cell(1, 7).Value = "RUC";

                    // postavimo vrijednosti u tablicu
                    int row = 2;
                    foreach (var document in listOfInventories)
                    {
                        worksheet.Cell(row, 1).Value = document.DateCreated;
                        worksheet.Cell(row, 2).Value = document.Name;
                        worksheet.Cell(row, 3).Value = document.PurchasePrice;
                        worksheet.Cell(row, 4).Value = document.SoldPrice;
                        worksheet.Cell(row, 5).Value = document.BasePrice;
                        worksheet.Cell(row, 6).Value = document.Taxes;
                        worksheet.Cell(row, 7).Value = document.Ruc;
                        row++;
                    }

                    // spremimo datoteku na odabrano mjesto
                    workbook.SaveAs(dialog.FileName);
                }
            }
        }


        [RelayCommand]
        private void ExportAsPdf()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDialog.FileName = "Spisak-inventurnih-dokumenata-" + DateTime.Now.ToString("ddMMyyyyhhmm");
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveDialog.ShowDialog() == true)
                {
                    Document pdfDocument = new Document();

                    PdfWriter.GetInstance(pdfDocument, new FileStream(saveDialog.FileName, FileMode.Create));

                    pdfDocument.Open();

                    //Define additional info
                    Paragraph header = new Paragraph();
                    header.SpacingAfter = 20f; // 10pt spacing after the paragraph
                    header.Add(new Chunk("Lista svih inventurnih dokumenata na dan " + DateTime.Now.ToString("dd.MM.yyyy. hh:mm"), new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    pdfDocument.Add(header);


                    List<string> columns = new List<string>() { "Datum i vrijeme", "Nabavljac", "Ulazna cijena", "Izlazna cijena", "Iznos osnovica", "Iznos poreza", "Razlika u cijeni" };

                    pdfDocument = Helpers.Extensions.UpdatePdfDocument(pdfDocument, columns, listOfInventories);
                }
            }
            catch
            {
                
                throw;
            }

        }
        public decimal? GetTotalIncome(InventoryDocument inventoryDocument)
        {
            return _invService.GetTotalInventoryItems(inventoryDocument.Id.ToString()).Result;
        }

        [RelayCommand]
        public void Cancel()
        {
            if(IsShowInventoryDetails)
                IsShowInventoryDetails = false;
        }
    }
}

