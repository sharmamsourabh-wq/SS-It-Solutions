using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using SolarQuotationBillingSystem.Helpers;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class RestoreViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _statusMessage = "Ready to perform restore.";

        [ObservableProperty]
        private bool _isBusy;

        [RelayCommand]
        private async Task PerformRestoreAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Backup Files (*.bak;*.mdf)|*.bak;*.mdf|All Files (*.*)|*.*",
                Title = "Select Database Backup File to Restore"
            };

            if (dialog.ShowDialog() == true)
            {
                var confirm = MessageBox.Show(
                    "WARNING: Restoring the database will overwrite all existing records with the selected backup file.\n\nAre you sure you want to proceed?",
                    "Confirm Overwrite & Restore",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm != MessageBoxResult.Yes) return;

                IsBusy = true;
                StatusMessage = "Restoring database from file...";

                try
                {
                    await Task.Run(async () =>
                    {
                        string mdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolarQuotationBillingSystem", "SolarDb.mdf");
                        string masterConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

                        if (dialog.FileName.EndsWith(".mdf", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var conn = new SqlConnection(masterConnStr))
                            {
                                await conn.OpenAsync();
                                string detachSql = @"
                                    IF DB_ID('SolarDb') IS NOT NULL
                                    BEGIN
                                        ALTER DATABASE [SolarDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                        EXEC sp_detach_db 'SolarDb';
                                    END";
                                using var detachCmd = new SqlCommand(detachSql, conn);
                                await detachCmd.ExecuteNonQueryAsync();

                                File.Copy(dialog.FileName, mdfPath, true);

                                string attachSql = $"CREATE DATABASE [SolarDb] ON PRIMARY (NAME='SolarDb', FILENAME='{mdfPath}') FOR ATTACH;";
                                using var attachCmd = new SqlCommand(attachSql, conn);
                                await attachCmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            using (var conn = new SqlConnection(masterConnStr))
                            {
                                await conn.OpenAsync();
                                string restoreSql = $@"
                                    ALTER DATABASE [SolarDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                    RESTORE DATABASE [SolarDb] FROM DISK = '{dialog.FileName}' WITH REPLACE;
                                    ALTER DATABASE [SolarDb] SET MULTI_USER;";
                                using var cmd = new SqlCommand(restoreSql, conn);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusMessage = "Database restored successfully! Please restart the application.";
                            MessageBox.Show("Database restored successfully!\n\nPlease restart the application for changes to take full effect.", "Restore Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Restore failed: {ex.Message}";
                    MessageBox.Show($"Failed to restore database: {ex.Message}", "Restore Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
