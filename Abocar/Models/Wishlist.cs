
namespace Abocar.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int Product { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
