using System.Windows;
using SolarQuotationBillingSystem.Helpers;

namespace SolarQuotationBillingSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ensure database and tables are created
            DatabaseHelper.InitializeDatabase();
        }
    }
}
