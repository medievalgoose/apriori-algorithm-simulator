using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ShopApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        public string ProductName { get; set; }

        public double Price { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        // For the Category, make it nullable, because they're using the value from CategoryId.
        // I tried this is Product/Create. If Category is non-nullable, then the ModelSate will be invalid.
        // In the View Page, the form targets CategoryId and it will have the intended result for the database.

        public ICollection<SalesDetail>? SalesDetails { get; set; }
    }
}
