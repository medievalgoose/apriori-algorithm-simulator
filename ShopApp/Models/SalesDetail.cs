using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models
{
    public class SalesDetail
    {
        
        public int SalesId { get; set; }
        [ForeignKey("SalesId")]
        public Sales? Sales { get; set; }

        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Products { get; set; }


        public int Quantity { get; set; }
    }
}
