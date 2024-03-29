﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Categories;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.Domain.Services.Taxes;
using ImporterPOS.Domain.Services.Units;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private IExcelService _excelService;
        private IArticleService _articleService;
        private IGoodService _goodService;
        private ISupplierService _supplierService;
        private IStorageService _storageService;
        private IUnitService _unitService;
        private ISubCategoryService _subCategoryService;
        private ITaxService _taxService;

        [ObservableProperty]
        private string pageDescription;

        [ObservableProperty]
        private string databaseConnection;

        [ObservableProperty]
        private string serverInstance;

        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private bool isOpen;

        [ObservableProperty]
        private string articleName;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string articleBarcode;

        [ObservableProperty]
        private string articleStorage;

        [ObservableProperty]
        private string articleSupplier;

        [ObservableProperty]
        private string articleSubCategory;

        [ObservableProperty]
        private string articleTaxes;

        [ObservableProperty]
        private string articlePrice;

        [ObservableProperty]
        private string goodUnit;

        [ObservableProperty]
        private string goodPurchasePrice;

        [ObservableProperty]
        private string goodTotalPrice;

        [ObservableProperty]
        private string goodQuantity;

        [ObservableProperty]
        private string discountBarcode;

        [ObservableProperty]
        private string discountValue;

        [ObservableProperty]
        private string priceWithDiscount;

        [ObservableProperty]
        private string priceWithoutDiscount;


        [ObservableProperty]
        private bool isSubCategoryEnabled;

        [ObservableProperty]
        private bool isStorageEnabled;

        [ObservableProperty]
        private bool isUnitEnabled;

        [ObservableProperty]
        private bool isTaxEnabled;

        [ObservableProperty]
        private bool isSupplierEnabled;



        [ObservableProperty]
        private AddNewSupplierViewModel addNewSupplierVM;

        [ObservableProperty]
        private bool isToggleButtonChecked;
        [ObservableProperty]
        private Brush toggleButtonColor;

        [ObservableProperty]
        private AddNewStorageViewModel addNewStorageVM;

        [ObservableProperty]
        private bool isAddingNewSupplierOpen;

        [ObservableProperty]
        private bool isAddingNewStorageOpen;


        [ObservableProperty]
        private List<string> suppliersList;

        [ObservableProperty]
        private string selectedSupplier;

        [ObservableProperty]
        private List<string> subcategoryList;

        [ObservableProperty]
        private string selectedSubcategory;

        [ObservableProperty]
        private List<string> unitsList;

        [ObservableProperty]
        private string selectedUnit;

        [ObservableProperty]
        private List<string> taxList;

        [ObservableProperty]
        private string selectedTax;

        [ObservableProperty]
        private List<string> storageList;

        [ObservableProperty]
        private string selectedStorage;

        [ObservableProperty]
        private string mappingTitle;


        public SettingsViewModel(Notifier notifier, IExcelService excelService, IArticleService articleService, IGoodService goodService, ISupplierService supplierService, IStorageService storageService, ISubCategoryService subCategoryService, IUnitService unitService, ITaxService taxService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _articleService = articleService;
            _goodService = goodService;
            _supplierService = supplierService;
            _storageService = storageService;
            _subCategoryService = subCategoryService;
            _unitService = unitService;
            _taxService = taxService;
            ToggleButtonColor = Brushes.Gray; // Početna boja toggle buttona (siva)
            MappingTitle = Translations.MappingTitle;

            LoadArticleParameters();
            LoadDiscountParameters();
            LoadDataFromDatabase();
            GetDatabaseInfo();

        }

        private async Task LoadDataFromDatabase(bool flag = false)
        {
            try
            {
                // Učitaj dobavljače iz baze
                if ((SuppliersList == null || SuppliersList.Count == 0) || flag)
                {
                    var suppliers = _supplierService.Get();
                    if(suppliers != null)
                        SuppliersList = suppliers.Select(supplier => supplier.Name).ToList();


                }

                // Učitaj skladišta iz baze
                if ((StorageList == null || StorageList.Count == 0) || flag)
                {
                    var storages = _storageService.Get();
                    if (storages != null)
                        StorageList = storages.Select(storage => storage.Name).ToList();
                }

                // Učitaj kategorije iz baze
                if ((SubcategoryList == null || SubcategoryList.Count == 0) || flag)
                {
                    var subcategories = _subCategoryService.Get();
                    SubcategoryList = subcategories.Select(storage => storage.Name).ToList();
                }

                // Učitaj mj. jedinice iz baze
                if ((UnitsList == null || UnitsList.Count == 0) || flag)
                {
                    var units = _unitService.Get();
                    UnitsList = units.Select(storage => storage.Name).ToList();
                }

                // Učitaj porez iz baze
                if ((TaxList == null || TaxList.Count == 0) || flag)
                {
                    var taxlist = _taxService.Get();
                    TaxList = taxlist.Select(storage => storage.Value.ToString()).ToList();
                }


                // Provjeri postojanje datoteke supplierAndStorageData.json
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    // Učitaj vrijednosti iz datoteke
                    string json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    // Postavi vrijednosti 
                    if (data.ContainsKey("SelectedSupplier"))
                        SelectedSupplier = data["SelectedSupplier"].ToString();

                    if (data.ContainsKey("SelectedStorage"))
                        SelectedStorage = data["SelectedStorage"].ToString();

                    if (data.ContainsKey("SelectedSubcategory"))
                        SelectedSubcategory = data["SelectedSubcategory"].ToString();

                    if (data.ContainsKey("SelectedUnit"))
                        SelectedUnit = data["SelectedUnit"].ToString();

                    if (data.ContainsKey("SelectedTax"))
                        SelectedTax = data["SelectedTax"].ToString();

                    if (data.ContainsKey("IsTaxEnabled") && bool.TryParse(data["IsTaxEnabled"].ToString(), out bool tax))
                        IsTaxEnabled = tax;

                    if (data.ContainsKey("IsStorageEnabled") && bool.TryParse(data["IsStorageEnabled"].ToString(), out bool storg))
                        IsStorageEnabled = storg;

                    if (data.ContainsKey("IsSubCategoryEnabled") && bool.TryParse(data["IsSubCategoryEnabled"].ToString(), out bool cat))
                        IsSubCategoryEnabled = cat;

                    if (data.ContainsKey("IsUnitEnabled") && bool.TryParse(data["IsUnitEnabled"].ToString(), out bool unit))
                        IsUnitEnabled = unit;

                    if (data.ContainsKey("IsSupplierEnabled") && bool.TryParse(data["IsSupplierEnabled"].ToString(), out bool supp))
                        IsSupplierEnabled = supp;
                }
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
        }

        [RelayCommand]
        public void AddNewSupplier()
        {
            IsAddingNewSupplierOpen = true;
            this.AddNewSupplierVM = new AddNewSupplierViewModel(_supplierService, _notifier, this);
        }


        [RelayCommand]
        public void AddNewStorage()
        {
            IsAddingNewStorageOpen = true;
            this.AddNewStorageVM = new AddNewStorageViewModel(_storageService, _notifier, this);
        }
        public void GetDatabaseInfo()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            int index = appDataPath.IndexOf("Roaming");
            appDataPath = appDataPath.Substring(0, index) + Informations.POSFolderPath;

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataPath);

            XmlNode databaseNode = doc.SelectSingleNode(Informations.DatabasePath);
            XmlNode serverInstanceNode = doc.SelectSingleNode(Informations.ServerInstancePath);
            XmlNode portNode = doc.SelectSingleNode(Informations.PortPath);
            DatabaseConnection = databaseNode.InnerText;
            ServerInstance = serverInstanceNode.InnerText;
            Port = portNode.InnerText;
        }

        private void LoadArticleParameters()
        {

            try
            {
                Title = Translations.MappingTitle;
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "articleColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);


                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Dictionary<string, string>? columnNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (columnNames.ContainsKey("ArticleName"))
                        ArticleName = columnNames["ArticleName"];
                    if (columnNames.ContainsKey("ArticleBarcode"))
                        ArticleBarcode = columnNames["ArticleBarcode"];
                    if (columnNames.ContainsKey("ArticleStorage"))
                        ArticleStorage = columnNames["ArticleStorage"];
                    if (columnNames.ContainsKey("ArticleSupplier"))
                        ArticleSupplier = columnNames["ArticleSupplier"];
                    if (columnNames.ContainsKey("ArticleSubCategory"))
                        ArticleSubCategory = columnNames["ArticleSubCategory"];
                    if (columnNames.ContainsKey("ArticleTaxes"))
                        ArticleTaxes = columnNames["ArticleTaxes"];
                    if (columnNames.ContainsKey("ArticlePrice"))
                        ArticlePrice = columnNames["ArticlePrice"];
                    if (columnNames.ContainsKey("GoodUnit"))
                        GoodUnit = columnNames["GoodUnit"];
                    if (columnNames.ContainsKey("GoodPurchasePrice"))
                        GoodPurchasePrice = columnNames["GoodPurchasePrice"];
                    if (columnNames.ContainsKey("GoodQuantity"))
                        GoodQuantity = columnNames["GoodQuantity"];
                    if (columnNames.ContainsKey("GoodTotalPrice"))
                        GoodTotalPrice = columnNames["GoodTotalPrice"];
                }
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                throw;
            }
        }

        private void LoadDiscountParameters()
        {
            try
            {
                Title = Translations.MappingTitle;
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "discountColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);


                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Dictionary<string, string>? columnNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (columnNames.ContainsKey("DiscountBarcode"))
                        DiscountBarcode = columnNames["DiscountBarcode"];
                    if (columnNames.ContainsKey("DiscountValue"))
                        DiscountValue = columnNames["DiscountValue"];
                    if (columnNames.ContainsKey("PriceWithDiscount"))
                        PriceWithDiscount = columnNames["PriceWithDiscount"];
                    if (columnNames.ContainsKey("PriceWithoutDiscount"))
                        PriceWithoutDiscount = columnNames["PriceWithoutDiscount"];

                }
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                throw;
            }
        }


        [RelayCommand]
        public Task SaveArticleParameters()
        {
            try
            {
                var columnNames = new
                {
                    ArticleName,
                    ArticleBarcode,
                    ArticleSubCategory,
                    ArticleTaxes,
                    ArticlePrice,
                    GoodUnit,
                    GoodTotalPrice,
                    GoodPurchasePrice,
                    GoodQuantity,
                    ArticleStorage,
                    ArticleSupplier
                };

                string json = JsonSerializer.Serialize(columnNames);

                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "articleColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(json);
                }
                _notifier.ShowSuccess(Translations.Success);
                return Task.CompletedTask;
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                return Task.CompletedTask;
            }
        }

        [RelayCommand]
        public Task SaveDiscountParameters()
        {
            try
            {
                var columnNames = new
                {
                    DiscountBarcode,
                    DiscountValue,
                    PriceWithDiscount,
                    PriceWithoutDiscount,
                    ArticleName,
                    ArticleSubCategory
                };

                string json = JsonSerializer.Serialize(columnNames);

                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "discountColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(json);
                }
                _notifier.ShowSuccess(Translations.Success);
                return Task.CompletedTask;
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                return Task.CompletedTask;
            }
        }

        [RelayCommand]
        public Task ClearArticleParameters()
        {
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "articleColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                //Check if json file exist

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    CleanupProperties();
                    _notifier.ShowSuccess(Translations.Success);
                }
                else
                {
                    _notifier.ShowInformation(Translations.FileNotFound);
                }

                return Task.CompletedTask;

            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                return Task.CompletedTask;
            }
        }


        [RelayCommand]
        public Task ClearDiscountParameters()
        {
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "discountColumnNames.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                //Check if json file exist

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    CleanupProperties();
                    _notifier.ShowSuccess(Translations.Success);
                }
                else
                {
                    _notifier.ShowInformation(Translations.FileNotFound);
                }

                return Task.CompletedTask;

            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                return Task.CompletedTask;
            }
        }
        [RelayCommand]
        public async Task SaveSupplierAndStorage()
        {
            try
            {
                var data = new
                {
                    SelectedSupplier,
                    SelectedStorage,
                    SelectedUnit,
                    SelectedTax,
                    SelectedSubcategory,
                    IsSubCategoryEnabled,
                    IsSupplierEnabled,
                    IsUnitEnabled,
                    IsTaxEnabled,
                    IsStorageEnabled,

                };

                string json = JsonSerializer.Serialize(data);

                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteAsync(json);
                }

                _notifier.ShowSuccess(Translations.Success);
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
        }

        [RelayCommand]
        public async Task ClearSupplierAndStorage()
        {
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    CleanUpSupplierAndStorage();
                    _notifier.ShowSuccess(Translations.Success);
                }
                else
                {
                    _notifier.ShowInformation(Translations.FileNotFound);
                }
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
        }

        private void CleanUpSupplierAndStorage()
        {
            SelectedSupplier = null;
            SelectedStorage = null;
        }



        [RelayCommand]
        public void ChangeToggleButtonState(object parameter)
        {
            bool isChecked = false;
           if(parameter == "True")
            {
                isChecked = true;
                IsToggleButtonChecked = isChecked;
            }
           else
            {
                isChecked = false;
                IsToggleButtonChecked = false;
            }
            ToggleButtonColor = isChecked ? Brushes.Green : Brushes.Gray;
        }
        private void CleanupProperties()
        {
            ArticleName = string.Empty;
            ArticleBarcode = string.Empty;
            ArticleSubCategory = string.Empty;
            ArticleTaxes = string.Empty;
            ArticlePrice = string.Empty;
            GoodQuantity = string.Empty;
            GoodPurchasePrice = string.Empty;
            GoodQuantity = string.Empty;
            GoodUnit = string.Empty;
            GoodTotalPrice = string.Empty;
        }

        [RelayCommand]
        public void ClosePopUp(bool flag = false)
        {
            if(IsAddingNewSupplierOpen)
            {
                IsAddingNewSupplierOpen = false;
            }

            if (IsAddingNewStorageOpen)
            {
                IsAddingNewStorageOpen = false;
            }

            if(flag)
            {
                LoadDataFromDatabase(true);
            }
        }
    }
}
