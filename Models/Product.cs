using System;

namespace SolarQuotationBillingSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; } // Auto
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; } // Percentage
        public int Stock { get; set; }
        public string Description { get; set; } = string.Empty;
        public string HSNCode { get; set; } = string.Empty;
        public string TaxType { get; set; } = "Exclusive";
        public decimal TaxablePrice { get; set; }
    }
}
