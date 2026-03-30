using System.Collections.Concurrent;
using System.Text.Json;
using ProductPricingAPI.Models;

namespace ProductPricingAPI.Repositories
{
    public class ProductDataRepository(IWebHostEnvironment env, ILogger<ProductDataRepository> logger) : IProductDataRepository
    {
        // We need to use ConcurrentDictionary because it is thread safe, and the API may be called concurrently.
        // This way we can ensure that we don't run into issues with multiple threads trying to read/write to the collection at the same time.
        private readonly ConcurrentDictionary<int, Product> _products = new();

        // This is the path to the folder where the JSON files are located.
        private readonly string _pathOfData = Path.Combine(env.ContentRootPath, "Data");

        public IEnumerable<Product> GetAllProducts()
        {
            if (!_products.IsEmpty)
                return _products.Values;

            // This should only run once when the API is first called and the _products collection is empty. After that, the data will be stored in memory and returned from the _products collection.
            try
            {
                JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

                // I decided to load the data from the JSON files because it could be done like this in production or done by loading from a database.
                // I wanted to replicate the idea of loading data from an external source, and I thought it would be more interesting than hardcoding the data in the code.
                var productsData = File.ReadAllText(Path.Combine(_pathOfData, "productData.json"));
                var productsResult = JsonSerializer.Deserialize<List<Product>>(productsData, jsonOptions) ?? [];

                // I decided to load the price history data separately from the product data to replicate a more realistic scenario where the price history could be stored in a different table or collection than the product data.
                var productPricingHistoryData = File.ReadAllText(Path.Combine(_pathOfData, "productPricingHistoryData.json"));
                var productPricingHistoryResult = JsonSerializer.Deserialize<List<ProductPriceHistory>>(productPricingHistoryData, jsonOptions) ?? [];
                var productPricingHistory = productPricingHistoryResult.ToDictionary(h => h.Id);

                // Here, I am populating the _products collection but also constructing the price history with it.
                foreach (var product in productsResult)
                {
                    if (productPricingHistory.TryGetValue(product.Id, out var history))
                        product.PriceHistory = history.PriceHistory;

                    _products.TryAdd(product.Id, product);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load product data from the JSON files.");
                throw;
            }

            return _products.Values;
        }

        public ProductUpdatePrice UpdateProductPrice(int productId, decimal newPrice)
        {
            // Since we are manipulating the product data in memory, I created the UpdateProductPrice method in the ProductDataRepository to handle the logic of updating the price and the price history of a product.
            // This way, we can keep the logic of manipulating the data in one place, and the ProductRepository can focus on handling the business logic and mapping the data to the DTOs.
            // In a real-world scenario, I would split this out into seperate repositories if it was to update the JSON or the database.

            var product = GetAllProducts().FirstOrDefault(p => p.Id == productId);
            if (product is null)
            {
                logger.LogWarning("UpdateProductPrice failed. Product with Id {ProductId} was not found.", productId);
                throw new KeyNotFoundException($"Product with Id {productId} was not found in the product catalog.");
            }

            product.Price = newPrice;
            product.LastUpdated = DateTime.UtcNow;
            product.PriceHistory ??= [];
            product.PriceHistory.Add(new ProductPriceHistoryRecord
            {
                Price = newPrice,
                Date = product.LastUpdated
            });

            return new ProductUpdatePrice
            {
                Id = product.Id,
                Name = product.Name,
                NewPrice = newPrice,
                LastUpdated = product.LastUpdated
            };
        }
    }
}
