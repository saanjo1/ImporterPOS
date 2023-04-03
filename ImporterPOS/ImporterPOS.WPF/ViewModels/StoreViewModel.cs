using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.Storages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ToastNotifications;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class StoreViewModel : BaseViewModel
    {
        private readonly IArticleService _articleService;
        private readonly IStorageService _storageService;
        private readonly IInventoryDocumentsService _invService;
        private readonly Notifier _notifier;

        public StoreViewModel(IArticleService articleService, IStorageService storageService, Notifier notifier, IInventoryDocumentsService invService)
        {
            _articleService = articleService;
            _storageService = storageService;
            _notifier = notifier;
            _invService = invService;
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
        private List<InventoryItemBasis> listOfItems = new List<InventoryItemBasis>();

        [ObservableProperty]
        private ICollection<GoodsArticlesViewModel> articleList;

        [ObservableProperty]
        private ICollectionView articleCollection;

        private InventoryDocument inventoryDocument;

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
        public void MultipleEdit()
        {
            IsEnabled = true;
            inventoryDocument = new InventoryDocument
            {
                Created = DateTime.Now,
                Order = _invService.GetHashCode().Result,
                Id = Guid.NewGuid(),
                StorageId = _storageService.GetStorageByName(StorageName).Result,
                SupplierId = null,
                Type = 2,
                IsActivated = true,
                IsDeleted = false
            };
            _invService.Create(inventoryDocument);
        }
        }
    }
