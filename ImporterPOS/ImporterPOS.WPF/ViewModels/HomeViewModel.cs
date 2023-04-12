using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class HomeViewModel : BaseViewModel
    {

        private readonly IInventoryDocumentsService _invService;
        private readonly ISupplierService _supplierDataService;
        private readonly IArticleService _articleService;


        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isShowDetailsOpen;

        [ObservableProperty]
        private ObservableCollection<InventoryDocumentsViewModel> listOfInventories;

        //[ObservableProperty]
        //private InventoryDocumentsDetails inventoryDocumentDetails;


        [ObservableProperty]
        private ICollectionView inventoryCollection;


        public HomeViewModel(IInventoryDocumentsService invService, ISupplierService supplierDataService, IArticleService articleService)
        {
            _invService = invService;
            _supplierDataService = supplierDataService;
            _articleService = articleService;
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

                foreach (var inventoryDocument in inventoryDocuments.Where(x => x.SupplierId != null).OrderBy(x => x.Created))
                {
                    var soldPrice = _articleService.GetTotalSellingPrice(inventoryDocument).Result;
                    var basePrice = _articleService.GetTotalBasePrices(inventoryDocument).Result;
                    var purchasePrice = (decimal)GetTotalIncome(inventoryDocument);
                    var taxes = soldPrice - basePrice;

                    ListOfInventories.Add(new InventoryDocumentsViewModel
                    {
                        DateCreated = inventoryDocument.Created.ToString("dd.MM.yyyy hh:mm"),
                        Name = _supplierDataService.Get(inventoryDocument.SupplierId.ToString()).Result != null ? _supplierDataService.Get(inventoryDocument.SupplierId.ToString()).Result.Name : "Otpis robe",
                        PurchasePrice = purchasePrice,
                        SoldPrice = soldPrice,
                        BasePrice = basePrice,
                        Taxes = taxes,
                        Ruc = basePrice - purchasePrice
                    });
                }

                InventoryCollection = CollectionViewSource.GetDefaultView(ListOfInventories);
            });

            IsLoading = false;
        }

        [RelayCommand]
        public void ShowInventoryDetails(InventoryDocumentsViewModel parameter)
        {
            //IsShowDetailsOpen = true;
            //this.InventoryDocumentDetails = new InventoryDocumentsDetails(parameter);
        }


        public decimal? GetTotalIncome(InventoryDocument inventoryDocument)
        {
            return _invService.GetTotalInventoryItems(inventoryDocument.Id.ToString()).Result;
        }
    }
}

