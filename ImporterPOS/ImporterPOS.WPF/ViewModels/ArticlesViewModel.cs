using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Categories;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.Domain.Services.Taxes;
using ImporterPOS.Domain.Services.Units;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private readonly IArticleService _articleService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IInventoryItemBasisService _inventoryItems;
        private readonly ITaxService _taxesService;
        private readonly IUnitService _unitService;
        private readonly Notifier _notifier;
        private string filePath;

        [ObservableProperty]
        private Supplier supplierEntity;

        [ObservableProperty]
        private Storage storageEntity;

        [ObservableProperty]
        private SubCategory subCategoryEntity;

        [ObservableProperty]
        private Taxis taxEntity;

        [ObservableProperty]
        private MeasureUnit unitEntity;

        [ObservableProperty]
        private Good? goodEntity;

        [ObservableProperty]
        private Article? articleEntity;


        [ObservableProperty]
        private ObservableCollection<ExcelArticlesListViewModel> articlesCollection;

        [ObservableProperty]
        private ExcelSheetChooserViewModel excelSheetViewModel;

        [ObservableProperty]
        private ImportArticleSettingsViewModel articleSettingsVM;



        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ClearAllDataCommand))]
        [NotifyCanExecuteChangedFor(nameof(ImportDataCommand))]
        public int count;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isSheetPopupOpened;

        [ObservableProperty]
        private string supplierName;

        [ObservableProperty]
        private string storageName;

        [ObservableProperty]
        private string subCategoryName;

        [ObservableProperty]
        private string unitName;

        [ObservableProperty]
        private string taxName;

        [ObservableProperty]
        private Dictionary<string, object>? data;



        [ObservableProperty]
        private bool isConnectChecked;

        [ObservableProperty]
        private bool isSettingsPopUpOpened;

        [ObservableProperty]
        private ICollectionView articleCollection;

        [ObservableProperty]
        ObservableCollection<ExcelArticlesListViewModel> articleList;

        public ArticlesViewModel(IExcelService excelService, ISupplierService supplierService, Notifier notifier, IInventoryDocumentsService invDocsService, IStorageService storageService, IGoodService goodService, IInventoryItemBasisService inventoryItems, IArticleService articleService, ISubCategoryService subCategoryService, IUnitService unitService, ITaxService taxService)
        {
            _excelService = excelService;
            _supplierService = supplierService;
            _notifier = notifier;
            _invDocsService = invDocsService;
            articlesCollection = new ObservableCollection<ExcelArticlesListViewModel>();
            _storageService = storageService;
            _goodService = goodService;
            _taxesService = taxService;
            _unitService = unitService;
            _inventoryItems = inventoryItems;
            _articleService = articleService;
            _subCategoryService = subCategoryService;
            LoadSupplierAndStorage();
        }

        private void LoadSupplierAndStorage()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "supplierAndStorageData.json";
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);


                if (data.ContainsKey("SelectedStorage") && data.ContainsKey("IsStorageEnabled") && bool.TryParse(data["IsStorageEnabled"].ToString(), out bool stEnabled) && stEnabled)
                    StorageName = data["SelectedStorage"].ToString();

                if (data.ContainsKey("SelectedSubcategory") && data.ContainsKey("IsSubCategoryEnabled") && bool.TryParse(data["IsSubCategoryEnabled"].ToString(), out bool catEnabled) && catEnabled)
                    SubCategoryName = data["SelectedSubcategory"].ToString();


                if (data.ContainsKey("SelectedSupplier") && data.ContainsKey("IsSupplierEnabled") && bool.TryParse(data["IsSupplierEnabled"].ToString(), out bool supEnabled) && supEnabled)
                    SupplierName = data["SelectedSupplier"].ToString();

                if (data.ContainsKey("SelectedUnit") && data.ContainsKey("IsUnitEnabled") && bool.TryParse(data["IsSupplierEnabled"].ToString(), out bool unitEnabled) && unitEnabled)
                    UnitName = data["SelectedUnit"].ToString();


                if (data.ContainsKey("SelectedTax") && data.ContainsKey("IsTaxEnabled") && bool.TryParse(data["IsSupplierEnabled"].ToString(), out bool taxEnabled) && taxEnabled)
                    TaxName = data["SelectedTax"].ToString();


            }
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
            if (articleList != null)
            {
                NumberOfPages = (int)Math.Ceiling((double)articleList.Count / SelectedRecord);
                NumberOfPages = NumberOfPages == 0 ? 1 : NumberOfPages;
                UpdateCollection(articleList.Take(SelectedRecord));
                CurrentPage = 1;
            }
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
        public async Task LoadDefinedExcelColumns()
        {
            try
            {
                ExcelArticlesListViewModel tempVm = new ExcelArticlesListViewModel();
                filePath = await _excelService.OpenDialog();
                if (filePath != null)
                {
                    InitializeAndLoadData(filePath);
                    articleList = await _excelService.ReadColumnsFromExcel(filePath, ExcelSheetViewModel.SelectedSheet, tempVm);
                    LoadData(filePath);
                }
            }
            catch (Exception)
            {
                if (articleList == null && ExcelSheetViewModel.SelectedSheet != null)
                    _notifier.ShowError(Translations.ErrorMessage);
            }
        }


        [RelayCommand]
        public void OpenArticleSettings()
        {
            IsSettingsPopUpOpened = true;
            this.ArticleSettingsVM = new ImportArticleSettingsViewModel(_notifier, this);
        }



        private void InitializeAndLoadData(string filePath)
        {
            IsSheetPopupOpened = true;
            this.ExcelSheetViewModel = new ExcelSheetChooserViewModel(_excelService, this, _notifier, filePath);
        }

        public async void LoadData(string filePath, ObservableCollection<ExcelArticlesListViewModel>? vm = null)
        {
            try
            {
                ExcelArticlesListViewModel tempVm = new ExcelArticlesListViewModel();
                articleList = await _excelService.ReadColumnsFromExcel(filePath, ExcelSheetViewModel.SelectedSheet, tempVm);

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

                _notifier.ShowSuccess(Translations.LoadDataSuccess);

            }
            catch
            {
                _notifier.ShowWarning(Translations.ExcelFileError);
            }

        }

        [RelayCommand(CanExecute = nameof(CanClick))]
        public void ClearAllData()
        {
            if (articleList != null)
            {
                articleList.Clear();
                ArticleCollection = null;
                _notifier.ShowInformation(Translations.ClearList);
                Count = 0;
            }
        }


        [RelayCommand(CanExecute = nameof(CanClick))]
        public async void ImportData()
        {
          
            IsConnectChecked = Helpers.Extensions.ReadFromJsonFile("supplierAndStorageData.json").Result;

            try
            {
                ICollection<Supplier> _supplier = _supplierService.Get(new SupplierSearchObject { Name = SupplierName });
                ICollection<Storage> _storage = _storageService.Get(new StorageSearchObject { Name = StorageName });
                ICollection<SubCategory> _subcategory = _subCategoryService.Get(new SubCategorySearchObject { Name = SubCategoryName });
                ICollection<Taxis> _tax = _taxesService.Get(new TaxSearchObject { Value = Helpers.Extensions.GetDecimal(TaxName) });
                ICollection<MeasureUnit> _unit = _unitService.Get(new MeasureUnitSearchObject { Name = UnitName });

                SupplierEntity = _supplier.First();
                StorageEntity = _storage.First();
                SubCategoryEntity = _subcategory.First();
                TaxEntity = _tax.First();
                UnitEntity = _unit.First();
            }
            catch
            {
                _notifier.ShowError(Translations.SettingsError);
            }

            if (TaxEntity != null && StorageEntity != null && SupplierEntity != null && SubCategoryEntity != null)
            {
                try
                {
                    InventoryDocument newInvDocument = new InventoryDocument
                    {
                        Id = Guid.NewGuid(),
                        Order = _invDocsService.GetInventoryOrderNumber().Result,
                        Created = DateTime.Now,
                        SupplierId = SupplierEntity.Id,
                        StorageId = StorageEntity.Id,
                        Type = 1,
                        IsActivated = false,
                        IsDeleted = false
                    };
                    _invDocsService.Create(newInvDocument);

                    for (int i = 0; i < articleList.Count; i++)
                    {

                        ICollection<Good> _good = _goodService.Get(new GoodSearchObject { Name = articleList[i].Name });
                        ICollection<Article> _article = _articleService.Get(new ArticleSearchObject { BarCode = articleList[i].BarCode });

                        ArticleEntity = _article.FirstOrDefault();
                        GoodEntity = _good.FirstOrDefault();

                        Good newGood = new Good
                        {
                            Id = Guid.NewGuid(),
                            Name = articleList[i].Name,
                            UnitId = UnitEntity.Id,
                            LatestPrice = Helpers.Extensions.GetDecimal(articleList[i].PricePerUnit),
                            Volumen = 1,
                            Refuse = 0
                        };

                        Article newArticle = new Article
                        {
                            Id = Guid.NewGuid(),
                            Name = articleList[i].Name,
                            Order = _articleService.GetCounter(SubCategoryEntity.Id).Result,
                            ArticleNumber = _articleService.GetCounter(Guid.Empty).Result,
                            SubCategoryId = SubCategoryEntity?.Id,
                            BarCode = articleList[i].BarCode,
                            Price = Helpers.Extensions.GetDecimal(articleList[i].ArticlePrice),
                        };

                        if (GoodEntity == null)
                        {
                            _goodService.Create(newGood);
                        }
                        else
                            _goodService.Update(GoodEntity.Id, newGood);

                        if (ArticleEntity == null)
                        {
                           
                            _articleService.Create(newArticle);
                        }
                        else
                            _articleService.Update(ArticleEntity.Id, newArticle);



                        bool articleGoodExist = _articleService.CheckForNormative(ArticleEntity != null ? ArticleEntity.Id : newArticle.Id).Result;

                        InventoryItemBasis newInventoryItem = new InventoryItemBasis
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            Price = Helpers.Extensions.GetDecimal(articleList[i].PricePerUnit),
                            Quantity = Helpers.Extensions.GetDecimal(articleList[i].Quantity),
                            Total = Helpers.Extensions.GetDecimal(articleList[i].TotalPrice),
                            Tax = 0,
                            GoodId = GoodEntity != null ? GoodEntity.Id : newGood.Id,
                            IsDeleted = false,
                            Discriminator = "InventoryDocumentItem",
                            InventoryDocumentId = newInvDocument.Id,
                            StorageId = newInvDocument.StorageId,
                            CurrentQuantity = Helpers.Extensions.GetDecimal(articleList[i].Quantity),
                        };

                        _inventoryItems.Create(newInventoryItem);

                        TaxArticle newTax = new TaxArticle
                        {
                            ArticleId = ArticleEntity != null ? ArticleEntity.Id : newArticle.Id,
                            TaxId = TaxEntity.Id
                        };

                        _taxesService.CreateTaxArticle(newTax);

                        if (IsConnectChecked && !articleGoodExist)
                        {
                            ArticleGood newArticleGood = new ArticleGood
                            {
                                Id = Guid.NewGuid(),
                                ArticleId = newArticle.Id,
                                GoodId = GoodEntity != null ? GoodEntity.Id : newGood.Id,
                                Quantity = 1,
                                ValidFrom = DateTime.Today,
                                ValidUntil = DateTime.Today.AddYears(50)
                            };
                            _articleService.SaveArticleGood(newArticleGood);
                        }

                    }
                }
                catch
                {
                    _notifier.ShowError(Translations.ImportArticlesError);
                }

            }

            _notifier.ShowSuccess(Translations.ImportArticlesSuccess);
        }


        [RelayCommand]
        public async void ChangePriceOption()
        {
            try
            {
                var _filePath = await _excelService.OpenDialog();

                if (_filePath != null)
                {
                    ObservableCollection<Article> articles = _excelService.ChangePriceOnArticles(_filePath).Result;
                    //var chng = _articleService.ChangePrices(articles);
                    bool result = _articleService.ChangePrices(articles);

                    if (result)
                        _notifier.ShowSuccess(Translations.Success);
                    else
                        _notifier.ShowError(Translations.ErrorMessage);
                }

            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                throw;
            }
        }
        public bool CanClick()

        {
            if (Count > 0)
                return true;
            return false;
        }

        [RelayCommand]
        public void Cancel()
        {
            if (IsSheetPopupOpened)
                IsSheetPopupOpened = false;
        }


        [RelayCommand]
        public Task DeleteArticleFromList(ExcelArticlesListViewModel parameter)
        {
            try
            {
                var deletedArticle = articleList.Remove(parameter);
                _notifier.ShowSuccess(Translations.RemoveArticleSuccess);
                ArticleCollection = CollectionViewSource.GetDefaultView(articleList);
                ArticleCollection = CollectionViewSource.GetDefaultView(ArticlesCollection);
                UpdateCollection(articlesCollection.Take(SelectedRecord));
                UpdateRecordCount();
                Count = ArticleList.Count;
            }
            catch (Exception)
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
            return Task.CompletedTask;

        }


        [RelayCommand]
        public Task EditArticle(ExcelArticlesListViewModel parameter)
        {
            try
            {

            }
            catch (Exception)
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
            return Task.CompletedTask;

        }

        public void CloseSettingsPopUp()
        {
            if (IsSettingsPopUpOpened)
                IsSettingsPopUpOpened = false;
        }
    }
}
