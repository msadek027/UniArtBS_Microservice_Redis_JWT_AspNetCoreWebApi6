using Microsoft.EntityFrameworkCore;

using System.Data.Common;
using Workflow.WorkflowCommon;

namespace Workflow.Data
{
    public class DbContextClass: DbContext
    {
        DBConnection conn = new DBConnection();
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(conn.SAConnStrReader(),
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,           // Number of retry attempts (default is 6)
                    maxRetryDelay: TimeSpan.FromSeconds(10), // Max delay between retries
                    errorNumbersToAdd: null    // List of additional SQL error numbers to consider transient
                )
            );
        }
       // public DbSet<Product> Products { get; set; }
    }
}
