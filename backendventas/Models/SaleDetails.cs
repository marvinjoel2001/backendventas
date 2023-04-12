using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backendventas.Models
{
    public class SaleDetails
    {
        [Key]
        public int SaleId { get; set; }
        [Key]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("ProductId")] 
        public virtual Products? Products { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sales? Sales { get; set; }
    }
}
