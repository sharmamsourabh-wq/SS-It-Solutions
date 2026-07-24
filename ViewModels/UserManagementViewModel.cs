using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using SolarQuotationBillingSystem.Helpers;
using SolarQuotationBillingSystem.Models;

namespace SolarQuotationBillingSystem.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<User> _usersList = new();

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private string _newUsername = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _selectedRole = "Admin";

        public ObservableCollection<string> Roles { get; } = new() { "Admin", "Operator", "User" };

        public UserManagementViewModel()
        {
            _ = LoadUsersAsync();
        }

        [RelayCommand]
        private async Task LoadUsersAsync()
        {
            try
            {
                UsersList.Clear();
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("SELECT Id, Username, PasswordHash, Role FROM Users ORDER BY Id", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    UsersList.Add(new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"]?.ToString() ?? "",
                        PasswordHash = reader["PasswordHash"]?.ToString() ?? "",
                        Role = reader["Role"]?.ToString() ?? "User"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task AddUserAsync()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Please enter both Username and Password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @user", conn);
                checkCmd.Parameters.AddWithValue("@user", NewUsername.Trim());
                int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                if (count > 0)
                {
                    MessageBox.Show("A user with this username already exists.", "Duplicate User", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var insertCmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, Role) VALUES (@user, @pass, @role)", conn);
                insertCmd.Parameters.AddWithValue("@user", NewUsername.Trim());
                insertCmd.Parameters.AddWithValue("@pass", NewPassword.Trim());
                insertCmd.Parameters.AddWithValue("@role", SelectedRole);
                await insertCmd.ExecuteNonQueryAsync();

                MessageBox.Show($"User '{NewUsername}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                NewUsername = string.Empty;
                NewPassword = string.Empty;
                SelectedRole = "Admin";

                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteUserAsync(User? userToDelete)
        {
            var user = userToDelete ?? SelectedUser;
            if (user == null)
            {
                MessageBox.Show("Please select a user to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UsersList.Count <= 1)
            {
                MessageBox.Show("Cannot delete the last user in the system.", "Action Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete user '{user.Username}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", user.Id);
                await cmd.ExecuteNonQueryAsync();

                MessageBox.Show($"User '{user.Username}' deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
