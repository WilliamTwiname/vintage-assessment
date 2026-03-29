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
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            return Ok(productRepository.GetProducts());
        }

        [HttpGet("{id:int}")]
        public ActionResult<ProductPriceHistoryDto> GetProductHistoryById(int id)
        {
            var result = productRepository.GetProductHistory(id);

            if (result is null)
                return NotFound("Product not found in the product catalog");

            return Ok(result);
        }

        [HttpPost("{id:int}/apply-discount")]
        public ActionResult<ApplyProductDiscountResultDto> ApplyDiscountById(int id, [FromBody] ApplyProductDiscountRequestDto discountRequest)
        {
            var result = productRepository.ApplyProductDiscount(id, discountRequest);

            if (result is null)
                return NotFound("Product not found in the product catalog");

            return Ok(result);
        }

        [HttpPut("{id:int}/update-price")]
        public ActionResult<UpdatePriceResultDto> UpdatePriceById(int id, [FromBody] UpdatePriceRequestDto updatePriceRequest)
        {
            var result = productRepository.UpdateProductPrice(id, updatePriceRequest);

            if (result is null)
                return NotFound("Product not found in the product catalog");

            return Ok(result);
        }
    }
}
