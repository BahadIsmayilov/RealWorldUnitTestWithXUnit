using System;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.DAL;
using RealWorldUnitTest.Web.Models;

namespace RealWordUnitTest.Test;

public class ProductControllerTest
{
    protected DbContextOptions<AppDbContext> _contextOptions { get; private set; }

    public void SetContextOptions(DbContextOptions<AppDbContext> contextOptions)
    {
        _contextOptions = contextOptions;
        SeedData();
    }
    public void SeedData()
    {
        using (AppDbContext _context = new AppDbContext(_contextOptions))
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Categories.AddRange(
                new Category {  Name = "Electronics" },
                new Category {  Name = "Books" }
            );
            _context.SaveChanges();
            _context.Products.AddRange(
                 new Product {  Name = "Laptop", Color = "Black", Stock = 12, Price = 1200, CategoryId = 1 },
                 new Product {  Name = "C# Book", Color = "Blue", Stock = 5, Price = 40, CategoryId = 2 }
            );
            _context.SaveChanges();
        }
    }

}
