using System.ComponentModel.DataAnnotations;

namespace ProductPricingAPI.DTOs
{
    public class ApplyProductDiscountRequestDto
    {
        [Required]
        [Range(0.01, 100, ErrorMessage = "Discount percentage must be between 0.01 and 100.")]
        public decimal? DiscountPercentage { get; set; }
    }
}
