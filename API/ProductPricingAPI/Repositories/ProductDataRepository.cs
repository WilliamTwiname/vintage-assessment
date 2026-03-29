using System.Collections.Concurrent;
using System.Text.Json;
using ProductPricingAPI.DTOs;
using ProductPricingAPI.Models;

namespace ProductPricingAPI.Repositories
{
    public class ProductDataRepository(IWebHostEnvironment env) : IProductDataRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();

        private readonly string _pathOfData = Path.Combine(env.ContentRootPath, "Data");

        public IEnumerable<Product> GetAllProducts()
        {
            if (!_products.IsEmpty)
            {
                return _products.Values;
            }

            JsonSerializerOptions _jsonOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            var productsData = File.ReadAllText(Path.Combine(_pathOfData, "productData.json"));
            var productsResult = JsonSerializer.Deserialize<List<Product>>(productsData, _jsonOptions) ?? [];

            var productPricingHistoryData = File.ReadAllText(Path.Combine(_pathOfData, "productPricingHistoryData.json"));
            var productPricingHistoryResult = JsonSerializer.Deserialize<List<ProductPriceHistory>>(productPricingHistoryData, _jsonOptions) ?? [];
            var productPricingHistory = productPricingHistoryResult.ToDictionary(h => h.Id);

            foreach (var product in productsResult)
            {
                if (productPricingHistory.TryGetValue(product.Id, out var history))
                {
                    product.PriceHistory = history.PriceHistory;
                }

                _products.TryAdd(product.Id, product);
            }

            return _products.Values;
        }

        public ProductUpdatePrice UpdateProductPrice(int productId, decimal newPrice)
        {
            var getProduct = GetAllProducts().FirstOrDefault(p => p.Id == productId);
            if (getProduct == null)
            {
                throw new KeyNotFoundException($"Product with Id {productId} not found.");
            }

            getProduct.Price = newPrice;
            getProduct.LastUpdated = DateTime.UtcNow;
            getProduct.PriceHistory ??= [];
            getProduct.PriceHistory.Add(new ProductPriceHistoryRecord
            {
                Price = newPrice,
                Date = getProduct.LastUpdated
            });

            return new ProductUpdatePrice
            {
                Id = getProduct.Id,
                Name = getProduct.Name,
                NewPrice = newPrice,
                LastUpdated = getProduct.LastUpdated
            };
        }
    }
}
