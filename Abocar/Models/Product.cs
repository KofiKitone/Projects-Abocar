using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Abocar.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FullDestription { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Brand { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StockQuantity { get; set; }
        public List<int>? ImagesId { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategory { get; set; } 
        public int VendorId { get; set; }
        public bool isActive { get; set; }
        public decimal? commission { get; set; }
        public decimal? MainPrice { get; set; }
        //variant  
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; } // Corrected spelling
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
    }


    public class ProductSet
    {
        public Product Product { get; set; }
        public ProductImage Image { get; set; }
    }

    public class ProductViewModel
    {
        public List<Product> Product { get; set; }
        public List<ProductImage> Image { get; set; }
        public List<Review> Review { get; set; }
    }

    public class ProductDetailsVM
    {
        public Product Product { get; set; }
        public List<ProductImage> Image { get; set; }
        public List<Review> Review { get; set; }
    }


    public class Review
    {
        public Guid ReviewId { get; set; }
        public int ProductId { get; set; }
        public Guid UserId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerified { get; set; }
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;
    }




}
