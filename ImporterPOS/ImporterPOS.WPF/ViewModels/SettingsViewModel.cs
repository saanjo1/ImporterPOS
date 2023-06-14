using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Vml;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        private List<string> storageList;

        [ObservableProperty]
        private string selectedStorage;

        public SettingsViewModel(Notifier notifier, IExcelService excelService, IArticleService articleService, IGoodService goodService, ISupplierService supplierService, IStorageService storageService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _articleService = articleService;
            _goodService = goodService;
            _supplierService = supplierService;
            _storageService = storageService;
            ToggleButtonColor = Brushes.Gray; // Početna boja toggle buttona (siva)

            LoadArticleParameters();
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
                    var suppliers = await _supplierService.GetAll();
                    SuppliersList = suppliers.Select(supplier => supplier.Name).ToList();
                }



                // Učitaj skladišta iz baze
                if ((StorageList == null || StorageList.Count == 0) || flag)
                {
                    var storages = await _storageService.GetAll();
                    StorageList = storages.Select(storage => storage.Name).ToList();
                }


                // Provjeri postojanje datoteke supplierAndStorageData.json
                string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "supplierAndStorageData.json";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    // Učitaj vrijednosti iz datoteke
                    string json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    // Postavi vrijednosti u SelectedSupplier i SelectedStorage
                    if (SuppliersList.Contains(data["SelectedSupplier"]))
                        SelectedSupplier = data["SelectedSupplier"];

                    if (StorageList.Contains(data["SelectedStorage"]))
                        SelectedStorage = data["SelectedStorage"];
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
                Title = Translations.TitleDescription;
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
                    GoodQuantity
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
        public async Task SaveSupplierAndStorage()
        {
            try
            {
                var data = new
                {
                    SelectedSupplier,
                    SelectedStorage
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
