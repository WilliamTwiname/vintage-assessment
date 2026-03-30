using Microsoft.AspNetCore.Components;
using ProductPricingUI.Models;

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

    }
}
