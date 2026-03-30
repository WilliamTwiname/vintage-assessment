using Microsoft.AspNetCore.Components;
using ProductPricingUI.Models;
using Radzen;

namespace ProductPricingUI.Components.Pages
{
    public partial class ProductList
    {
        [Inject]
        private HttpClient HttpClient { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public List<ProductDto> Products { get; set; } = [];

        private async Task ApplyDiscountAsync(int id, decimal discountPercentage)
        {
            var request = new ApplyProductDiscountRequestDto { DiscountPercentage = discountPercentage };
            var response = await HttpClient.PostAsJsonAsync($"api/products/{id}/apply-discount", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                NotificationService.Notify(NotificationSeverity.Error, "Apply Discount Failed", error);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<ApplyProductDiscountResultDto>();

            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product is not null && result is not null)
            {
                product.Price = result.DiscountedPrice;
            }

            StateHasChanged();
        }

        private async Task UpdatePriceAsync(int id, decimal newPrice)
        {
            var request = new UpdatePriceRequestDto { NewPrice = newPrice };
            var response = await HttpClient.PutAsJsonAsync($"api/products/{id}/update-price", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                NotificationService.Notify(NotificationSeverity.Error, "Update Price Failed", error);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<UpdatePriceResultDto>();

            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product is not null && result is not null)
            {
                product.Price = result.NewPrice;
                product.LastUpdated = result.LastUpdated;

                if (product.DiscountPercentage >= 0)
                {
                    await ApplyDiscountAsync(id, product.DiscountPercentage);
                }
            }

            StateHasChanged();
        }

        private async Task ViewHistoryAsync(int id)
        {
            var response = await HttpClient.GetFromJsonAsync<ProductPriceHistoryDto>($"api/products/{id}");

            if (response is null)
            {
                return;
            }

            await DialogService.OpenAsync<ProductPriceHistoryDialog>(
                $"Product Price History - {response.Name}",
                new Dictionary<string, object?> { { "History", response.PriceHistory } }
            );
        }
    }
}
