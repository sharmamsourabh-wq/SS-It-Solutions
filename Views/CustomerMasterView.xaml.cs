using System.Windows.Controls;
namespace SolarQuotationBillingSystem.Views
{
    public partial class CustomerMasterView : UserControl
    {
        public CustomerMasterView()
        {
            try
            {
                InitializeComponent();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}\nInner: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}",
                    "InitializeComponent Error", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
                throw;
            }
        }
    }
}
