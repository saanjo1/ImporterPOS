using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
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
        private ObservableCollection<DiscountColumnsViewModel> articlesCollection = new ObservableCollection<DiscountColumnsViewModel>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ClearAllDataCommand))]
        [NotifyCanExecuteChangedFor(nameof(LoadFixedExcelColumnsCommand))]
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
                var filt = obj as DiscountColumnsViewModel;
                return filt != null && (filt.BarCode.Contains(TextToFilter));
            }
            return true;
        }




        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isOptions;

        [ObservableProperty]
        private DiscountColumnsViewModel mapDataModel;

        [ObservableProperty]
        private OptionsForDiscounts discountOptionsModel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadFixedExcelColumnsCommand))]
        ObservableCollection<DiscountColumnsViewModel>? articleList;

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
          if(articleList != null)
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

        private void UpdateCollection(IEnumerable<DiscountColumnsViewModel> recordsToShow)
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
                DiscountColumnsViewModel tempVm = new DiscountColumnsViewModel();
                string filePath = await _excelDataService.OpenDialog();

                if(filePath != null)
                {
                    articleList = _excelDataService.ReadDiscountColumns(filePath, tempVm).Result;
                    LoadData(articleList);
                    _notifier.ShowInformation(Translations.LoadDataSuccess);
                }

            }
            catch (Exception)
            {
                if (articleList == null)
                    _notifier.ShowError(Translations.ExcelFileError);

            }

        }


        [RelayCommand(CanExecute = nameof(CanClickOptions))]
        public void Options()
        {
            this.IsOptions = true;
            this.DiscountOptionsModel = new OptionsForDiscounts(this, _notifier);
        }

        [RelayCommand]
        public void Close()
        {
            if (IsOptions)
                IsOptions = false;
        }


        [RelayCommand(CanExecute = nameof(CanClear))]
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
        public void LoadData(ObservableCollection<DiscountColumnsViewModel>? vm)
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


        [RelayCommand]
        public async void ImportItems()
        {
            IsLoading = true;
                if (DiscountOptionsModel == null)
                {
                    _notifier.ShowWarning(Translations.OptionsRequired);
                }
                else
                {
                    int importCounter = 0;
                    int updateCounter = 0;
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
                                    var article = _articleDataService.Get(articleID.ToString()).Result;

                                    Rule disc = _discountDataService.GetRuleByName(articleList[i].Discount).Result;
                                    if (disc != null && disc.Name == articleList[i].Discount && CheckDates(disc))
                                    {
                                        newRule = _discountDataService.Get(disc.Id.ToString()).Result;
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

                                    Article newArticle = new Article()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = articleList[i].Name,
                                        Price = Helpers.Extensions.GetDecimal(articleList[i].Price),
                                        BarCode = articleList[i].BarCode,
                                        SubCategoryId = _articleDataService.ManageSubcategories(articleList[i]?.Category, articleList[i]?.Storage).Result,
                                        Deleted = false,
                                    };

                                    _articleDataService.Update(article.Id, newArticle);
                                    updateCounter++;


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

                            if (importCounter > 0)
                            {
                                _notifier.ShowSuccess("Discounts for " + importCounter + " articles successfully added.");
                            }
                            else if (notImported > 0)
                            {
                                _notifier.ShowWarning("Discounts for " + notImported + " items can not be added. Discounts for non existing article is not possible.");

                            }

                            ClearAllData();
                        }
                        catch (Exception)
                        {

                            _notifier.ShowError("Something went wrong. Please try again.");
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

        public bool CanClick()
        {
            if (IsOptions)
                return false;

            if (articleList != null)
                return false;

            return true;
        }


        public bool CanClear()
        {
            if (Count > 0)
                return true;
            return false;
        }

        public bool CanClickOptions()
        {

            if (articleList == null || articleList.Count == 0)
                return false;

            if (IsOptions)
                return false;

            if (Count < 0)
                return false;


            return true;

        }
    }
}

 