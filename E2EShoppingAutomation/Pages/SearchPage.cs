using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public class SearchPage : BasePage
    {
        public SearchPage(IPage page) : base(page) { }

        // Updated Locators based on your findings
        private ILocator SearchInput => Page.Locator("#small-searchterms");
        private ILocator SearchButton => Page.GetByRole(AriaRole.Button, new() { Name = "Search" });
        private ILocator NextPageButton => Page.Locator(".next-page a");

        private string ProductItemSelector = ".product-item";
        private string PriceSelector = ".actual-price";
        private string TitleLinkSelector = ".product-title a";

        public async Task<List<string>> SearchItemsByNameUnderPrice(string query, decimal maxPrice, int limit = 3)
        {
            var resultUrls = new List<string>();
            await SearchInput.FillAsync(query);
            await SearchButton.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            bool hasMorePages = true;

            while (resultUrls.Count < limit && hasMorePages)
            {
                await Page.WaitForSelectorAsync(ProductItemSelector, new() { Timeout = 5000 });
                var productItems = await Page.Locator(ProductItemSelector).AllAsync();

                foreach (var item in productItems)
                {
                    if (resultUrls.Count >= limit) break;

                    var priceText = await item.Locator(PriceSelector).InnerTextAsync();
                    decimal price = ParsePrice(priceText);

                    if (price > 0 && price <= maxPrice)
                    {
                        var url = await item.Locator(TitleLinkSelector).GetAttributeAsync("href");
                        if (!string.IsNullOrEmpty(url))
                        {
                            var fullUrl = url.StartsWith("http") ? url : $"https://demowebshop.tricentis.com{url}";
                            resultUrls.Add(fullUrl);
                        }
                    }
                }

                if (resultUrls.Count < limit && await NextPageButton.IsVisibleAsync())
                {
                    await NextPageButton.ClickAsync();
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
                else
                {
                    hasMorePages = false;
                }
            }
            return resultUrls;
        }

        private decimal ParsePrice(string priceText)
        {
            var cleanPrice = Regex.Replace(priceText, @"[^\d\.]", "");
            return decimal.TryParse(cleanPrice, out decimal result) ? result : 0;
        }
    }
}
