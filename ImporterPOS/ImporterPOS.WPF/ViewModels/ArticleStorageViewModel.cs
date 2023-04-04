using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.WPF.Helpers;
using ImporterPOS.WPF.Modals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Messages;

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
        private readonly string storeType;
        private Notifier _notifier;

        public ArticleStorageViewModel(IArticleService articleService, string _storeType, IStorageService storageService, Notifier notifier, IInventoryDocumentsService inventoryService, IGoodService goodService, IInventoryItemBasisService invItemService)
        {
            _articleService = articleService;
            _storageService = storageService;
            _notifier = notifier;
            _inventoryService = inventoryService;
            _goodService = goodService;
            _invItemService = invItemService;
            storeType = _storeType;
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
        private EditStorageViewModel editArticleViewModel;

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
                ArticleList = StorageQuantityCounter(storeType).Result;

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
            this.EditArticleViewModel = new EditStorageViewModel(parameter, _notifier, _storageService, this, inventoryDocument, _invItemService);
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
                if (quantity > 0)
                {
                    tempList.Add(new GoodsArticlesViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name,
                        GoodId = _goodService.GetGoodByName(item.Name).Result,
                        Quantity = quantity,
                        Storage = storage,
                        Price = Math.Round((item.LatestPrice * quantity), 2),
                        LatestPrice = item.LatestPrice
                    });

                }
            }

            return await Task.FromResult(tempList);
        }

    }
}
