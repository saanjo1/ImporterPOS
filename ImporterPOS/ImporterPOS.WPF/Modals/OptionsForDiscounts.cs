using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.Modals
{
    [ObservableObject]
    public partial class OptionsForDiscounts
    {
        private readonly DiscountViewModel _discountViewModel;
        private readonly Notifier _notifier;

        [ObservableProperty]
        private DateTime validFrom;

        [ObservableProperty] 
        private DateTime validTo;

        [ObservableProperty]
        private bool activateDiscount;

        [ObservableProperty]
        private bool optionsFlag;

        public OptionsForDiscounts(DiscountViewModel discountViewModel, Notifier notifier)
        {
            _discountViewModel = discountViewModel;
            _notifier = notifier;

            if (ValidFrom == DateTime.MinValue)
                ValidFrom = DateTime.Now;
            if (ValidTo == DateTime.MinValue)
                ValidTo = DateTime.Now;
        }

        [RelayCommand]
        public void Save()
        {
           if(ValidTo < ValidFrom)
            {
                _notifier.ShowWarning(Translations.WrongDateError);
            }
            else
            {
                OptionsFlag = true;
                _discountViewModel.Close();
                _notifier.ShowSuccess(Translations.DiscountSettings);
            }
        }

        [RelayCommand]
        public void Cancel()
        {
            OptionsFlag = false;
            _discountViewModel.Close();
        }
    }
}
