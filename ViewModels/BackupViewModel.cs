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
    public partial class BackupViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _dbLocation = string.Empty;

        [ObservableProperty]
        private string _statusMessage = "Ready to perform backup.";

        [ObservableProperty]
        private bool _isBusy;

        public BackupViewModel()
        {
            string dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolarQuotationBillingSystem");
            DbLocation = Path.Combine(dbFolder, "SolarDb.mdf");
        }

        [RelayCommand]
        private async Task PerformBackupAsync()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Database Backup (*.bak)|*.bak|SQL MDF File (*.mdf)|*.mdf|All Files (*.*)|*.*",
                FileName = $"SolarDb_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak",
                Title = "Select Destination to Save Backup File"
            };

            if (dialog.ShowDialog() == true)
            {
                IsBusy = true;
                StatusMessage = "Creating database backup...";

                try
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                            await conn.OpenAsync();
                            string sql = $"BACKUP DATABASE [SolarDb] TO DISK = '{dialog.FileName}' WITH FORMAT, INIT, NAME = 'Full SolarDb Backup';";
                            using var cmd = new SqlCommand(sql, conn);
                            await cmd.ExecuteNonQueryAsync();

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                StatusMessage = $"Backup completed successfully!\nSaved at: {dialog.FileName}";
                                MessageBox.Show($"Database backup completed successfully!\n\nLocation:\n{dialog.FileName}", "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                            });
                        }
                        catch
                        {
                            // Fallback file copy if SQL BACKUP command is restricted in LocalDB
                            string sourceMdf = DbLocation;
                            string destFile = dialog.FileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase) ? dialog.FileName.Replace(".bak", ".mdf") : dialog.FileName;
                            File.Copy(sourceMdf, destFile, true);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                StatusMessage = $"Backup file copied successfully!\nSaved at: {destFile}";
                                MessageBox.Show($"Database file backup copied successfully!\n\nLocation:\n{destFile}", "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Backup failed: {ex.Message}";
                    MessageBox.Show($"Failed to create backup: {ex.Message}", "Backup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
