namespace Abocar.Models
{
    public class PaymentOption
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; }
        public bool isActive { get; set; }
    }


    public class DeliveryOption
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; }  
        public decimal Cost { get; set; } 
        public string? EstimatedDeliveryTime { get; set; }
        public bool isActive { get; set; } 
    }

    public class LocalPickUp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressLine { get; set; }
        public bool isActive { get; set; }
    }
    public class EditOptionViewModel
    {
        public string DPChoice { get; set; }
        public PaymentOption PaymentOption { get; set; }
        public DeliveryOption DeliveryOption { get; set; }
        public LocalPickUp LocalPickUp { get; set; }
    }
}
