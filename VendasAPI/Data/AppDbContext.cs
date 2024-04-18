using Microsoft.EntityFrameworkCore;
using VendasAPI.Models;

namespace VendasAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) 
        {
        }

        public DbSet<VendaModel> Vendas { get; set; }
    }
}
