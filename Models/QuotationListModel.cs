using System;

namespace SolarQuotationBillingSystem.Models
{
    public class QuotationListModel
    {
        public int QuotationID { get; set; }
        public string QuotationNo { get; set; }
        public DateTime QuotationDate { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } 
        public bool IsPending => Status == "Pending";
    }
}
