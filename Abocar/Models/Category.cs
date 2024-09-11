namespace Abocar.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; } // Use ICollection for better performance
    }

    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }  
    }

    public class BrandandCat
    {
        public string active { get; set; }
        public Brand? brand { get; set; }
        public SubCategory? subCategory { get; set; }
    }



}
