namespace Abocar.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AdressLine { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCods { get; set; }
        public string DigitalAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }


    public class Message
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int? OrderId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }

    public class MessageViewModel()
    {
        public Message Message { get; set; }
        public List<ProductAndImage> Product { get; set; }
    }

    public class ProductAndImage()
    {
        public Product Product { get; set; }
        public ProductImage Image { get; set; }
    }

}
