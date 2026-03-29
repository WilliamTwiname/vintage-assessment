using System.ComponentModel.DataAnnotations;

namespace ProductPricingAPI.DTOs
{
    public class UpdatePriceRequestDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "New price must be greater than 0.")]
        public decimal? NewPrice { get; set; }
    }
}
