using EtherScanTest.Entities.DbModel;
using Microsoft.EntityFrameworkCore;

namespace EtherScanTest.Infrastructure.Data
{
    public class EtherScanContext : DbContext
    {
        public EtherScanContext(DbContextOptions<EtherScanContext> options) : base(options)
        {
        }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
