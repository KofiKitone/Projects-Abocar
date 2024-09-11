using Microsoft.CodeAnalysis;

namespace Abocar.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DateCreated { get; set; }
        public int ProductId { get; set; }
        public int ImageId { get; set; }
        public int Quantity { get; set; }
        public double Total {  get; set; }
    }


    public class cartItemViewModel
    {
        public List<Product> Product { get; set; }
        public List<ProductImage> Image { get; set; }
        public List<CartItem> CartItem { get; set; }
        public List<Review> Review { get; set; }
        public Address Address { get; set; }
        public List<DeliveryOption> DeliveryOption { get; set; }
        public List<PaymentOption> PaymentOptions { get; set; }
        public decimal Total { get; set; }
    }

    public class WishlistViewModel
    {
        public Product Product { get; set; }
        public ProductImage Image { get; set; }
        public List<Review> Review { get; set; }
    }
}
