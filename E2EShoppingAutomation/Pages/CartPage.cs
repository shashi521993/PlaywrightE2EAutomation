using Microsoft.Playwright;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace E2EShoppingAutomation.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IPage page) : base(page) { }

        // Selectors for DemoWebShop
        private ILocator TotalPriceLabel => Page.Locator(".order-total strong");
        private ILocator ShoppingCartLink => Page.Locator(".header-links .ico-cart");

        /// <summary>
        /// Navigates to the cart, calculates total budget vs actual price, and asserts result.
        /// (Requirement 4.3)
        /// </summary>
        public async Task AssertCartTotalNotExceeds(decimal budgetPerItem, int itemsCount)
        {
            // 1. Open the cart
            await ShoppingCartLink.ClickAsync();
            await WaitForNetworkIdle();

            // 2. Extract and parse the actual total price from the UI
            var totalText = await TotalPriceLabel.InnerTextAsync();
            decimal actualTotal = ParsePrice(totalText);

            // 3. Calculate the threshold based on the data-driven inputs
            decimal maxAllowedBudget = budgetPerItem * itemsCount;

            // 4. Requirement 4.3: Take screenshot of the cart page
            await TakeScreenshot("Final_Cart_Summary");

            // 5. Assertion
            Assert.That(actualTotal, Is.LessThanOrEqualTo(maxAllowedBudget),
                $"Budget exceeded! Cart Total: {actualTotal}, Max Allowed: {maxAllowedBudget}");

            Console.WriteLine($"Assertion Success: Total {actualTotal} is within budget of {maxAllowedBudget}");
        }

        private decimal ParsePrice(string priceText)
        {
            // Removes currency symbols and commas using Regex
            var cleanPrice = Regex.Replace(priceText, @"[^\d\.]", "");
            return decimal.TryParse(cleanPrice, out decimal result) ? result : 0;
        }
    }
}
