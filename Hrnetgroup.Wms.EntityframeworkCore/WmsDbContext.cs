using Microsoft.EntityFrameworkCore;

namespace Hrnetgroup.Wms.EntityframeworkCore;

public class WmsDbContext : DbContext
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<Test> Tests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class Test
{
    public int Id { get; set; }
    
    public string TestName { get; set; }
}