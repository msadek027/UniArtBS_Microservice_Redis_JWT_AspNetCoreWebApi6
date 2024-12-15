using Microsoft.EntityFrameworkCore;
using Documents.Model;

namespace Documents.Data
{
    public class DbContextClass: DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=SILCHQSOF184D;Initial Catalog=dbHR;User Id=sa;Password=123456;",
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,           // Number of retry attempts (default is 6)
                    maxRetryDelay: TimeSpan.FromSeconds(10), // Max delay between retries
                    errorNumbersToAdd: null    // List of additional SQL error numbers to consider transient
                )
            );
        }
        public DbSet<Product> Products { get; set; }
    }
}
