using ProductPricingAPI.Models;

namespace ProductPricingAPI.Repositories
{
    public interface IProductDataRepository
    {
        IEnumerable<Product> GetAllProducts();
        ProductUpdatePrice UpdateProductPrice(int productId, decimal newPrice);
    }
}
