namespace ProductPricingAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<ProductPriceHistoryRecord> PriceHistory { get; set; } = [];
    }
}
