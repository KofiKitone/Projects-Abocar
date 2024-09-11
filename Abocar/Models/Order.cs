using Microsoft.Identity.Client;
using Microsoft.VisualBasic;

namespace Abocar.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = "Pending";
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string PaymentOption { get; set; }
        public string DeliveryOption { get; set; }
        public ICollection<OrderProduct>? OrderProduct { get; set; }
        public int AddressId { get; set; }
        public bool isPaid { get; set; }
        public string OrderNumber { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime DeliveryDate { get; set; }
    }




    public class OrderProduct
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal Total { get; set; }
    } 


    public class Transaction
    {
        public int Id { get; set;  }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string TransactionReference { get; set; }
        public string Email { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;
        public string OrderId { get; set; }
    }

    public class TransViewModel
    {
        public string Operator { get; set; }
        public int Number { get; set; }
    }

    public class AllOrdersViewModel
    {
        public string OrderNumber { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string PaymentOption { get; set; }
        public bool isPaid { get; set; }
    }
}
