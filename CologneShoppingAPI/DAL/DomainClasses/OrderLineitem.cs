using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudyAPI.DAL.DomainClasses
{
    public class OrderLineitem
    {
        public int Id { get; set; }

        // Foreign Key for Order
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
       

        // Foreign Key for Product
        [ForeignKey("ProductId")]
        public string? ProductId { get; set; }
       

        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyBackOrdered { get; set; }

        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }
    }
}
