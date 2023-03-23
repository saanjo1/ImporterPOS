using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Concurrent;
using ToastNotifications;
using ImporterPOS.Domain.Models;
using FontAwesome.Sharp;
using ImporterPOS.WPF.ViewModels;
using ImporterPOS.WPF.Resources;

namespace ImporterPOS.WPF.States
{
    public partial class Navigator : ObservableObject, INavigator
    {

        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        [ObservableProperty]
        private string caption;

        [ObservableProperty]
        private IconChar icon;


        private ConcurrentDictionary<string, string> _myDictionary;
        private Notifier _notifier;

        public Navigator (Notifier notifier)
        {
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
                        Icon = IconChar.Home;
                        break;
                    case ViewType.Discounts:
                        this.CurrentViewModel = new DiscountViewModel();
                        Caption = Translations.Discounts;
                        Icon = IconChar.Percentage;
                        break;
                    case ViewType.Articles:
                        this.CurrentViewModel = new StoreViewModel();
                        Caption = Translations.Storages;
                        Icon = IconChar.TableList;
                        break;
                    case ViewType.ImportArticles:
                        this.CurrentViewModel = new ArticlesViewModel();
                        Caption = Translations.Articles;
                        Icon = IconChar.FileExcel;
                        break;
                    case ViewType.Settings:
                        this.CurrentViewModel = new SettingsViewModel(_notifier);
                        Caption = Translations.Settings;
                        Icon = IconChar.Gear;
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
            Icon = IconChar.Home;
        }




    }
}
