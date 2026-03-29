namespace ProductPricingAPI.Models
{
    public class ProductPriceHistory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProductPriceHistoryRecord> PriceHistory { get; set; } = [];
    }
}
