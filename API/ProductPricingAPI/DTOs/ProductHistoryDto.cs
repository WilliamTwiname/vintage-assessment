namespace ProductPricingAPI.DTOs
{
    public class ProductHistoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<ProductHistoryRecordDto> PriceHistory { get; set; } = [];
    }
}
