using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using SolarQuotationBillingSystem.Helpers;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class WhatsAppPreviewViewModel : ObservableObject
    {
        [ObservableProperty] private string mobileNumber = string.Empty;
        [ObservableProperty] private string messageText = string.Empty;
        
        public string CustomerName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string PdfPath { get; set; } = string.Empty;

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }

        [RelayCommand]
        private void Send(Window window)
        {
            if (string.IsNullOrWhiteSpace(MobileNumber))
            {
                MessageBox.Show("Please enter a valid mobile number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Clean mobile number
                string cleanMobile = MobileNumber.Replace(" ", "").Replace("+", "").Replace("-", "");
                if (cleanMobile.Length == 10) cleanMobile = "91" + cleanMobile; // Assume India

                // Copy PDF to clipboard
                if (!string.IsNullOrEmpty(PdfPath) && System.IO.File.Exists(PdfPath))
                {
                    Clipboard.SetFileDropList(new StringCollection { PdfPath });
                }

                // Open WhatsApp via API (most reliable for forwarding text to Desktop App)
                string url = $"https://api.whatsapp.com/send?phone={cleanMobile}&text={Uri.EscapeDataString(MessageText)}";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                // Log to history
                LogHistory("Success");

                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                LogHistory("Failed");
                MessageBox.Show($"Could not launch WhatsApp: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogHistory(string status)
        {
            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO WhatsAppHistory (SentDate, CustomerName, MobileNumber, DocumentType, Status)
                    VALUES (@date, @name, @mobile, @type, @status)", conn);
                
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@name", CustomerName ?? "");
                cmd.Parameters.AddWithValue("@mobile", MobileNumber ?? "");
                cmd.Parameters.AddWithValue("@type", DocumentType ?? "");
                cmd.Parameters.AddWithValue("@status", status);
                
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging WhatsApp history: {ex.Message}");
            }
        }
    }
}
