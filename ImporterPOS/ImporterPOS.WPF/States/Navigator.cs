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
        private IArticleService _articleService;
        private IGoodService _goodService;
        private IRuleService _ruleService;
        private Notifier _notifier;

        public Navigator(Notifier notifier, ISupplierService supplierService, IExcelService excelService,
            ConcurrentDictionary<string, string> myDictionary, IInventoryDocumentsService invDocsService, IStorageService storageService, IGoodService goodService, IInventoryItemBasisService invitemsService, IArticleService articleService, IRuleService ruleService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _supplierService = supplierService;
            _myDictionary = myDictionary;
            _invDocsService = invDocsService;
            _storageService = storageService;
            _goodService = goodService;
            _invitemsService = invitemsService;
            _articleService = articleService;
            DefaultLoad();
            _ruleService = ruleService;
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
                        this.CurrentViewModel = new HomeViewModel(_invDocsService, _supplierService, _articleService, _invitemsService, _excelService, _goodService, _notifier);
                        Caption = Translations.Dashboard;
                        this.Icon = IconChar.Home;
                        break;
                    case ViewType.Discounts:
                        this.CurrentViewModel = new DiscountViewModel(_excelService, _notifier, _myDictionary, _articleService, _ruleService);
                        Caption = Translations.Discounts;
                        this.Icon = IconChar.Percentage;
                        break;
                    case ViewType.Articles:
                        this.CurrentViewModel = new StoreViewModel(_articleService, _notifier, _storageService, _invDocsService, _goodService, _invitemsService, _excelService);
                        Caption = Translations.Storages;
                        this.Icon = IconChar.TableList;
                        break;
                    case ViewType.ImportArticles:
                        this.CurrentViewModel = new ArticlesViewModel(_excelService, _supplierService, _notifier, _invDocsService, _storageService, _goodService, _invitemsService, _articleService);
                        Caption = Translations.Articles;
                        this.Icon = IconChar.FileExcel;
                        break;
                    case ViewType.Settings:
                        this.CurrentViewModel = new SettingsViewModel(_notifier, _excelService, _myDictionary, _articleService);
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
            this.CurrentViewModel = new HomeViewModel(_invDocsService, _supplierService, _articleService, _invitemsService, _excelService, _goodService, _notifier);
            Caption = Translations.Dashboard;
            this.Icon = IconChar.Home;
        }




    }
}
