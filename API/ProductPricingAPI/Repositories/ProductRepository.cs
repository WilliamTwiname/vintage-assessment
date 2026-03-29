using ProductPricingAPI.DTOs;
using ProductPricingAPI.Models;

namespace ProductPricingAPI.Repositories
{
    public class ProductRepository(IProductDataRepository productDataRepository) : IProductRepository
    {
        public IEnumerable<ProductDto> GetProducts()
        {
            return productDataRepository.GetAllProducts().Select(ProductDtoMapper);
        }

        public ProductPriceHistoryDto? GetProductHistory(int id)
        {
            var product = productDataRepository.GetAllProducts().FirstOrDefault(p => p.Id == id);
            return product is null ? null : ProductHistoryDtoMapper(product);
        }

        public ApplyProductDiscountResultDto? ApplyProductDiscount(int id, ApplyProductDiscountRequestDto discountRequest)
        {
            var product = productDataRepository.GetAllProducts().FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return null;
            }

            // According to the spec, it doesn't look like we save this discount price so we can just apply the discount and create the dto model
            var originalPrice = product.Price;
            var discountAmount = originalPrice * (discountRequest.DiscountPercentage / 100);
            var discountedPrice = originalPrice - discountAmount;

            var applyDiscountResult = ApplyDiscountResultDtoMapper(product, discountedPrice);
            return applyDiscountResult;
        }

        public UpdatePriceResultDto? UpdateProductPrice(int id, UpdatePriceRequestDto updatePriceRequest)
        {
            var updateProductResult = productDataRepository.UpdateProductPrice(id, updatePriceRequest.NewPrice);

            var applyUpdateResult = UpdatePriceResultDtoMapper(updateProductResult);
            return applyUpdateResult;
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
                    Date = r.Date.ToString("yyyy-MM-dd")
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
