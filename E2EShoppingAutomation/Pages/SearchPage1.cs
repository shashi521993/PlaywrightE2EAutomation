using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public class SearchPage1 : BasePage
    {
        public SearchPage1(IPage page) : base(page)
        {
        }

        private string SearchInputSelector => "input#small-searchterms";
        private string SearchButtonSelector => "button.search-box-button";
        private string ProductListSelector => "div.product-item";
        private string ProductNameSelector => ".product-title a";
        private string ProductPriceSelector => ".prices .price";

        /// <summary>
        /// חיפוש מוצרים לפי שם, מחזיר עד N URLs שעומדים בתנאי המחיר
        /// </summary>
        public async Task<List<string>> SearchItemsByNameUnderPrice(string query, decimal maxPrice, int limit = 5)
        {
            await ExecuteSearch(query);
            List<string> resultUrls = new List<string>();

            //await SearchForProduct(query);
            //await ExecuteSearch(query);
            //await WaitForPageLoad();

            var products = await Page.QuerySelectorAllAsync(ProductListSelector);

            foreach (var product in products)
            {
                var priceText = await product.QuerySelectorAsync(ProductPriceSelector);
                decimal price = decimal.Parse((await priceText.InnerTextAsync()).Replace("$", "").Trim());

                if (price <= maxPrice)
                {
                    var urlElem = await product.QuerySelectorAsync(ProductNameSelector);
                    string url = await urlElem.GetAttributeAsync("href");
                    resultUrls.Add(url);
                    Console.WriteLine($"Found product: {url} at price {price}");
                    System.Diagnostics.Debug.WriteLine($"Found product: {url} at price {price}");
                }

                if (resultUrls.Count >= limit)
                    break;
            }

            return resultUrls;
        }
        public async Task SearchForProduct(string query)
        {
            await FillInput(SearchInputSelector, query);        // מזין את המוצר
            await Page.WaitForSelectorAsync(SearchButtonSelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
            await ClickWhenVisible(SearchButtonSelector);       // לוחץ על כפתור החיפוש
            await WaitForPageLoad();                             // מחכה שהדף יטען
        }

        public async Task ExecuteSearch(string query)
        {
            await FillInput(SearchInputSelector, query); // מזין את המוצר
            await Page.WaitForSelectorAsync(SearchButtonSelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await ClickWhenVisible(SearchButtonSelector); // לוחץ על הכפתור
            await WaitForPageLoad();                     // מחכה שהדף נטען
        }
    }
}
