using Microsoft.AspNetCore.Mvc;
using ProductPricingAPI.DTOs;
using ProductPricingAPI.Repositories;

namespace ProductPricingAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductPricingController(IProductRepository productRepository) : ControllerBase
    {
        [HttpGet]
        [EndpointSummary("Get all products")]
        [EndpointDescription("Retrieve a list of products with their current prices.")]
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            return Ok(productRepository.GetProducts());
        }

        [HttpGet("{id:int}")]
        [EndpointSummary("Get product price history")]
        [EndpointDescription("Retrieves a product's price history")]
        public ActionResult<ProductPriceHistoryDto> GetProductHistoryById(int id)
        {
            var result = productRepository.GetProductHistory(id);
            return Ok(result);
        }

        [HttpPost("{id:int}/apply-discount")]
        [EndpointSummary("Apply a discount to a product")]
        [EndpointDescription("Applies a discount to a product. The discount should be applied in percentage terms (but without the % sign)")]
        public ActionResult<ApplyProductDiscountResultDto> ApplyDiscountById(int id, [FromBody] ApplyProductDiscountRequestDto discountRequest)
        {
            var result = productRepository.ApplyProductDiscount(id, discountRequest);
            return Ok(result);
        }

        [HttpPut("{id:int}/update-price")]
        [EndpointSummary("Update the price of a product")]
        [EndpointDescription("Update the price of a product")]
        public ActionResult<UpdatePriceResultDto> UpdatePriceById(int id, [FromBody] UpdatePriceRequestDto updatePriceRequest)
        {
            var result = productRepository.UpdateProductPrice(id, updatePriceRequest);
            return Ok(result);
        }
    }
}
