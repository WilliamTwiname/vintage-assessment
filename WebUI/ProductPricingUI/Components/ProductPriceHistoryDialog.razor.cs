using Microsoft.AspNetCore.Components;
using ProductPricingUI.Models;

namespace ProductPricingUI.Components
{
    public partial class ProductPriceHistoryDialog
    {
        [Parameter]
        public List<ProductPriceHistoryRecordDto> History { get; set; } = [];
    }
}