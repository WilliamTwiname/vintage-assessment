using ProductPricingAPI.DTOs;
using ProductPricingAPI.Models;

namespace ProductPricingAPI.Repositories
{
    public class ProductRepository(IProductDataRepository productDataRepository, ILogger<ProductRepository> logger) : IProductRepository
    {
        // I created this repository to handle the business logic of the API, and to map the data from the ProductDataRepository to the DTOs that are returned by the API.
        // This way, we can keep the logic of manipulating the data in one place, and the controllers can focus on handling the HTTP requests and responses.
        // It also means the ProductDataRepository doesn't need to know about the DTOs, and can focus on just providing the data.

        public IEnumerable<ProductDto> GetProducts()
        {
            try
            {
                return productDataRepository.GetAllProducts().Select(ProductDtoMapper);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong when trying to retrieve all products.");
                throw;
            }
        }

        public ProductPriceHistoryDto? GetProductHistory(int id)
        {
            try
            {
                var product = productDataRepository.GetAllProducts().FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
                    logger.LogWarning("Product with Id {ProductId} was not found. (GetProductHistory)", id);
                    throw new KeyNotFoundException($"Product with Id {id} was not found in the product catalog.");
                }

                return ProductHistoryDtoMapper(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong when trying to retrieve product history for Id {ProductId}.", id);
                throw;
            }
        }

        public ApplyProductDiscountResultDto? ApplyProductDiscount(int id, ApplyProductDiscountRequestDto discountRequest)
        {
            // The Apply Product Discount request looks like (from the spec given) it doesn't need to update anything in the memory.
            // Therefore I decided to keep it to this repository and not add any new methods to the ProductDataRepository and just return the result dto.
            try
            {
                var product = productDataRepository.GetAllProducts().FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
                    logger.LogWarning("Product with Id {ProductId} was not found. (ApplyProductDiscount)", id);
                    throw new KeyNotFoundException($"Product with Id {id} was not found in the product catalog.");
                }

                var currentPrice = product.Price;
                var discountAmount = currentPrice * (discountRequest.DiscountPercentage!.Value / 100);
                var discountedPrice = currentPrice - discountAmount;

                return ApplyDiscountResultDtoMapper(product, discountedPrice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong when trying to apply a discount for Id {ProductId}.", id);
                throw;
            }
        }

        public UpdatePriceResultDto? UpdateProductPrice(int id, UpdatePriceRequestDto updatePriceRequest)
        {
            try
            {
                var updateProductResult = productDataRepository.UpdateProductPrice(id, updatePriceRequest.NewPrice!.Value);

                return UpdatePriceResultDtoMapper(updateProductResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong when trying to update the price for Id {ProductId}.", id);
                throw;
            }
        }

        private static ProductDto ProductDtoMapper(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            LastUpdated = product.LastUpdated
        };

        private static ProductPriceHistoryDto ProductHistoryDtoMapper(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            PriceHistory = [.. product.PriceHistory
                .Select(r => new ProductPriceHistoryRecordDto
                {
                    Price = r.Price,
                    Date = r.Date.ToString("yyyy-MM-dd") // Looking at the spec, it looks like the date should be returned in a string format of "yyyy-MM-dd", so I am formatting the date to match that.
                })]
        };

        private static ApplyProductDiscountResultDto ApplyDiscountResultDtoMapper(Product product, decimal discountedPrice) => new()
        {
            Id = product.Id,
            Name = product.Name,
            OriginalPrice = product.Price,
            DiscountedPrice = discountedPrice
        };

        private static UpdatePriceResultDto UpdatePriceResultDtoMapper(ProductUpdatePrice productUpdatePrice) => new()
        {
            Id = productUpdatePrice.Id,
            Name = productUpdatePrice.Name,
            NewPrice = productUpdatePrice.NewPrice,
            LastUpdated = productUpdatePrice.LastUpdated
        };
    }
}
