using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using E2EShoppingAutomation.Pages;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Tests
{
    [TestFixture]
    public class CartTests : PageTest
    {
        [Test]
        [Description("Requirement 4.3: Verify the total cart amount does not exceed the calculated budget")]
        public async Task AssertCartTotalNotExceeds_ShouldPass()
        {
            // Arrange
            // We must add an item to the cart first to test the cart logic
            string specificProductUrl = "https://demowebshop.tricentis.com";
            decimal expectedPricePerItem = 10.00M; // This item costs exactly 10.00
            int quantity = 1;

            var productPage = new ProductPage(Page);

            // Navigate and add a known item to the cart
            await Page.GotoAsync(specificProductUrl);
            await Page.Locator("input[id^='add-to-cart-button']").ClickAsync();
            await Page.WaitForSelectorAsync("#bar-notification", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            // Act & Assert (using the CartPage logic)
            var cartPage = new CartPage(Page);
            // This method contains the assertion logic and the screenshot capture
            await cartPage.AssertCartTotalNotExceeds(expectedPricePerItem, quantity);
        }
    }
}
