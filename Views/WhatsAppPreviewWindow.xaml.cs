using System.Windows;
using SolarQuotationBillingSystem.ViewModels;

namespace SolarQuotationBillingSystem.Views
{
    public partial class WhatsAppPreviewWindow : Window
    {
        public WhatsAppPreviewWindow(WhatsAppPreviewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
