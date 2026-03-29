using Microsoft.AspNetCore.Mvc;
using ProductPricingAPI.DTOs;

namespace ProductPricingAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductPricingController : ControllerBase
    {
        private readonly ILogger<ProductPricingController> _logger;

        public ProductPricingController(ILogger<ProductPricingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ProductDto> GetProducts()
        {
            return new List<ProductDto>();
        }

        [HttpGet("{id:int}")]
        public IEnumerable<ProductHistoryDto> GetProductHistoryById(int id)
        {
            return new List<ProductHistoryDto>();
        }

        [HttpPost("{id:int}/apply-discount")]
        public ApplyDiscountResultDto ApplyDiscountById(int id, [FromBody] ApplyDiscountRequestDto discountRequest)
        {
            return new ApplyDiscountResultDto();
        }

        [HttpPut("{id:int}/update-price")]
        public UpdatePriceResultDto UpdatePriceById(int id, [FromBody] UpdatePriceRequestDto updatePriceRequest)
        {
            return new UpdatePriceResultDto();
        }
    }
}
