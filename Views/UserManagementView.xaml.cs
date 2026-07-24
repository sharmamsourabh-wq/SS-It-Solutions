using System.Windows.Controls;
using SolarQuotationBillingSystem.ViewModels;

namespace SolarQuotationBillingSystem.Views
{
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
        }

        private void PwdBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UserManagementViewModel vm && sender is PasswordBox pb)
            {
                vm.NewPassword = pb.Password;
            }
        }
    }
}
