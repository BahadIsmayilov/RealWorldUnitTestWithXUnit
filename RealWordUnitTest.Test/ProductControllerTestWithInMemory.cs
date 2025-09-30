using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.DAL;
using RealWorldUnitTest.Web.Models;

namespace RealWordUnitTest.Test;

public class ProductControllerTestWithInMemory : ProductControllerTest
{
    public ProductControllerTestWithInMemory()
    {
        SetContextOptions(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("AppDbContext").Options);
    }
    [Fact]
    public async void Create_ValidModelState_ReturnRedirectToActionWithProduct()
    {
        var newProduct = new Product { Name = "New Product", Color = "Red", Stock = 10, Price = 100 };

        using (var context = new AppDbContext(_contextOptions))
        {
            var category = context.Categories.First();

            newProduct.CategoryId = category.Id;

            var controller = new ProductController(context);

            var result = await controller.Create(newProduct);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }
        using (var context = new AppDbContext(_contextOptions))
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Name == newProduct.Name);

            Assert.Equal(newProduct.Name, product.Name);
        }
    }
    [Theory]
    [InlineData(1)]
    public async void DeleteCategory_ExistCategoryId_DeletedAllProducts(int categoryId)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            var category = await context.Categories.FindAsync(categoryId);
            Assert.NotNull(category);
            context.Categories.Remove(category);
            context.SaveChanges();
        }
        using (var context = new AppDbContext(_contextOptions))
        {
            var products = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();

            Assert.Empty(products);
        }
    }
}