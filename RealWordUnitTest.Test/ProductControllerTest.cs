using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWordUnitTest.Test;

public class ProductControllerTest
{
    private readonly Mock<IRepository<Product>> _mockRepo;
    private readonly ProductController _productController;
    private List<Product> products;
    public ProductControllerTest()
    {
        _mockRepo = new Mock<IRepository<Product>>();
        _productController = new ProductController(_mockRepo.Object);
        products = new List<Product>()
        {
            new Product{ Id=1, Name="Pencil_1", Color="Red", Stock=12,Price=12.5m,CreateDate=DateTime.Now},
            new Product{ Id=2, Name="Pencil_2", Color="Green", Stock=19,Price=11.3m,CreateDate=DateTime.Now},
            new Product{ Id=3, Name="Pencil_3", Color="Blue", Stock=18,Price=10.9m,CreateDate=DateTime.Now}
        };
    }

    [Fact]
    public async void Index_ActionExecutes_ReturnView()
    {
        var result = await _productController.Index();

        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async void Index_ActionExecutes_ReturnProdictList()
    {
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

        var result = await _productController.Index();

        var viewResult = Assert.IsType<ViewResult>(result);

        var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

        Assert.Equal(3, productList.Count());
    }
    [Fact]
    public async void Details_IdIsNull_ReturnToBadRequest()
    {
        var result = await _productController.Details(null);

        Assert.IsType<BadRequestResult>(result);

        // Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async void Details_IdInValid_ReturnNotFound()
    {
        Product product = null;
        _mockRepo.Setup(repo => repo.GetByIdAsync(0)).ReturnsAsync(product);

        var result = await _productController.Details(0);

        var notFound = Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, notFound.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void Details_ValidId_ReturnProduct(int productId)
    {
        var product = products.FirstOrDefault(x => x.Id == productId);

        _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.Details(productId);

        var viewResult = Assert.IsType<ViewResult>(result);

        var productCheck = Assert.IsAssignableFrom<Product>(viewResult.Model);

        Assert.Equal(product.Id, productCheck.Id);
        Assert.Equal(product.Name, productCheck.Name);
    }

    [Fact]
    public void Create_ActionExecute_ReturnView()
    {
        var result = _productController.Create();

        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async void Create_InValidModelState_ReturnView()
    {
        _productController.ModelState.AddModelError("Name", "Name is required");

        var result = await _productController.Create(products.First());

        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.IsType<Product>(viewResult.Model);
    }
    [Fact]
    public async void Create_ValidModelState_ReturnRedirectToIndexAction()
    {
        var result = await _productController.Create(products.First());

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async void Create_ValidModelState_ExecuteCreateMethod()
    {
        Product product = null;
        _mockRepo.Setup(x => x.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);

        var result = await _productController.Create(products.FirstOrDefault());

        _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);

        Assert.Equal(products.First().Id, product.Id);
    }

    [Fact]
    public async void Create_InValidModelState_NeverCreateExecute()
    {
        _productController.ModelState.AddModelError("Name", "Name is required");

        var result = await _productController.Create(products.First());

        _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async void Edit_ActionExecute_ReturnView()
    {
        var result = await _productController.Edit(null);

        Assert.IsType<BadRequestResult>(result);
    }
    [Theory]
    [InlineData(4)]
    public async void Edit_IdInValid_ReturnNotFound(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.Edit(productId);

        var redirect = Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, redirect.StatusCode);
    }

    [Theory]
    [InlineData(2)]
    public async void Edit_ActionExecute_ReturnProduct(int productId)
    {
        var product = products.First(x => x.Id == productId);

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.Edit(productId);

        var viewResult = Assert.IsType<ViewResult>(result);

        var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

        Assert.Equal(product.Name, resultProduct.Name);
    }

    [Theory]
    [InlineData(1)]
    public async void Edit_IdAndProductIdInValid_ReturnBadRequest(int productId)
    {
        var product = _productController.Edit(products.First(x => x.Id == productId), 2);

        var redirect = Assert.IsType<BadRequestResult>(product);
    }

    [Theory]
    [InlineData(1)]
    public async void Edit_InValidModelState_ReturnView(int productId)
    {
        _productController.ModelState.AddModelError("Name", "Name is required");

        var result = _productController.Edit(products.First(x => x.Id == productId), productId);

        var redirect = Assert.IsType<ViewResult>(result);

        Assert.IsType<Product>(redirect.Model);
    }


    [Theory]
    [InlineData(1)]
    public void Edit_ActionExecute_ReturnRedirectActionResult(int productId)
    {
        var product = _productController.Edit(products.First(x => x.Id == productId), productId);

        var result = Assert.IsType<RedirectToActionResult>(product);

        Assert.Equal("Index", result.ActionName);
    }

    [Theory]
    [InlineData(1)]
    public async void Edit_ModelStateValid_EditModel(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.Update(It.IsAny<Product>())).Callback<Product>(x => product = x);

        var result = _productController.Edit(products.First(x => x.Id == productId), productId);

        _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once());

        Assert.Equal(product.Id, productId);
    }

    [Fact]
    public async Task Delete_IdIsNotValid_ReturnNotFound()
    {
        var result = await _productController.Delete(null);

        Assert.IsType<NotFoundResult>(result);
    }
    [Theory]
    [InlineData(5)]
    public async void Delete_IdIsNotMatchProductsId_ReturnNotFound(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.Delete(productId);

        var notFoundResult = Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, notFoundResult.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void Delete_MatchIds_ReturnProduct(int productId)
    {
        var product = products.First(x => x.Id == productId);

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.Delete(productId);

        var checkResult = Assert.IsType<ViewResult>(result);

        Assert.IsAssignableFrom<Product>(checkResult.Model);
    }

    [Theory]
    [InlineData(7)]
    public async void DeleteConfirm_ProductIsNotFound_ReturnNotFound(int productId)
    {
        Product product = null;

        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.DeleteConfirm(productId);

        var notFoundResult = Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, notFoundResult.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void DeleteConfirm_ExecuteAction_ReturnRedirect(int productId)
    {
        var product= products.First(x => x.Id==productId);
        
        _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productController.DeleteConfirm(productId);

        var resultAction = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", resultAction.ActionName);
    }

    [Theory]
    [InlineData(1)]
    public async void DeleteConfirm_GetCorrectId_DeleteMethodExecute(int productId)
    {
        var product = products.First(x => x.Id == productId);
        
        _mockRepo.Setup(repo => repo.GetByIdAsync(productId))
             .ReturnsAsync(product);

        await _productController.DeleteConfirm(productId);

        _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
    }

}
