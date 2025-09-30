using System;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.Models;

namespace RealWorldUnitTest.Web.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            if (entity.Entity is Product entityReferance)
            {
                switch (entity.State)
                {
                      case EntityState.Added:
                      entityReferance.CreateDate = DateTime.Now;
                      break;
                      case EntityState.Modified:
                      entityReferance.UpdateDate = DateTime.Now;
                      Entry(entityReferance).Property(x=>x.CreateDate).IsModified = false;
                        break;
                }
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    public override int SaveChanges()
    {
         foreach (var entity in ChangeTracker.Entries())
        {
            if (entity.Entity is Product entityReferance)
            {
                switch (entity.State)
                {
                      case EntityState.Added:
                      entityReferance.CreateDate = DateTime.Now;
                      break;
                      case EntityState.Modified:
                      entityReferance.UpdateDate = DateTime.Now;
                      Entry(entityReferance).Property(x=>x.CreateDate).IsModified = false;
                        break;
                }
            }
        }
        return base.SaveChanges();
    }
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
