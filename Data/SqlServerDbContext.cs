using api_net_8_hibrida.Models;
using Microsoft.EntityFrameworkCore;

namespace api_net_8_hibrida.Data;

public class SqlServerDbContext : DbContext
{
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().ToTable("Produtos");
        base.OnModelCreating(modelBuilder);
    }
}