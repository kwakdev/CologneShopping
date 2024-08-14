using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<CologneItem>? CologneItems { get; set; }
        public virtual DbSet<Brand>? Categories { get; set; }
        public virtual DbSet<Customer>? Customers { get; set; }

        public virtual DbSet<Order>? Orders { get; set;}

        public virtual DbSet<OrderLineitem>? OrderLines { get; set; }

    }
}
