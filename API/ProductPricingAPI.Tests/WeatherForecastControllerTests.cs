using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using ProductPricingAPI.Controllers;
using ProductPricingAPI.DTOs;
using ProductPricingAPI.Repositories;

namespace ProductPricingAPI.Tests;

[TestFixture]
public class ProductPricingControllerTests
{
    private IProductRepository _productRepository = null!;
    private ProductPricingController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _controller = new ProductPricingController(_productRepository);
    }

    [Test]
    public void GetProducts_ReturnsOk()
    {
        _productRepository.GetProducts().Returns([]);

        var result = _controller.GetProducts();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void GetProducts_ReturnsProducts()
    {
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Product A", Price = 100.0m, LastUpdated = DateTime.Now },
            new() { Id = 2, Name = "Product B", Price = 200.0m, LastUpdated = DateTime.Now }
        };

        _productRepository.GetProducts().Returns(products);

        var result = (OkObjectResult)_controller.GetProducts().Result!;

        Assert.That(result.Value, Is.EqualTo(products));
    }

    [Test]
    public void GetProductHistoryById_ReturnsOk_WhenProductExists()
    {
        var history = new ProductPriceHistoryDto { Id = 1, Name = "Product A" };

        _productRepository.GetProductHistory(1).Returns(history);

        var result = _controller.GetProductHistoryById(1);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void GetProductHistoryById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        _productRepository.GetProductHistory(99).ReturnsNull();

        var result = _controller.GetProductHistoryById(99);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }
}
