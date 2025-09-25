using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Moq;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.Helpers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWordUnitTest.Test;

public class ProductApiControllerTest
{
    private readonly Mock<IRepository<Product>> _mockRepo;
    private readonly ProductApiController _apiController;
    private readonly Helper _helper;
    private List<Product> products;
    public ProductApiControllerTest()
    {
        _mockRepo = new Mock<IRepository<Product>>();
        _apiController = new ProductApiController(_mockRepo.Object);
        _helper = new Helper();
        products = new List<Product>()
        {
            new Product{ Id=1, Name="Birkart Miles", Color="Red", Stock=12,Price=12.5m,CreateDate=DateTime.Now},
            new Product{ Id=2, Name="Birkart Debit", Color="Green", Stock=19,Price=11.3m,CreateDate=DateTime.Now},
            new Product{ Id=3, Name="Birkart Kredit", Color="Blue", Stock=18,Price=10.9m,CreateDate=DateTime.Now}
        };
    }

    [Fact]
    public async void GetProduct_ExecuteAction_ReturnOkResultWithProducts()
    {
        _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        var result = await _apiController.GetProduct();

        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

        Assert.Equal<int>(3, returnProduct.ToList().Count());
    }
    [Theory]
    [InlineData(0)]
    public async void GetProduct_ExecuteAction_ReturnNotFound(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _apiController.GetProduct(productId);

        var okResult = Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, okResult.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void GetProduct_ExecuteAction_ReturnOkResultWithProduct(int productId)
    {
        var product = products.Find(x => x.Id == productId);

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _apiController.GetProduct(productId);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnProduct = Assert.IsAssignableFrom<Product>(okResult.Value);

        Assert.Equal(productId, returnProduct.Id);
    }

    [Theory]
    [InlineData(1)]
    public async void PutProduct_IdIsNotMatch_ReturnBadRequestResult(int productId)
    {
        var result = _apiController.PutProduct(2, products.Find(x => x.Id == productId));

        Assert.IsType<BadRequestResult>(result);
    }

    [Theory]
    [InlineData(1)]
    public void PutProduct_IdIsValid_UpdateAndReturnNoContentResult(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.Update(It.IsAny<Product>())).Callback<Product>(x => product = x);

        var result = _apiController.PutProduct(1, products.Find(x => x.Id == productId));

        var checkResult = Assert.IsType<NoContentResult>(result);

        _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async void PostProduct_ExecuteAction_CreateProductReturnCreatedAction()
    {
        Product product = products.First();

        _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

        var result = await _apiController.PostProduct(product);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

        _mockRepo.Verify(repo => repo.Create(product), Times.Once);

        Assert.Equal("GetProduct", createdAtActionResult.ActionName);
    }

    [Theory]
    [InlineData(5)]
    public async void DeleteProduct_IdIsNotValid_ReturnNotFoundAction(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _apiController.DeleteProduct(productId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(2)]
    public async void DeleteProduct_IdIsValid_DeleteReturnNoContentAction(int productId)
    {
        Product product = products.Find(x => x.Id == productId);

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        
        _mockRepo.Setup(x => x.Delete(product));

        var result = await _apiController.DeleteProduct(productId);

        Assert.IsType<NoContentResult>(result);

        _mockRepo.Verify(repo => repo.Delete(product), Times.Once);
    }

    [Theory]
    [InlineData(5, 4, 9)]
    public void Add_SimpleValues_ReturnTotal(int a, int b, int total)
    {
        var result = _helper.Add(a, b);

        Assert.Equal(total, result);
    }
}
