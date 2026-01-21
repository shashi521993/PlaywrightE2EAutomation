using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace E2EShoppingAutomation.Pages
{
    public class ProductPage : BasePage
    {
        public ProductPage(IPage page) : base(page) { }

        private string AddToCartButton => "button#add-to-cart-button";
        private string VariantDropdowns => "select";  // וריאנטים אם קיימים
        private string CartCountSelector => "span.cart-qty"; // ספרת העגלה

        /// <summary>
        /// מוסיף רשימת מוצרים לעגלה, מחכה עד שהעגלה משתנה, ושומר צילום מסך
        /// </summary>
        public async Task AddItemsToCart(List<string> productUrls)
        {
            int index = 1;

            foreach (var url in productUrls)
            {
                await Page.GotoAsync(url);
                await WaitForPageLoad();

                // בחר וריאנטים אם קיימים
                var dropdowns = await Page.QuerySelectorAllAsync(VariantDropdowns);
                var random = new Random();

                foreach (var dropdown in dropdowns)
                {
                    var options = await dropdown.QuerySelectorAllAsync("option");
                    if (options.Count > 1)
                    {
                        int randomIndex = random.Next(1, options.Count);
                        var value = await options[randomIndex].GetAttributeAsync("value");
                        await dropdown.SelectOptionAsync(value);
                    }
                }

                // ספרת העגלה לפני ההוספה
                var cartBeforeText = await Page.InnerTextAsync(CartCountSelector);
                int cartBefore = int.Parse(cartBeforeText.Replace("(", "").Replace(")", "").Trim());

                // כאן נוודא שהלחיצה על Add to Cart מתבצעת בפועל
                var addButton = await Page.QuerySelectorAsync(AddToCartButton);
                if (addButton != null)
                {
                    // Scroll to the button to ensure visibility
                    await addButton.ScrollIntoViewIfNeededAsync();
                    // לחיצה על הכפתור
                    await addButton.ClickAsync();

                    // חזק – ודא שהלחיצה באמת מבוצעת עם Enter (כמו החיפוש)
                    await Page.Keyboard.PressAsync("Enter");
                }

                // המתנה עד שהעגלה תשתנה, עד 10 שניות
                try
                {
                    await Page.WaitForFunctionAsync($@"() => {{
                        const cart = document.querySelector('{CartCountSelector}');
                        return cart && parseInt(cart.innerText.replace('(', '').replace(')', '').trim()) > {cartBefore};
                    }}", new PageWaitForFunctionOptions { Timeout = 10000 });
                }
                catch
                {
                    // אם לא השתנה, נמשיך – צילום המסך יישמר בכל מקרה
                }

                // צילום מסך תמיד
                //await TakeScreenshot($"Screenshots/Added_Item_{index}.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = $"Screenshots/Added_Item_{index}.png",
                    FullPage = true
                });

                // חזרה לדף הקודם
                await Page.GoBackAsync();
                await WaitForPageLoad();

                index++;
            }
        }
    }
}
