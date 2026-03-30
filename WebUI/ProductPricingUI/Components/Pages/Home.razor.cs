using Microsoft.AspNetCore.Components;
using ProductPricingUI.Models;
using System.Net.Http.Json;

namespace ProductPricingUI.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private HttpClient HttpClient { get; set; } = null!;
        private List<ProductDto>? products;
        private bool isLoading = true;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                products = await HttpClient.GetFromJsonAsync<List<ProductDto>>("api/products");
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to load products: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ApplyDiscountAsync(int id, decimal discountPercentage)
        {
            var request = new ApplyProductDiscountRequestDto { DiscountPercentage = discountPercentage };
            var response = await HttpClient.PostAsJsonAsync($"api/products/{id}/apply-discount", request);

            if (!response.IsSuccessStatusCode)
                return;

            var result = await response.Content.ReadFromJsonAsync<ApplyProductDiscountResultDto>();

            var product = products?.FirstOrDefault(p => p.Id == id);
            if (product is not null && result is not null)
                product.Price = result.DiscountedPrice;

            StateHasChanged();
        }

        private async Task UpdatePriceAsync(int id, decimal newPrice)
        {
            var request = new UpdatePriceRequestDto { NewPrice = newPrice };
            var response = await HttpClient.PutAsJsonAsync($"api/products/{id}/update-price", request);

            if (!response.IsSuccessStatusCode)
                return;

            var result = await response.Content.ReadFromJsonAsync<UpdatePriceResultDto>();

            var product = products?.FirstOrDefault(p => p.Id == id);
            if (product is not null && result is not null)
            {
                product.Price = result.NewPrice;
                product.LastUpdated = result.LastUpdated;

                if (product.DiscountPercentage >= 0
                    {
                    await ApplyDiscountAsync(id, product.DiscountPercentage);
                }
            }

            StateHasChanged();
        }

        private void ViewHistoryAsync(int id)
        {
            // To be implemented
        }
    }
}
