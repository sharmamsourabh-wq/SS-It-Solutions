using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarQuotationBillingSystem.Helpers;
using Microsoft.Data.SqlClient;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int totalCustomers;

        [ObservableProperty]
        private int totalQuotations;

        [ObservableProperty]
        private int totalBills;

        [ObservableProperty]
        private decimal todaySales;

        [ObservableProperty]
        private int todayQuotations;

        [ObservableProperty]
        private decimal monthlySales;

        private readonly MainViewModel _mainViewModel;

        public DashboardViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _ = LoadDashboardDataAsync();
        }

        [RelayCommand]
        private void OpenQuotations()
        {
            var module = new QuotationBillingModuleViewModel(_mainViewModel);
            module.NavBilling();
            _mainViewModel.NavigateTo(module);
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                // 1. Total Customers
                var cmd1 = new SqlCommand("SELECT COUNT(*) FROM Customers", conn);
                var res1 = await cmd1.ExecuteScalarAsync();
                TotalCustomers = res1 != DBNull.Value ? Convert.ToInt32(res1) : 0;

                // 2. Total Quotations
                var cmd2 = new SqlCommand("SELECT COUNT(*) FROM Quotation", conn);
                var res2 = await cmd2.ExecuteScalarAsync();
                TotalQuotations = res2 != DBNull.Value ? Convert.ToInt32(res2) : 0;

                // 3. Today's Quotations
                var cmd3 = new SqlCommand("SELECT COUNT(*) FROM Quotation WHERE CAST(QuotationDate AS DATE) = CAST(GETDATE() AS DATE)", conn);
                var res3 = await cmd3.ExecuteScalarAsync();
                TodayQuotations = res3 != DBNull.Value ? Convert.ToInt32(res3) : 0;

                // 4. Total Bills / Invoices (Paid Quotations + Invoice Table)
                var cmd4 = new SqlCommand("SELECT (SELECT COUNT(*) FROM Invoice) + (SELECT COUNT(*) FROM Quotation WHERE Status = 'Paid')", conn);
                var res4 = await cmd4.ExecuteScalarAsync();
                TotalBills = res4 != DBNull.Value ? Convert.ToInt32(res4) : 0;

                // 5. Today's Sales
                var cmd5 = new SqlCommand(@"
                    SELECT ISNULL(
                        ISNULL((SELECT SUM(NetPayable) FROM Quotation WHERE Status = 'Paid' AND CAST(QuotationDate AS DATE) = CAST(GETDATE() AS DATE)), 0)
                        +
                        ISNULL((SELECT SUM(GrandTotal) FROM Invoice WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)), 0)
                    , 0)", conn);
                var res5 = await cmd5.ExecuteScalarAsync();
                TodaySales = res5 != DBNull.Value ? Convert.ToDecimal(res5) : 0m;

                // 6. Monthly Sales
                var cmd6 = new SqlCommand(@"
                    SELECT ISNULL(
                        ISNULL((SELECT SUM(NetPayable) FROM Quotation WHERE Status = 'Paid' AND MONTH(QuotationDate) = MONTH(GETDATE()) AND YEAR(QuotationDate) = YEAR(GETDATE())), 0)
                        +
                        ISNULL((SELECT SUM(GrandTotal) FROM Invoice WHERE MONTH(InvoiceDate) = MONTH(GETDATE()) AND YEAR(InvoiceDate) = YEAR(GETDATE())), 0)
                    , 0)", conn);
                var res6 = await cmd6.ExecuteScalarAsync();
                MonthlySales = res6 != DBNull.Value ? Convert.ToDecimal(res6) : 0m;
            }
            catch (Exception ex)
            {
                // In production, log this.
                Console.WriteLine(ex.Message);
            }
        }
    }
}
