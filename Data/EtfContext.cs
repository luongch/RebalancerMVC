using Microsoft.EntityFrameworkCore;
using RebalancerMVC.Models;

namespace RebalancerMVC.Data
{
    public class EtfContext : DbContext
    {
        public EtfContext(DbContextOptions<EtfContext> options)
            :base(options)
        {

        }

        public DbSet<Etf> Etf { get; set; }
    }
}
