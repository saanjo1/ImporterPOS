using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryItems;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    public partial class InventoryDocumentsDetails
    {
        private readonly IInventoryItemBasisService _invItemsService;
        private readonly IArticleService _articleService;

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
        public ICollection<InventoryDocumentsDetails> listOfItems;

        [ObservableProperty]
        private ICollectionView inventoryItemsCollection;

        public InventoryDocumentsDetails(string id, IInventoryItemBasisService invItemsService, IArticleService articleService)
        {
            _invItemsService = invItemsService;
            _articleService = articleService;
            LoadInventoryItemBases(id);
        }

        public InventoryDocumentsDetails()
        {
            
        }

        private void LoadInventoryItemBases(string id)
        {
            ICollection<InventoryItemBasis> invItemBases = _invItemsService.GetItemsByInventoryId(id).Result;

            ListOfItems = new ObservableCollection<InventoryDocumentsDetails>();

            foreach (var invitemBases in invItemBases.OrderBy(x => x.Created))
            {
                var article = _articleService.GetPriceByGood(invitemBases.GoodId).Result;

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

            inventoryItemsCollection = CollectionViewSource.GetDefaultView(ListOfItems);
        }
    }
}
