using System.Data.Entity;
using SnackStore.Domain.Entities;

namespace SnackStore.Domain.Concrete
{
    class EFDbContext : DbContext
    {
        public EFDbContext(): base("EFDbContext") 
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<EFDbContext>());
        }

        public DbSet<Product> Products { get; set; }
    }
}
