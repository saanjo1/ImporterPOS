using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.Domain.Models;
using System;

namespace ImporterPOS.WPF.Helpers
{
    [ObservableObject]
    public partial class GoodsArticlesViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Guid id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal? quantity;

        [ObservableProperty]
        private decimal? totalPurchasePrice;

        [ObservableProperty]
        private decimal? totalSoldPrice;

        [ObservableProperty]
        private decimal? totalBasePrice;

        [ObservableProperty]
        private decimal? totalTaxes;

        [ObservableProperty]
        private decimal? ruc;

        [ObservableProperty]
        private Guid storage;

        [ObservableProperty]
        private Guid goodId;

    }
}
