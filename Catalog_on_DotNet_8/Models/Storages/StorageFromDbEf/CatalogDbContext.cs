using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Catalog_on_DotNet
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<Unit> Units { get; set; }
        public DbSet<Unit.SaveQuantityChange> QuantityChanges { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=myCatalogDbEntity.db");
        }
    }
}
