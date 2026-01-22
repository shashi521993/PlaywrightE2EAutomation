using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace E2EShoppingAutomation.Pages
{
    public class CartPage : BasePage
    {
        // This targets the actual number value in the cart summary
        private ILocator CartTotalValue => Page.Locator(".order-summary-content .product-price").Last;
        private ILocator ShoppingCartLink => Page.GetByRole(AriaRole.Link, new() { Name = "Shopping cart" });

        public CartPage(IPage page) : base(page) { }

        /// <summary>
        /// Requirement 4.3: Asserts that the cart total does not exceed the budget threshold
        /// </summary>
        public async Task AssertCartTotalNotExceeds(decimal budgetPerItem, int itemsCount)
        {
            // Fix: Using Force=true to click even if the green notification bar covers the link
            await ShoppingCartLink.First.ClickAsync(new() { Force = true });
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Check if cart is empty
            if (itemsCount == 0)
            {
                TestContext.WriteLine("Cart is empty as expected. Budget check skipped.");
                await TakeScreenshot("Empty_Cart_Validation");
                return;
            }

            string totalText = await CartTotalValue.InnerTextAsync();
            decimal actualTotal = ParsePrice(totalText);
            decimal threshold = budgetPerItem * itemsCount;

            await TakeScreenshot("Cart_Budget_Validation");

            TestContext.WriteLine($"Budget Check: Total {actualTotal} vs Threshold {threshold}");

            Assert.That(actualTotal, Is.LessThanOrEqualTo(threshold), "Budget exceeded!");
        }

        public async Task ClearCartAsync()
        {
            await ShoppingCartLink.First.ClickAsync(new() { Force = true });
            var updateCartButton = Page.GetByRole(AriaRole.Button, new() { Name = "Update shopping cart" });

            if (await updateCartButton.IsVisibleAsync())
            {
                var removeCheckboxes = await Page.Locator("input[name='removefromcart']").AllAsync();
                foreach (var checkbox in removeCheckboxes)
                {
                    await checkbox.CheckAsync();
                }
                await updateCartButton.ClickAsync();
            }
        }

        private decimal ParsePrice(string priceText)
        {
            var cleanPrice = Regex.Replace(priceText, @"[^0-9\.]", "");
            return decimal.TryParse(cleanPrice, out decimal result) ? result : 0;
        }
    }
}
