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

                var soldPrice = article.Price * invitemBases.Quantity;
                var basePrice = Helpers.Extensions.GetBasePrice(soldPrice, 25);
                var purchasePrice = invitemBases.Total;
                var taxes = soldPrice - basePrice;
                var name = article.Name;

                listOfItems.Add(new InventoryDocumentsDetails
                {
                    Name = name,
                    Quantity = invitemBases.Quantity,
                    PurchasePrice = (decimal)purchasePrice,
                    Taxes = taxes,
                    BasePrice = basePrice,
                    SoldPrice = soldPrice
                });
            }

            inventoryItemsCollection = CollectionViewSource.GetDefaultView(ListOfItems);
        }
    }
}
