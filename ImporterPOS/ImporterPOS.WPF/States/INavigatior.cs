using ImporterPOS.Domain.Models;
using ImporterPOS.WPF.ViewModels;

namespace ImporterPOS.WPF.States
{
    public enum ViewType
    {
        Home,
        Articles,
        ImportArticles,
        Settings,
        Discounts,
        Log
    }


    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
        //ICommand UpdateCurrentViewModelCommand { get; }
    }
}
