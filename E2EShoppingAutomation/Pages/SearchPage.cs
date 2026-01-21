using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public class SearchPage : BasePage
    {
        public SearchPage(IPage page) : base(page)
        {
        }

        private string SearchInputSelector => "input#small-searchterms";
        private string ProductListSelector => "div.product-item";
        private string ProductNameSelector => ".product-title a";
        private string ProductPriceSelector => ".prices .price";

        /// <summary>
        /// חיפוש מוצרים לפי שם, מחזיר עד N URLs שעומדים בתנאי המחיר
        /// </summary>
        public async Task<List<string>> SearchItemsByNameUnderPrice(
            string productName,
            decimal maxPrice,
            int limit = 5)
        {
            var resultUrls = new List<string>();

            // 1️⃣ כתיבה לשדה החיפוש
            await FillInput(SearchInputSelector, productName);

            // 2️⃣ ENTER – זה מה שמפעיל חיפוש אמיתי באתר
            await Page.Keyboard.PressAsync("Enter");

            // 3️⃣ המתנה לטעינת דף תוצאות
            //await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForSelectorAsync(
                ProductListSelector,
                new PageWaitForSelectorOptions
                {
                    Timeout = 10000
                });


            // 4️⃣ שליפת מוצרים
            var products = await Page.QuerySelectorAllAsync(ProductListSelector);

            foreach (var product in products)
            {
                var priceElement = await product.QuerySelectorAsync(ProductPriceSelector);
                if (priceElement == null)
                    continue;

                var priceText = (await priceElement.InnerTextAsync())
                    .Replace("$", "")
                    .Trim();

                if (!decimal.TryParse(priceText, out var price))
                    continue;

                if (price <= maxPrice)
                {
                    var linkElement = await product.QuerySelectorAsync(ProductNameSelector);
                    var url = await linkElement.GetAttributeAsync("href");

                    resultUrls.Add(url);
                    Console.WriteLine($"✔ Found product: {url} | Price: {price}");
                }

                if (resultUrls.Count >= limit)
                    break;
            }

            return resultUrls;
        }
    }
}
