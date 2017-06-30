using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Casestudy.Models
{
    public class Product
    {
        [StringLength(15)]
        public string Id { get; set; }
        public int BrandId { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(20)]
        public string GraphicName { get; set; }
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }
        public int QtyOnHand { get; set; }
        public int QtyOnBackOrder { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
    }
}
