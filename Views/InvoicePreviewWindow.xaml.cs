using System;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace SolarQuotationBillingSystem.Views
{
    public partial class InvoicePreviewWindow : Window
    {
        private readonly string _pdfPath;

        public InvoicePreviewWindow(string pdfPath)
        {
            InitializeComponent();
            _pdfPath = pdfPath;
            Loaded += InvoicePreviewWindow_Loaded;
        }

        private async void InvoicePreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use a writable user data folder so WebView2 works on client PCs
                // where the install directory is read-only (fixes E_ACCESSDENIED error)
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SolarQuotationBillingSystem", "WebView2Cache");

                Directory.CreateDirectory(userDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await pdfWebView.EnsureCoreWebView2Async(env);
                pdfWebView.Source = new Uri(_pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize PDF preview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pdfWebView.CoreWebView2.ExecuteScriptAsync("window.print();");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to print: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            pdfWebView.ZoomFactor += 0.1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (pdfWebView.ZoomFactor > 0.2)
            {
                pdfWebView.ZoomFactor -= 0.1;
            }
        }

        private void FitWidth_Click(object sender, RoutedEventArgs e)
        {
            pdfWebView.ZoomFactor = 1.0;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
