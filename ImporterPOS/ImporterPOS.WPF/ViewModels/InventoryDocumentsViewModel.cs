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
        public string name;

        [ObservableProperty]
        public decimal purchasePrice;

        [ObservableProperty]
        public decimal soldPrice;

        [ObservableProperty]
        public decimal basePrice;

        [ObservableProperty]
        public decimal taxes;

        [ObservableProperty]
        public decimal ruc;

    }
}
