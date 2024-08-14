using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudyAPI.DAL.DomainClasses
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "money")]
        public decimal OrderAmount { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; } // needs to be a FK
    }
}
