using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class QuotationBillingModuleViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private ObservableObject currentSubView;

        // Flags for UI Selection state
        [ObservableProperty] private bool isCustomerMasterSelected;
        [ObservableProperty] private bool isQuotationSelected;
        [ObservableProperty] private bool isBillingSelected;

        public QuotationBillingModuleViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            
            // Default to Customer Master when opening the module
            NavCustomerMaster();
        }

        [RelayCommand]
        public void NavCustomerMaster()
        {
            CurrentSubView = new CustomerMasterViewModel(_mainViewModel, NavQuotationWithCustomer);
            UpdateSelection(true, false, false);
        }

        [RelayCommand]
        public void NavQuotation()
        {
            CurrentSubView = new QuotationViewModel();
            UpdateSelection(false, true, false);
        }

        public void NavQuotationWithCustomer(int customerId)
        {
            CurrentSubView = new QuotationViewModel(customerId);
            UpdateSelection(false, true, false);
        }

        [RelayCommand]
        public void NavBilling()
        {
            // Pass the _mainViewModel reference since BillingViewModel needs it for "Create Bill" to navigate to Invoice
            CurrentSubView = new BillingViewModel(_mainViewModel);
            UpdateSelection(false, false, true);
        }

        private void UpdateSelection(bool customer, bool quotation, bool billing)
        {
            IsCustomerMasterSelected = customer;
            IsQuotationSelected = quotation;
            IsBillingSelected = billing;
        }
    }
}
