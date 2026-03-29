using ProductPricingAPI.DTOs;

namespace ProductPricingAPI.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<ProductDto> GetProducts();
        ProductPriceHistoryDto? GetProductHistory(int id);
        ApplyProductDiscountResultDto? ApplyProductDiscount(int id, ApplyProductDiscountRequestDto discountRequest);
        UpdatePriceResultDto? UpdateProductPrice(int id, UpdatePriceRequestDto updatePriceRequest);
    }
}
