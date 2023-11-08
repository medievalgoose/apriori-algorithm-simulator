using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models
{
    public class Sales
    {
        [Key]
        public int Id { get; set; }

        /*
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        */

        [DataType(DataType.Date)]
        public DateTime SalesDate { get; set; }

        public ICollection<SalesDetail>? SalesDetails { get; set; }
    }
}
