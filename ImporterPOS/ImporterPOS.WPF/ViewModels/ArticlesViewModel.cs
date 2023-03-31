using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class ArticlesViewModel : BaseViewModel
    {
        private readonly IExcelService _excelService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryDocumentsService _invDocsService;
        private readonly IGoodService _goodService;
        private readonly IStorageService _storageService;
        private readonly IInventoryItemBasisService _inventoryItems;
        private readonly Notifier _notifier;
        private ConcurrentDictionary<string, string> _myDictionary;


        [ObservableProperty]
        private ObservableCollection<ExcelArticlesListViewModel> articlesCollection;

        [ObservableProperty]
        public int count;

        [ObservableProperty]
        private bool isLoading;


        [ObservableProperty]
        private ExcelArticlesListViewModel articleQ;

        [ObservableProperty]
        private ICollectionView articleCollection;

        [ObservableProperty]
        ObservableCollection<ExcelArticlesListViewModel>? articleList;

        public ArticlesViewModel(IExcelService excelService, ISupplierService supplierService, Notifier notifier, ConcurrentDictionary<string, string> myDictionary, IInventoryDocumentsService invDocsService, IStorageService storageService, IGoodService goodService, IInventoryItemBasisService inventoryItems)
        {
            _excelService = excelService;
            _supplierService = supplierService;
            _notifier = notifier;
            _invDocsService = invDocsService;
            _myDictionary = myDictionary;
            articlesCollection = new ObservableCollection<ExcelArticlesListViewModel>();
            _storageService = storageService;
            _goodService = goodService;
            _inventoryItems = inventoryItems;
        }


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
                var filt = obj as ExcelArticlesListViewModel;
                return filt != null && filt.BarCode.Contains(TextToFilter);
            }
            return true;
        }

        private int currentPage = 1;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdateEnableState();
            }
        }

        private int selectedRecord = 15;
        public int SelectedRecord
        {
            get { return selectedRecord; }
            set
            {
                selectedRecord = value;
                OnPropertyChanged(nameof(SelectedRecord));
                UpdateRecordCount();
            }
        }

        private void UpdateRecordCount()
        {
            NumberOfPages = (int)Math.Ceiling((double)articleList.Count / SelectedRecord);
            NumberOfPages = NumberOfPages == 0 ? 1 : NumberOfPages;
            UpdateCollection(articleList.Take(SelectedRecord));
            CurrentPage = 1;
        }

        [ObservableProperty]
        private int numberOfPages = 15;

        [ObservableProperty]
        private bool isFirstEnabled;

        [ObservableProperty]
        private bool isPreviousEnabled;

        [ObservableProperty]
        private bool isNextEnabled;

        [ObservableProperty]
        private bool isLastEnabled;


        public static int RecordStartForm = 0;
        [RelayCommand]
        private void PreviousPage(object obj)
        {
            CurrentPage--;
            RecordStartForm = articleList.Count - SelectedRecord * (NumberOfPages - (CurrentPage - 1));

            var recordsToShow = articleList.Skip(RecordStartForm).Take(SelectedRecord);

            UpdateCollection(recordsToShow);
            UpdateEnableState();
        }

        [RelayCommand]
        private void LastPage(object obj)
        {
            var recordsToSkip = SelectedRecord * (NumberOfPages - 1);
            UpdateCollection(articleList.Skip(recordsToSkip));
            CurrentPage = NumberOfPages;
            UpdateEnableState();
        }


        [RelayCommand]
        private void FirstPage(object obj)
        {
            UpdateCollection(articleList.Take(SelectedRecord));
            CurrentPage = 1;
            UpdateEnableState();
        }

        [RelayCommand]
        private void NextPage(object obj)
        {
            RecordStartForm = CurrentPage * SelectedRecord;
            var recordsToShow = articleList.Skip(RecordStartForm).Take(SelectedRecord);
            UpdateCollection(recordsToShow);
            CurrentPage++;
            UpdateEnableState();
        }

        private void UpdateEnableState()
        {
            IsFirstEnabled = CurrentPage > 1;
            IsPreviousEnabled = CurrentPage > 1;
            IsNextEnabled = CurrentPage < NumberOfPages;
            IsLastEnabled = CurrentPage < NumberOfPages;
        }

        private void UpdateCollection(IEnumerable<ExcelArticlesListViewModel> recordsToShow)
        {
            ArticlesCollection.Clear();
            foreach (var item in recordsToShow)
            {
                ArticlesCollection.Add(item);
            }
        }


        [RelayCommand]
        public void LoadFixedExcelColumns()
        {
            ExcelArticlesListViewModel tempVm = new ExcelArticlesListViewModel();

            try
            {
                articleList = _excelService.ReadColumnsFromExcel(_myDictionary, tempVm).Result;
                _notifier.ShowInformation(articleList.Count() + " articles pulled. ");
                LoadData(articleList);
            }
            catch (Exception)
            {
                if (articleList == null)
                    _notifier.ShowError("Please check your ExcelFile & Sheet, and try again.");
                else
                    _notifier.ShowError(Translations.ErrorMessage);

            }

        }

        [RelayCommand]
        public void LoadData(ObservableCollection<ExcelArticlesListViewModel>? vm = null)
        {
            if (vm != null)
            {
                articleList = vm;
                ArticleCollection = CollectionViewSource.GetDefaultView(vm);
            }
            //IsMapped = false;

            ArticleCollection = CollectionViewSource.GetDefaultView(ArticlesCollection);
            UpdateCollection(articlesCollection.Take(SelectedRecord));
            UpdateRecordCount();
            Count = ArticleList.Count;

        }

        [RelayCommand]
        public void ClearAllData()
        {
            if (articleList != null)
            {
                articleList.Clear();
                ArticleCollection = null;
                _notifier.ShowInformation(count + " records were successfully removed.");
                Count = 0;
            }
        }


        [RelayCommand]
        public async void ImportData()
        {
            try
            {
                IsLoading = true;

                await Task.Run(async () =>
                {
                if (articleList.Any())
                {
                    Guid _supplierId = await _supplierService.GetSupplierByName("Test");
                    Guid _storageId = await _storageService.GetStorageByName("Glavno skladište");
                    int orderNmbr = await _invDocsService.GetInventoryOrderNumber();

                    var invDoc = new InventoryDocument
                    {
                        Id = Guid.NewGuid(),
                        Order = orderNmbr + 1,
                        Created = DateTime.Now,
                        SupplierId = _supplierId,
                        StorageId = _storageId,
                        Type = 1,
                        IsActivated = false,
                        IsDeleted = false
                    };
                        await _invDocsService.CreateInventoryDocAsync(invDoc);

                        for (int i = 0; i < articleList.Count; i++)
                        {
                            Guid _goodId = await _goodService.GetGoodByName(articleList[i].BarCode, 1);
                            Good newGood = new Good
                            {
                                Id = Guid.NewGuid(),
                                Name = articleList[i].Name,
                                UnitId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                                LatestPrice = Helpers.Extensions.GetDecimal(articleList[i].PricePerUnit),
                                Volumen = 1,
                                Refuse = 0
                            };
                            if(_goodId == Guid.Empty)
                            {
                                await _goodService.CreateGoodAsync(newGood);
                                _goodId = newGood.Id;
                            }
                            else
                            {
                                newGood.Id = _goodId;
                                await _goodService.UpdateGoodAsync(newGood);
                            }

                            InventoryItemBasis newInventoryItem = new InventoryItemBasis
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.Now,
                                Price = Helpers.Extensions.GetDecimal(articleList[i].PricePerUnit),
                                Quantity = Helpers.Extensions.GetDecimal(articleList[i].Quantity),
                                Total = Helpers.Extensions.GetDecimal(articleList[i].PricePerUnit) * Helpers.Extensions.GetDecimal(articleList[i].Quantity),
                                Tax = 0,
                                IsDeleted = false,
                                Discriminator = "InventoryDocumentItem",
                                InventoryDocumentId = invDoc.Id,
                                StorageId = invDoc.StorageId,
                                CurrentQuantity = Helpers.Extensions.GetDecimal(articleList[i].Quantity),
                            };

                        }

                    }



                });

                IsLoading = false;
            }
            catch
            {

            }
        }

    }
}
