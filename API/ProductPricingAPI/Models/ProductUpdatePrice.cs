namespace ProductPricingAPI.Models
{
    public class ProductUpdatePrice
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
