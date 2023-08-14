using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Rules;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Messages;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.RuleItems;
using System.Collections.Generic;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class DiscountViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string excelFile;

        private readonly IExcelService _excelDataService;
        private readonly IArticleService _articleDataService;
        private readonly IRuleService _ruleService;
        private readonly IRuleItemsService _ruleItemservice;
        private readonly Notifier _notifier;
        private readonly ConcurrentDictionary<string, string> _myDictionary;

        [ObservableProperty]
        private ObservableCollection<ArticleDiscountViewModel> articlesCollection = new ObservableCollection<ArticleDiscountViewModel>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ClearAllDataCommand))]
        [NotifyCanExecuteChangedFor(nameof(OptionsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ImportItemsCommand))]
        private int count;

        private string textToFilter;


        [ObservableProperty]
        private bool isSheetPopupOpened;

        [ObservableProperty]
        private ExcelSheetChooserViewModel excelSheetViewModel;



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
                var filt = obj as ArticleDiscountViewModel;
                return filt != null && (filt.BarCode.Contains(TextToFilter));
            }
            return true;
        }

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isOptions;

        [ObservableProperty]
        private ArticleDiscountViewModel mapDataModel;

        [ObservableProperty]
        private DiscountSettingsViewModel discountOptionsModel;

        [ObservableProperty]
        ObservableCollection<ArticleDiscountViewModel>? articleList;

        [ObservableProperty]
        private ICollectionView articleCollection;


        public DiscountViewModel(IExcelService excelDataService, Notifier notifier, ConcurrentDictionary<string, string> myDictionary, IArticleService articleDataService, IRuleService discountDataService, IRuleItemsService ruleItemservice)
        {
            _excelDataService = excelDataService;
            _notifier = notifier;
            _myDictionary = myDictionary;
            _articleDataService = articleDataService;
            _ruleService = discountDataService;
            _ruleItemservice = ruleItemservice;
        }

        private void UpdateCollection(IEnumerable<ArticleDiscountViewModel> recordsToShow)
        {
            ArticlesCollection.Clear();
            foreach (var item in recordsToShow)
            {
                ArticlesCollection.Add(item);
            }
        }


        private void ChooseSheet(string filePath)
        {
            IsSheetPopupOpened = true;
            this.ExcelSheetViewModel = new ExcelSheetChooserViewModel(_excelDataService, this, _notifier, filePath);
        }

        [RelayCommand(CanExecute = nameof(CanClickOptions))]
        public void Options()
        {
            this.IsOptions = true;
            this.DiscountOptionsModel = new DiscountSettingsViewModel(this, _notifier);
        }

        [RelayCommand]
        public void Close()
        {
            if (IsOptions)
                IsOptions = false;

            if (IsSheetPopupOpened)
                IsSheetPopupOpened = false;
        }


        [RelayCommand(CanExecute = nameof(CanClickOptions))]
        public void ClearAllData()
        {
            if (articleList != null)
            {
                articleList.Clear();
                ArticleCollection = null;
                Count = 0;
            }
            else
            {
                _notifier.ShowError("Lista je prazna.");
            }
        }


        [RelayCommand]
        public async Task LoadDefinedExcelColumns()
        {
            try
            {
                ArticleDiscountViewModel tempVm = new ArticleDiscountViewModel();
                var filePath = await _excelDataService.OpenDialog();
                if (filePath != null)
                {
                    ChooseSheet(filePath);
                    articleList = await _excelDataService.ReadDiscountColumns(filePath, ExcelSheetViewModel.SelectedSheet, tempVm);
                    LoadData(filePath);
                }
            }
            catch (Exception)
            {
                if (articleList == null && ExcelSheetViewModel.SelectedSheet != null)
                    _notifier.ShowError(Translations.ErrorMessage);
            }
        }


        public async void LoadData(string filepath, ObservableCollection<ArticleDiscountViewModel>? vm = null)
        {

            try
            {
                ArticleDiscountViewModel tempVM = new ArticleDiscountViewModel();
                ObservableCollection<ArticleDiscountViewModel> loadedVM = await _excelDataService.ReadDiscountColumns(filepath, ExcelSheetViewModel.SelectedSheet, tempVM);

                if (loadedVM != null)
                {
                    ArticleCollection = CollectionViewSource.GetDefaultView(loadedVM);
                    ArticleList = loadedVM;
                }

                Count = ArticleList.Count;
                _notifier.ShowWarning(Translations.LoadDataSuccess);

            }
            catch (Exception)
            {
                _notifier.ShowWarning(Translations.ExcelFileError);
                throw;
            }


        }
        [RelayCommand(CanExecute = nameof(CanClickOptions))]
        public async Task ImportItems()
        {
            if (DiscountOptionsModel == null || DiscountOptionsModel.OptionsFlag == false)
            {
                _notifier.ShowWarning(Translations.OptionsRequired);
            }
            else
            {
                IsLoading = true;
                int importCounter = 0;
                int notImported = 0;
                int counter = _articleDataService.GetCounter(Guid.Empty).Result;

                if (ArticleList != null)
                {
                    Rule? newRule = null;

                    try
                    {
                        // Provjeri postoji li Rule s odgovarajućim imenom
                        Rule? disc = _ruleService.Get(new Domain.SearchObjects.RuleSearchObject { Name = articleList[0].Discount }).FirstOrDefault();
                        if (disc != null && CheckDates(disc))
                        {
                            newRule = disc;
                        }
                        else
                        {
                            newRule = new Rule()
                            {
                                Id = Guid.NewGuid(),
                                Name = articleList[0].Discount + DateTime.Now.ToString("dd.MM.yyyy hh:MM"),
                                ValidFrom = DiscountOptionsModel.ValidFrom,
                                ValidTo = DiscountOptionsModel.ValidTo,
                                Type = "Period",
                                Active = DiscountOptionsModel.ActivateDiscount,
                                IsExecuted = false
                            };

                            _ruleService.Create(newRule);
                        }

                        // Koristi isti Rule za sve artikle
                        for (int i = 0; i < articleList.Count; i++)
                        {
                            Article? _article = _articleDataService.Get(new ArticleSearchObject { BarCode = articleList[i].BarCode }).FirstOrDefault();
                            if (_article != null)
                            {
                                RuleItem newRuleItem = new RuleItem()
                                {
                                    Id = Guid.NewGuid(),
                                    ArticleId = _article.Id,
                                    RuleId = newRule.Id,
                                    NewPrice = articleList[i].NewPrice
                                };

                                _ruleItemservice.Create(newRuleItem);
                                importCounter++;
                            }
                            else
                            {
                                notImported++;
                            }
                        }

                        _notifier.ShowSuccess(Translations.CountDiscountArticles + importCounter);
                        IsLoading = false;
                        ClearAllData();
                    }
                    catch
                    {
                        // Handleanje grešaka
                    }
                }
            }
        }

        private bool CheckDates(Rule disc)
        {
            TimeSpan diff = (disc.ValidFrom - DiscountOptionsModel.ValidFrom).Duration();
            TimeSpan diff1 = (disc.ValidTo - DiscountOptionsModel.ValidTo).Duration();

            double threshold = 5; // 5ms


            if (diff.TotalMilliseconds < threshold)
            {
                // Dates are the same
                return true;
            }

            return false;
        }



        public bool CanClickOptions()
        {
            if (Count > 0)
                return true;
            return false;
        }

        [RelayCommand]
        public Task DeleteArticleFromList(ArticleDiscountViewModel parameter)
        {
            try
            {
                var deletedArticle = articleList.Remove(parameter);
                _notifier.ShowSuccess(Translations.RemoveArticleSuccess);
                ArticleCollection = CollectionViewSource.GetDefaultView(articleList);
                ArticleCollection = CollectionViewSource.GetDefaultView(ArticlesCollection);
                UpdateCollection(ArticlesCollection);
                Count = ArticleList.Count;
            }
            catch (Exception)
            {
                _notifier.ShowError(Translations.ErrorMessage);
            }
            return Task.CompletedTask;

        }

    }
}

