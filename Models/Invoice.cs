using System;

namespace SolarQuotationBillingSystem.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public string InvoiceNo { get; set; } = string.Empty; // Auto
        public DateTime InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        
        public decimal Subtotal { get; set; }
        public decimal TotalGST { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public string PaymentMode { get; set; } = "Cash"; // Cash, UPI, Cheque, Bank Transfer
        public string AmountInWords { get; set; } = string.Empty;
    }

    public class InvoiceItem
    {
        public int InvoiceItemID { get; set; }
        public int InvoiceID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal GSTPercentage { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal Amount { get; set; }
    }
}
