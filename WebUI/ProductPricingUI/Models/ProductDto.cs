namespace ProductPricingUI.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal NewPrice { get; set; }
    }
}
