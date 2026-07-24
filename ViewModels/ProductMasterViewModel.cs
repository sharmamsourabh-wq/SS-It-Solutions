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
    public partial class ProductMasterViewModel : ObservableObject
    {
        [ObservableProperty]
        private Product _currentProduct = new();

        [ObservableProperty]
        private ObservableCollection<Product> _productList = new();

        [ObservableProperty]
        private bool _isEditing;

        public ISnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public ObservableCollection<string> TaxTypes { get; } = new() { "Exclusive", "Inclusive" };

        public ProductMasterViewModel()
        {
            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                ProductList.Clear();
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Products ORDER BY ProductID DESC", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ProductList.Add(new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"]?.ToString() ?? "",
                        Category = reader["Category"]?.ToString() ?? "",
                        Brand = reader["Brand"]?.ToString() ?? "",
                        Model = reader["Model"]?.ToString() ?? "",
                        Unit = reader["Unit"]?.ToString() ?? "",
                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0,
                        GST = reader["GST"] != DBNull.Value ? Convert.ToDecimal(reader["GST"]) : 0,
                        Stock = reader["Stock"] != DBNull.Value ? Convert.ToInt32(reader["Stock"]) : 0,
                        Description = reader["Description"]?.ToString() ?? "",
                        HSNCode = reader["HSNCode"]?.ToString() ?? "",
                        TaxType = reader["TaxType"]?.ToString() ?? "Exclusive",
                        TaxablePrice = reader["TaxablePrice"] != DBNull.Value ? Convert.ToDecimal(reader["TaxablePrice"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Error loading products: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentProduct.ProductName) || string.IsNullOrWhiteSpace(CurrentProduct.HSNCode))
            {
                MessageQueue.Enqueue("Product Name and HSN Code are required!");
                return;
            }

            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();

                // Calculate Taxable Price
                if (CurrentProduct.TaxType == "Inclusive")
                {
                    CurrentProduct.TaxablePrice = CurrentProduct.Price / (1 + (CurrentProduct.GST / 100m));
                }
                else
                {
                    CurrentProduct.TaxablePrice = CurrentProduct.Price;
                }

                SqlCommand cmd;
                if (CurrentProduct.ProductID == 0)
                {
                    cmd = new SqlCommand(@"
                        INSERT INTO Products (ProductName, Category, Brand, Model, Unit, Price, GST, Stock, Description, HSNCode, TaxType, TaxablePrice)
                        VALUES (@ProductName, @Category, @Brand, @Model, @Unit, @Price, @GST, @Stock, @Description, @HSNCode, @TaxType, @TaxablePrice);
                    ", conn);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        UPDATE Products SET 
                        ProductName=@ProductName, Category=@Category, Brand=@Brand, Model=@Model, Unit=@Unit, 
                        Price=@Price, GST=@GST, Stock=@Stock, Description=@Description, HSNCode=@HSNCode, TaxType=@TaxType, TaxablePrice=@TaxablePrice
                        WHERE ProductID=@ProductID;
                    ", conn);
                    cmd.Parameters.AddWithValue("@ProductID", CurrentProduct.ProductID);
                }

                cmd.Parameters.AddWithValue("@ProductName", CurrentProduct.ProductName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", CurrentProduct.Category ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Brand", CurrentProduct.Brand ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Model", CurrentProduct.Model ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Unit", CurrentProduct.Unit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Price", CurrentProduct.Price);
                cmd.Parameters.AddWithValue("@GST", CurrentProduct.GST);
                cmd.Parameters.AddWithValue("@Stock", CurrentProduct.Stock);
                cmd.Parameters.AddWithValue("@Description", CurrentProduct.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HSNCode", CurrentProduct.HSNCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxType", CurrentProduct.TaxType ?? "Exclusive");
                cmd.Parameters.AddWithValue("@TaxablePrice", CurrentProduct.TaxablePrice);

                await cmd.ExecuteNonQueryAsync();
                
                MessageQueue.Enqueue(CurrentProduct.ProductID == 0 ? "Product Added Successfully!" : "Product Updated Successfully!");
                await LoadProductsAsync();
                Clear();
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Database Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Edit(Product product)
        {
            if (product == null) return;
            CurrentProduct = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Category = product.Category,
                Brand = product.Brand,
                Model = product.Model,
                Unit = product.Unit,
                Price = product.Price,
                GST = product.GST,
                Stock = product.Stock,
                Description = product.Description,
                HSNCode = product.HSNCode,
                TaxType = product.TaxType,
                TaxablePrice = product.TaxablePrice
            };
            IsEditing = true;
        }

        [RelayCommand]
        private async Task Delete(Product product)
        {
            if (product == null) return;
            try
            {
                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@ProductID", conn);
                cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                await cmd.ExecuteNonQueryAsync();
                
                MessageQueue.Enqueue("Product Deleted!");
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Error deleting product: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Clear()
        {
            CurrentProduct = new Product();
            IsEditing = false;
        }
    }
}
