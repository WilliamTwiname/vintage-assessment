namespace ProductPricingUI.Models
{
    public class ApplyProductDiscountResultDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
    }
}
