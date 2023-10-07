using Hrnetgroup.Wms.Domain.ApplicationUsers;
using Hrnetgroup.Wms.Domain.Holidays;
using Hrnetgroup.Wms.Domain.Leaves;
using Hrnetgroup.Wms.Domain.Workers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hrnetgroup.Wms.EntityframeworkCore;

public class WmsDbContext : IdentityDbContext<AppUser>
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options) : base(options)
    {
        
    }

    public virtual DbSet<Worker> Workers { get; set; }
    public virtual DbSet<Holiday> Holidays { get; set; }
    public virtual DbSet<Leave> Leaves { get; set; }
    public virtual DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Worker>(b =>
        {
            b.Property(x => x.AmountPerHour).HasColumnName(nameof(Worker.AmountPerHour)).HasColumnType("decimal(18, 2)");
        });
    }
}
