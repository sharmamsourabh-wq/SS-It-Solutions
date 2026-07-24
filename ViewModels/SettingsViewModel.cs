using System;
using System.Windows;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarQuotationBillingSystem.Helpers;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _companyName = string.Empty;

        [ObservableProperty]
        private string _companyPAN = string.Empty;

        [ObservableProperty]
        private string _gSTNumber = string.Empty;

        [ObservableProperty]
        private string _state = string.Empty;

        [ObservableProperty]
        private string _stateCode = string.Empty;

        [ObservableProperty]
        private string _quotationPrefix = "QT-";

        [ObservableProperty]
        private int _quotationStartingNumber = 1000;

        public SettingsViewModel()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 * FROM Settings", conn);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    CompanyName = reader["CompanyName"]?.ToString() ?? "";
                    GSTNumber = reader["GSTNumber"]?.ToString() ?? "";
                    QuotationPrefix = reader["QuotationPrefix"]?.ToString() ?? "QT-";
                    if (reader["QuotationStartingNumber"] != DBNull.Value)
                        QuotationStartingNumber = Convert.ToInt32(reader["QuotationStartingNumber"]);
                    
                    CompanyPAN = reader["CompanyPAN"]?.ToString() ?? "";
                    State = reader["State"]?.ToString() ?? "";
                    StateCode = reader["StateCode"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SaveSettings()
        {
            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();
                var cmd = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Settings)
                    BEGIN
                        UPDATE Settings SET 
                            CompanyName = @CompanyName,
                            GSTNumber = @GSTNumber,
                            QuotationPrefix = @QuotationPrefix,
                            QuotationStartingNumber = @QuotationStartingNumber,
                            CompanyPAN = @CompanyPAN,
                            State = @State,
                            StateCode = @StateCode
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Settings (CompanyName, GSTNumber, QuotationPrefix, QuotationStartingNumber, CompanyPAN, State, StateCode)
                        VALUES (@CompanyName, @GSTNumber, @QuotationPrefix, @QuotationStartingNumber, @CompanyPAN, @State, @StateCode)
                    END
                ", conn);

                cmd.Parameters.AddWithValue("@CompanyName", CompanyName ?? "");
                cmd.Parameters.AddWithValue("@GSTNumber", GSTNumber ?? "");
                cmd.Parameters.AddWithValue("@QuotationPrefix", QuotationPrefix ?? "QT-");
                cmd.Parameters.AddWithValue("@QuotationStartingNumber", QuotationStartingNumber);
                cmd.Parameters.AddWithValue("@CompanyPAN", CompanyPAN ?? "");
                cmd.Parameters.AddWithValue("@State", State ?? "");
                cmd.Parameters.AddWithValue("@StateCode", StateCode ?? "");

                cmd.ExecuteNonQuery();
                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
