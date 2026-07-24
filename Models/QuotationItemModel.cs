using CommunityToolkit.Mvvm.ComponentModel;

namespace SolarQuotationBillingSystem.Models
{
    public partial class QuotationItemModel : ObservableObject
    {
        public int QuotationItemID { get; set; }
        public int ProductID { get; set; }

        [ObservableProperty]
        private string _component = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _hSNCode = string.Empty;

        [ObservableProperty]
        private string _brand = string.Empty;

        [ObservableProperty]
        private decimal _quantity = 1;

        [ObservableProperty]
        private string _unit = string.Empty;

        [ObservableProperty]
        private decimal _rate = 0;

        [ObservableProperty]
        private decimal _gSTPercentage = 0;

        [ObservableProperty]
        private decimal _taxableAmount = 0;

        [ObservableProperty]
        private decimal _cGST = 0;

        [ObservableProperty]
        private decimal _sGST = 0;

        [ObservableProperty]
        private decimal _iGST = 0;

        [ObservableProperty]
        private decimal _total = 0;

        partial void OnQuantityChanged(decimal value) => CalculateRowTotals();
        partial void OnRateChanged(decimal value) => CalculateRowTotals();
        partial void OnGSTPercentageChanged(decimal value) => CalculateRowTotals();

        public bool IsIGST { get; set; } = false;

        public void CalculateRowTotals()
        {
            TaxableAmount = Quantity * Rate;
            
            decimal totalGstAmount = TaxableAmount * (GSTPercentage / 100m);
            
            if (IsIGST)
            {
                IGST = totalGstAmount;
                CGST = 0;
                SGST = 0;
            }
            else
            {
                IGST = 0;
                CGST = totalGstAmount / 2m;
                SGST = totalGstAmount / 2m;
            }

            Total = TaxableAmount + totalGstAmount;
        }
    }
}
