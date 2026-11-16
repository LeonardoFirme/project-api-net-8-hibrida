using api_net_8_hibrida.Models;
using Microsoft.EntityFrameworkCore;

namespace api_net_8_hibrida.Data;

public class MySqlDbContext : DbContext
{
    public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().ToTable("produtos");
        base.OnModelCreating(modelBuilder);
    }
}