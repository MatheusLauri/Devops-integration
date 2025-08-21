using Microsoft.EntityFrameworkCore;
using IntegracaoDevOps.Data.Models;

namespace IntegracaoDevOps.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<UpgradeDevops> UpgradeDevops { get; set; }
}