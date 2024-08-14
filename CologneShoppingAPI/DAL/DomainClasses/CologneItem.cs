using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudyAPI.DAL.DomainClasses
{
    public class CologneItem
    {
        [ForeignKey("BrandId")]
        public Brand? Category
        {
            get; set;
        }
        public string? Id { get; set; }
        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(200)]
        public string? ProductName { get; set; }

        [Required]
        [StringLength(200)]
        public string? GraphicName { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }

        [Required]
        public int QtyOnHand { get; set; }

        [Required]
        public int QtyOnBackOrder { get; set; }
        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
