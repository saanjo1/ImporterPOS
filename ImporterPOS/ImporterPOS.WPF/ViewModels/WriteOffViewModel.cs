using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class WriteOffViewModel
    {

        [ObservableProperty]
        private string? item;

        [ObservableProperty]
        private string? item_size;

        [ObservableProperty]
        private string color_number;

        [ObservableProperty]
        private string quantity;

    }
}
