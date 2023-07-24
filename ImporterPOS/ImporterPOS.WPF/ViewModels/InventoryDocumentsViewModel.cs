using CommunityToolkit.Mvvm.ComponentModel;
using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class InventoryDocumentsViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string dateCreated;

        [ObservableProperty]
        public string? name;

        [ObservableProperty]
        public string purchasePrice;

        [ObservableProperty]
        public string soldPrice;

        [ObservableProperty]
        public string basePrice;

        [ObservableProperty]
        public string taxes;

        [ObservableProperty]
        public string ruc;

    }
}
