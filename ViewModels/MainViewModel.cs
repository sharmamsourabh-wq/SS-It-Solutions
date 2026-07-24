using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private bool _isLoggedIn;

        [ObservableProperty]
        private bool _isMenuOpen;

        public MainViewModel()
        {
            // Initial view is Login
            CurrentView = new LoginViewModel(this);
        }

        public void NavigateTo(object viewModel)
        {
            CurrentView = viewModel;
            IsMenuOpen = false; // Close the drawer whenever we navigate
        }

        [RelayCommand]
        private void Logout()
        {
            IsLoggedIn = false;
            NavigateTo(new LoginViewModel(this));
        }

        [RelayCommand]
        private void NavDashboard() => NavigateTo(new DashboardViewModel(this));

        [RelayCommand]
        private void NavCustomerMaster() => NavigateTo(new CustomerMasterViewModel(this));

        [RelayCommand]
        private void NavQuotationBillingModule() => NavigateTo(new QuotationBillingModuleViewModel(this));

        [RelayCommand]
        private void NavProductMaster() => NavigateTo(new ProductMasterViewModel());

        [RelayCommand]
        private void NavQuotation() => NavigateTo(new QuotationViewModel());

        [RelayCommand]
        private void NavBilling() => NavigateTo(new BillingViewModel());

        [RelayCommand]
        private void NavInvoice() => NavigateTo(new InvoiceViewModel());

        [RelayCommand]
        private void NavInventory() => NavigateTo(new InventoryViewModel());

        [RelayCommand]
        private void NavPayments() => NavigateTo(new PaymentsViewModel());

        [RelayCommand]
        private void NavReports() => NavigateTo(new ReportsViewModel());

        [RelayCommand]
        private void NavSettings() => NavigateTo(new SettingsViewModel());

        [RelayCommand]
        private void NavBackup() => NavigateTo(new BackupViewModel());

        [RelayCommand]
        private void NavRestore() => NavigateTo(new RestoreViewModel());

        [RelayCommand]
        private void NavUserManagement() => NavigateTo(new UserManagementViewModel());
    }
}
