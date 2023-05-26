using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.Helpers;
using ImporterPOS.WPF.ViewModels;
using System;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class EditStorageViewModel
    {

        private GoodsArticlesViewModel _goodsArticlesViewModel;
        private Notifier _notifier;
        private IStorageService _storageDataService;
        private IInventoryItemBasisService _inventoryService;
        private ArticleStorageViewModel viewModel;
        private InventoryDocument inventoryDocument;

        public EditStorageViewModel(GoodsArticlesViewModel goodsArticlesViewModel, Notifier notifier, IStorageService storageDataService, InventoryDocument inventoryDocument, IInventoryItemBasisService inventoryService)
        {
            _goodsArticlesViewModel = goodsArticlesViewModel;
            Name = _goodsArticlesViewModel.Name;
            Quantity = _goodsArticlesViewModel.Quantity;
            CurrentQuantity = _goodsArticlesViewModel.Quantity;
            LatestPrice = _goodsArticlesViewModel.TotalPurchasePrice;
            GoodId = _goodsArticlesViewModel.GoodId;
            Sttorage = _goodsArticlesViewModel.Storage;
            _notifier = notifier;
            _storageDataService = storageDataService;
            this.inventoryDocument = inventoryDocument;
            _inventoryService = inventoryService;
        }


        [ObservableProperty]
        private decimal? quantity;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal? currentQuantity;

        [ObservableProperty]
        private Guid sttorage;

        [ObservableProperty]
        private Guid goodId;

        [ObservableProperty]
        private decimal? latestPrice;



        [RelayCommand]
        public void Save()
        {
            if (Quantity == CurrentQuantity)
            {
                _notifier.ShowInformation("No changes applied.");
                viewModel.Cancel();
            }
            else
            {
                InventoryItemBasis newInventoryItem = new InventoryItemBasis()
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now,
                    Quantity = (decimal)Quantity - (decimal)CurrentQuantity,
                    IsDeleted = false,
                    Price = LatestPrice,
                    Tax = 0,
                    Total = ((decimal)Quantity - (decimal)CurrentQuantity) * LatestPrice,
                    Discriminator = "InventoryDocumentItem",
                    InventoryDocumentId = inventoryDocument.Id,
                    StorageId = inventoryDocument.StorageId,
                    GoodId = GoodId,
                    CurrentQuantity = (decimal)Quantity - (decimal)CurrentQuantity,
                };

                _inventoryService.Create(newInventoryItem);


                _notifier.ShowSuccess("Quantity updated!");
                viewModel.Cancel();
                viewModel.LoadData();

                viewModel.ListOfItems.Add(newInventoryItem);
            }

        }

        [RelayCommand]
        public void Cancel()
        {
            viewModel.Cancel();
        }
    }
}
