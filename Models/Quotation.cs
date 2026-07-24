using System;

namespace SolarQuotationBillingSystem.Models
{
    public class Quotation
    {
        public int QuotationID { get; set; }
        public string QuotationNo { get; set; } = string.Empty; // Auto
        public DateTime QuotationDate { get; set; } = DateTime.Now;
        public DateTime ValidUntil { get; set; } = DateTime.Now.AddDays(15);
        public int CustomerID { get; set; }
        public string SalesExecutive { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string InstallationType { get; set; } = "On-Grid";
        public string RoofType { get; set; } = "RCC Flat Roof";
        public string SubsidyEligible { get; set; } = "No";

        // Solar Specs
        public decimal SystemCapacityKW { get; set; }
        public string SolarPanelBrand { get; set; } = string.Empty;
        public int PanelWatt { get; set; }
        public int NoOfPanels { get; set; }
        public string InverterBrand { get; set; } = string.Empty;
        public decimal InverterCapacity { get; set; }
        public string Battery { get; set; } = string.Empty;
        public string MountingStructure { get; set; } = "GI Structure";
        public string EarthingKit { get; set; } = "3 Pits";
        public string LightningArrestor { get; set; } = "Conventional";
        public string MC4Connector { get; set; } = "Standard";
        public string DCCable { get; set; } = "4 sqmm";
        public string ACCable { get; set; } = "4 Core";
        
        // Custom charges
        public decimal InstallationCharges { get; set; }
        public decimal Transportation { get; set; }
        public decimal OtherCharges { get; set; }
        
        public decimal Subtotal { get; set; }
        public decimal TotalGST { get; set; }
        public decimal Subsidy { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal NetPayable { get; set; }
        public string AmountInWords { get; set; } = string.Empty;
    }

    public class QuotationItem
    {
        public int QuotationItemID { get; set; }
        public int QuotationID { get; set; }
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
