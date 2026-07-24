using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SolarQuotationBillingSystem.Helpers;
using SolarQuotationBillingSystem.Models;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class CustomerMasterViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        private readonly Action<int> _onRequestQuotation;

        // Current editing customer
        [ObservableProperty]
        private Customer _currentCustomer = new();

        [ObservableProperty]
        private ObservableCollection<Customer> _customerList = new();

        [ObservableProperty]
        private bool _isEditing;

        // Snackbar queue for professional toast notifications
        public ISnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public CustomerMasterViewModel(MainViewModel mainViewModel, Action<int> onRequestQuotation = null)
        {
            _mainViewModel = mainViewModel;
            _onRequestQuotation = onRequestQuotation;
            _ = LoadCustomersAsync();
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                CustomerList.Clear();
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Customers ORDER BY CustomerID DESC", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CustomerList.Add(new Customer
                    {
                        CustomerID = Convert.ToInt32(reader["CustomerID"]),
                        CustomerName = reader["CustomerName"].ToString() ?? "",
                        CompanyName = reader["CompanyName"]?.ToString(),
                        Mobile = reader["Mobile"].ToString() ?? "",
                        Email = reader["Email"]?.ToString(),
                        GSTNumber = reader["GSTNumber"]?.ToString(),
                        City = reader["City"]?.ToString(),
                        State = reader["State"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Error loading customers: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentCustomer.CustomerName) || string.IsNullOrWhiteSpace(CurrentCustomer.Mobile))
            {
                MessageQueue.Enqueue("Validation Error: Customer Name and Mobile are mandatory!");
                return;
            }

            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                // Check for duplicate mobile
                var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Customers WHERE Mobile = @Mobile AND CustomerID != @ID", conn);
                checkCmd.Parameters.AddWithValue("@Mobile", CurrentCustomer.Mobile);
                checkCmd.Parameters.AddWithValue("@ID", CurrentCustomer.CustomerID);
                if (Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0)
                {
                    MessageQueue.Enqueue("Duplicate Error: A customer with this Mobile number already exists!");
                    return;
                }

                SqlCommand cmd;
                if (CurrentCustomer.CustomerID == 0)
                {
                    // INSERT
                    cmd = new SqlCommand(@"
                        INSERT INTO Customers (CustomerName, CompanyName, ContactPerson, FatherName, Mobile, AlternateMobile, Email, Address, City, District, State, StateCode, PINCode, Aadhar, PAN, GSTNumber, InstallationAddress, Remarks)
                        VALUES (@CustomerName, @CompanyName, @ContactPerson, @FatherName, @Mobile, @AlternateMobile, @Email, @Address, @City, @District, @State, @StateCode, @PINCode, @Aadhar, @PAN, @GSTNumber, @InstallationAddress, @Remarks);
                        SELECT SCOPE_IDENTITY();
                    ", conn);
                }
                else
                {
                    // UPDATE
                    cmd = new SqlCommand(@"
                        UPDATE Customers SET 
                        CustomerName=@CustomerName, CompanyName=@CompanyName, ContactPerson=@ContactPerson, FatherName=@FatherName, 
                        Mobile=@Mobile, AlternateMobile=@AlternateMobile, Email=@Email, Address=@Address, City=@City, District=@District, 
                        State=@State, StateCode=@StateCode, PINCode=@PINCode, Aadhar=@Aadhar, PAN=@PAN, GSTNumber=@GSTNumber, InstallationAddress=@InstallationAddress, Remarks=@Remarks
                        WHERE CustomerID=@CustomerID;
                    ", conn);
                    cmd.Parameters.AddWithValue("@CustomerID", CurrentCustomer.CustomerID);
                }

                cmd.Parameters.AddWithValue("@CustomerName", CurrentCustomer.CustomerName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CompanyName", CurrentCustomer.CompanyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPerson", CurrentCustomer.ContactPerson ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FatherName", CurrentCustomer.FatherName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", CurrentCustomer.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AlternateMobile", CurrentCustomer.AlternateMobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", CurrentCustomer.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", CurrentCustomer.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", CurrentCustomer.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@District", CurrentCustomer.District ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", CurrentCustomer.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StateCode", CurrentCustomer.StateCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PINCode", CurrentCustomer.PINCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Aadhar", CurrentCustomer.Aadhar ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PAN", CurrentCustomer.PAN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTNumber", CurrentCustomer.GSTNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InstallationAddress", CurrentCustomer.InstallationAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", CurrentCustomer.Remarks ?? (object)DBNull.Value);

                if (CurrentCustomer.CustomerID == 0)
                {
                    int newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    CurrentCustomer.CustomerID = newId;
                    MessageQueue.Enqueue($"Customer saved successfully with ID: {newId}");
                }
                else
                {
                    await cmd.ExecuteNonQueryAsync();
                    MessageQueue.Enqueue("Customer updated successfully!");
                }

                // WORKFLOW AUTOMATION:
                // Refresh list
                await LoadCustomersAsync();
                
                int savedId = CurrentCustomer.CustomerID;

                // Clear form
                Clear();

                // Auto-Navigate to Next Step (Quotation / Product Selection)
                // Offer professional Snackbar action to continue immediately
                MessageQueue.Enqueue(
                    $"Customer Saved. Proceed to Quotation?",
                    "CONTINUE",
                    param => NavigateToQuotation(Convert.ToInt32(param)),
                    savedId
                );
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Database Error: {ex.Message}");
            }
        }

        private void NavigateToQuotation(int customerId)
        {
            if (_onRequestQuotation != null)
            {
                _onRequestQuotation(customerId);
            }
            else
            {
                _mainViewModel.NavigateTo(new QuotationViewModel(customerId));
            }
        }

        [RelayCommand]
        private void Clear()
        {
            CurrentCustomer = new Customer();
            IsEditing = false;
        }
        
        [RelayCommand]
        private void Edit(Customer selected)
        {
            if (selected == null) return;
            CurrentCustomer = new Customer
            {
                CustomerID = selected.CustomerID,
                CustomerName = selected.CustomerName,
                CompanyName = selected.CompanyName,
                Mobile = selected.Mobile,
                Email = selected.Email,
                GSTNumber = selected.GSTNumber,
                City = selected.City,
                State = selected.State,
                // In a full app, you would load all fields from DB here if needed
            };
            IsEditing = true;
        }

        [RelayCommand]
        private void CreateQuotation(Customer selected)
        {
            if (selected != null)
            {
                NavigateToQuotation(selected.CustomerID);
            }
        }

        [RelayCommand]
        private async Task Delete(Customer selected)
        {
            if (selected == null || selected.CustomerID == 0) return;

            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Customers WHERE CustomerID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", selected.CustomerID);
                await cmd.ExecuteNonQueryAsync();

                MessageQueue.Enqueue($"Customer {selected.CustomerName} deleted successfully.");
                
                if (CurrentCustomer.CustomerID == selected.CustomerID)
                {
                    Clear();
                }

                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Error deleting customer: {ex.Message}");
            }
        }
    }
}
