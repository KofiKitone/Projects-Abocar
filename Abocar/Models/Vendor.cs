using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Abocar.Models
{
    public class Vendor
    {
        public int Id { get; set; }  
        public string UserId {  get; set; }
        public string BusinessEmail { get; set; }
        public bool IsRegistered { get; set; } = false;
        public string Status { get; set; } = "Not Active";
        public List<Product>? Products { get; set; }
    }

    public class VendorTransactionModel
    {
        public string Vendor { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * (decimal)Total;
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public string TransactionReference { get; set; }
        public string OrderId { get; set; }
        public string PaymentOption { get; set; }
    }
}
