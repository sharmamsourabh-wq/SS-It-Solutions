using System.ComponentModel.DataAnnotations;

namespace SolarQuotationBillingSystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        public string CustomerName { get; set; } = string.Empty;

        public string? CompanyName { get; set; }
        
        public string? ContactPerson { get; set; }
        
        public string? FatherName { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        public string Mobile { get; set; } = string.Empty;

        public string? AlternateMobile { get; set; }
        
        public string? Email { get; set; }
        
        public string? Address { get; set; }
        
        public string? City { get; set; }
        
        public string? District { get; set; }
        
        public string? State { get; set; }
        
        public string? StateCode { get; set; }
        
        public string? PINCode { get; set; }
        
        public string? Aadhar { get; set; }
        
        public string? PAN { get; set; }
        
        public string? GSTNumber { get; set; }
        
        public string? InstallationAddress { get; set; }
        
        public string? Remarks { get; set; }
    }
}
