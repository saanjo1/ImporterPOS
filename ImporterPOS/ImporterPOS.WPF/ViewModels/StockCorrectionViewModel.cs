using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.Domain.Models;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class StockCorrectionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string currentQuantity;

        [ObservableProperty]
        private string newQuantity;
    }
}
