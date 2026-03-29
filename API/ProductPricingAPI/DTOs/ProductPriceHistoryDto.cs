namespace ProductPricingAPI.DTOs
{
    public class ProductPriceHistoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<ProductPriceHistoryRecordDto> PriceHistory { get; set; } = [];
    }
}
