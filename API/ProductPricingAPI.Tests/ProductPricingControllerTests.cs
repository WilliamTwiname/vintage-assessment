using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
    public void GetProducts_ReturnsAllProducts()
    {
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Product A", Price = 100.0m, LastUpdated = DateTime.Parse("2024-09-26T12:34:56") },
            new() { Id = 2, Name = "Product B", Price = 200.0m, LastUpdated = DateTime.Parse("2024-09-25T10:12:34") }
        };

        _productRepository.GetProducts().Returns(products);

        var result = (OkObjectResult)_controller.GetProducts().Result!;

        Assert.That(result.Value, Is.EqualTo(products));
    }

    [Test]
    public void GetProducts_ReturnsEmptyList_WhenNoProductsExist()
    {
        _productRepository.GetProducts().Returns([]);

        var result = (OkObjectResult)_controller.GetProducts().Result!;

        Assert.That(result.Value, Is.Empty);
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
    public void GetProductHistoryById_ReturnsCorrectHistory_WhenProductExists()
    {
        var history = new ProductPriceHistoryDto
        {
            Id = 1,
            Name = "Product A",
            PriceHistory =
            [
                new() { Price = 120.0m, Date = "2024-09-01" },
                new() { Price = 110.0m, Date = "2024-08-15" },
                new() { Price = 100.0m, Date = "2024-08-10" }
            ]
        };

        _productRepository.GetProductHistory(1).Returns(history);

        var result = (OkObjectResult)_controller.GetProductHistoryById(1).Result!;

        Assert.That(result.Value, Is.EqualTo(history));
    }

    [Test]
    public void GetProductHistoryById_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        _productRepository.GetProductHistory(99).Throws(new KeyNotFoundException());

        Assert.Throws<KeyNotFoundException>(() => _controller.GetProductHistoryById(99));
    }

    [Test]
    public void ApplyDiscountById_ReturnsOk_WhenProductExists()
    {
        var request = new ApplyProductDiscountRequestDto { DiscountPercentage = 10 };
        var result = new ApplyProductDiscountResultDto { Id = 1, Name = "Product A", OriginalPrice = 100.0m, DiscountedPrice = 90.0m };

        _productRepository.ApplyProductDiscount(1, request).Returns(result);

        var response = _controller.ApplyDiscountById(1, request);

        Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void ApplyDiscountById_ReturnsCorrectDiscountedPrice()
    {
        var request = new ApplyProductDiscountRequestDto { DiscountPercentage = 10 };
        var result = new ApplyProductDiscountResultDto { Id = 1, Name = "Product A", OriginalPrice = 100.0m, DiscountedPrice = 90.0m };

        _productRepository.ApplyProductDiscount(1, request).Returns(result);

        var response = (OkObjectResult)_controller.ApplyDiscountById(1, request).Result!;
        var value = (ApplyProductDiscountResultDto)response.Value!;

        Assert.That(value.DiscountedPrice, Is.EqualTo(90.0m));
    }

    [Test]
    public void ApplyDiscountById_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        var request = new ApplyProductDiscountRequestDto { DiscountPercentage = 10 };

        _productRepository.ApplyProductDiscount(99, request).Throws(new KeyNotFoundException());

        Assert.Throws<KeyNotFoundException>(() => _controller.ApplyDiscountById(99, request));
    }

    [Test]
    public void UpdatePriceById_ReturnsOk_WhenProductExists()
    {
        var request = new UpdatePriceRequestDto { NewPrice = 150.0m };
        var result = new UpdatePriceResultDto { Id = 1, Name = "Product A", NewPrice = 150.0m, LastUpdated = DateTime.UtcNow };

        _productRepository.UpdateProductPrice(1, request).Returns(result);

        var response = _controller.UpdatePriceById(1, request);

        Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void UpdatePriceById_ReturnsUpdatedPrice()
    {
        var request = new UpdatePriceRequestDto { NewPrice = 150.0m };
        var result = new UpdatePriceResultDto { Id = 1, Name = "Product A", NewPrice = 150.0m, LastUpdated = DateTime.UtcNow };

        _productRepository.UpdateProductPrice(1, request).Returns(result);

        var response = (OkObjectResult)_controller.UpdatePriceById(1, request).Result!;
        var value = (UpdatePriceResultDto)response.Value!;

        Assert.That(value.NewPrice, Is.EqualTo(150.0m));
    }

    [Test]
    public void UpdatePriceById_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        var request = new UpdatePriceRequestDto { NewPrice = 150.0m };

        _productRepository.UpdateProductPrice(99, request).Throws(new KeyNotFoundException());

        Assert.Throws<KeyNotFoundException>(() => _controller.UpdatePriceById(99, request));
    }
}
