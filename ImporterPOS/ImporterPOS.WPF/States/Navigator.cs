using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Concurrent;
using ToastNotifications;
using ImporterPOS.Domain.Models;
using FontAwesome.Sharp;
using ImporterPOS.WPF.ViewModels;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.Storages;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Rules;
using ImporterPOS.Domain.Services.Units;
using ImporterPOS.Domain.Services.Categories;
using ImporterPOS.Domain.Services.Taxes;
using ImporterPOS.Domain.Services.RuleItems;

namespace ImporterPOS.WPF.States
{
    public partial class Navigator : ObservableObject, INavigator
    {

        [ObservableProperty]
        private BaseViewModel? currentViewModel;

        [ObservableProperty]
        private string caption;

        [ObservableProperty]
        private IconChar icon;


        private ConcurrentDictionary<string, string> _myDictionary;
        private IExcelService _excelService;
        private ISupplierService _supplierService;
        private IInventoryDocumentsService _invDocsService;
        private IInventoryItemBasisService _invitemsService;
        private IStorageService _storageService;
        private ISubCategoryService _subCategoryService;
        private IArticleService _articleService;
        private IGoodService _goodService;
        private IRuleService _ruleService;
        private IRuleItemsService _ruleItemsService;
        private ITaxService _taxService;
        private IUnitService _unitService;
        private Notifier _notifier;

        public Navigator(Notifier notifier, ISupplierService supplierService, IExcelService excelService,
            ConcurrentDictionary<string, string> myDictionary, IInventoryDocumentsService invDocsService, IStorageService storageService, IGoodService goodService, IInventoryItemBasisService invitemsService, IArticleService articleService, IRuleService ruleService, IUnitService unitService, ISubCategoryService subCategoryService, IRuleItemsService ruleItemsService, ITaxService taxService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _supplierService = supplierService;
            _myDictionary = myDictionary;
            _invDocsService = invDocsService;
            _unitService = unitService;
            _storageService = storageService;
            _goodService = goodService;
            _ruleService = ruleService;
            _subCategoryService = subCategoryService;
            _invitemsService = invitemsService;
            _articleService = articleService;
            _ruleItemsService = ruleItemsService;
            _taxService = taxService;
            DefaultLoad();
        }


        [RelayCommand]
        public void EditCurrentViewModel(object? parameter)
        {
            if (parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;
                switch (viewType)
                {
                    case ViewType.Home:
                        this.CurrentViewModel = new HomeViewModel();
                        Caption = Translations.Dashboard;
                        this.Icon = IconChar.Home;
                        break;
                    case ViewType.Discounts:
                        this.CurrentViewModel = new DiscountViewModel(_excelService, _notifier, _myDictionary, _articleService, _ruleService, _ruleItemsService);
                        Caption = Translations.Discounts;
                        this.Icon = IconChar.Percentage;
                        break;
                    case ViewType.ImportArticles:
                        this.CurrentViewModel = new ArticlesViewModel(_excelService, _supplierService, _notifier, _invDocsService, _storageService, _goodService, _invitemsService, _articleService, _subCategoryService, _unitService, _taxService);
                        Caption = Translations.Articles;
                        this.Icon = IconChar.FileExcel;
                        break;
                    case ViewType.Settings:
                        this.CurrentViewModel = new SettingsViewModel(_notifier, _excelService, _articleService, _goodService, _supplierService, _storageService, _subCategoryService, _unitService, _taxService);
                        Caption = Translations.Settings;
                        this.Icon = IconChar.Gear;
                        break;
                    default:
                        break;
                }
            }
        }

        public void DefaultLoad()
        {
            this.CurrentViewModel = new HomeViewModel();
            Caption = Translations.Dashboard;
            this.Icon = IconChar.Home;
        }




    }
}
