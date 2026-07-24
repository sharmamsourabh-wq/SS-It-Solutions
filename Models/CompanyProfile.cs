using System;

namespace SolarQuotationBillingSystem.Models
{
    public class CompanyProfile
    {
        public int ID { get; set; } = 1;
        public byte[]? CompanyLogo { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string GSTIN { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string BankDetails { get; set; } = string.Empty;
        public byte[]? UPIQR { get; set; }
        public byte[]? AuthorizedSignatory { get; set; }
    }

    public class Settings
    {
        public int ID { get; set; } = 1;
        public decimal DefaultGSTPercentage { get; set; } = 18.0m;
        public decimal DefaultSubsidyPercentage { get; set; } = 0.0m;
        public string TermsAndConditions { get; set; } = string.Empty;
        public int DefaultValidDays { get; set; } = 15;
        public string InvoicePrefix { get; set; } = "INV-";
        public string QuotationPrefix { get; set; } = "QTN-";
        public string CompanyPAN { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
    }
}
