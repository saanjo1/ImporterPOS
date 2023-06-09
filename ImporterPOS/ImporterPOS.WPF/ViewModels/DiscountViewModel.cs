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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class DiscountViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string excelFile;

        private readonly IExcelService _excelDataService;
        private readonly IArticleService _articleDataService;
        private readonly IRuleService _discountDataService;
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

        private void UpdateCollection(IEnumerable<ArticleDiscountViewModel> recordsToShow)
        {
            ArticlesCollection.Clear();
            foreach (var item in recordsToShow)
            {
                ArticlesCollection.Add(item);
            }
        }

        public DiscountViewModel(IExcelService excelDataService, Notifier notifier, ConcurrentDictionary<string, string> myDictionary, IArticleService articleDataService, IRuleService discountDataService)
        {
            _excelDataService = excelDataService;
            _notifier = notifier;
            _myDictionary = myDictionary;
            _articleDataService = articleDataService;
            _discountDataService = discountDataService;

        }

        [RelayCommand]
        public async Task LoadFixedExcelColumns()
        {
            try
            {
                ArticleDiscountViewModel tempVm = new ArticleDiscountViewModel();
                string filePath = await _excelDataService.OpenDialog();

                if (filePath != null)
                {
                    articleList = _excelDataService.ReadDiscountColumns(filePath, tempVm).Result;
                    LoadData(articleList);
                    _notifier.ShowInformation(Translations.LoadDataSuccess);
                }

            }
            catch (Exception)
            {
                if (articleList == null)
                    _notifier.ShowError(Translations.WrongDiscountFile);
            }

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
                _notifier.ShowError("Can not clear empty list.");
            }
        }

        [RelayCommand]
        public void LoadData(ObservableCollection<ArticleDiscountViewModel>? vm)
        {
            if (vm != null)
            {
                articleList = vm;
                ArticleCollection = CollectionViewSource.GetDefaultView(vm);
            }
            ArticleCollection = CollectionViewSource.GetDefaultView(ArticlesCollection);
            UpdateCollection(articlesCollection.Take(SelectedRecord));
            UpdateRecordCount();
            Count = ArticleList.Count;

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
                    Rule newRule;

                    try
                    {
                        for (int i = 0; i < articleList.Count; i++)
                        {
                            var articleID = _articleDataService.GetComparedByBarcode(articleList[i].BarCode).Result;
                            if (articleID != Guid.Empty)
                            {
                                var article = await _articleDataService.Get(articleID.ToString());

                                Rule disc = _discountDataService.GetRuleByName(articleList[i].Discount).Result;
                                if (disc != null && disc.Name == articleList[i].Discount && CheckDates(disc))
                                {
                                    newRule = await _discountDataService.Get(disc.Id.ToString());
                                }
                                else
                                {
                                    newRule = new Rule()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = articleList[i].Discount,
                                        ValidFrom = DiscountOptionsModel.ValidFrom,
                                        ValidTo = DiscountOptionsModel.ValidTo,
                                        Type = "Period",
                                        Active = DiscountOptionsModel.ActivateDiscount,
                                        IsExecuted = false
                                    };

                                    _discountDataService.Create(newRule);
                                }

                                RuleItem newRuleItem = new RuleItem()
                                {
                                    Id = Guid.NewGuid(),
                                    ArticleId = article.Id,
                                    RuleId = newRule.Id,
                                    NewPrice = Helpers.Extensions.GetDecimal(articleList[i].NewPrice.ToString())
                                };

                                _discountDataService.CreateRuleItem(newRuleItem);
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
                    catch (Exception)
                    {

                        _notifier.ShowError(Translations.ImportArticlesError);
                        IsLoading = false;
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

    }
}

