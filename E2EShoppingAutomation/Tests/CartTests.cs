using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using E2EShoppingAutomation.Pages;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Tests
{
    [TestFixture]
    public class CartTests : PageTest
    {
        private JObject _config;

        [SetUp]
        public async Task Setup()
        {
            string configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Config", "appsettings.json");
            _config = JObject.Parse(await File.ReadAllTextAsync(configPath));
        }

        [Test]
        [Description("Requirement 4.3: Verify cart total against budget threshold using config data")]
        public async Task AssertCartTotalNotExceeds_ShouldPass()
        {
            // 1. Arrange - Load Data from Config
            string baseUrl = _config["baseUrl"]?.ToString() ?? "https://demowebshop.tricentis.com";
            decimal budgetPerItem = _config["budgetPerItem"]?.Value<decimal>() ?? 1000m;
            string query = _config["searchQuery"]?.ToString() ?? "Computing";

            var searchPage = new SearchPage(Page);
            var productPage = new ProductPage(Page);
            var cartPage = new CartPage(Page);

            // 2. Act - Create a state where the cart has items
            await Page.GotoAsync(baseUrl);

            // Get at least 1 product that fits the budget
            var results = await searchPage.SearchItemsByNameUnderPrice(query, budgetPerItem, 1);
            Assert.That(results.Count, Is.GreaterThan(0), "No products found to test cart total.");

            // Add the found product to cart using our smart ProductPage
            await productPage.AddItemsToCart(results);

            // 3. Assert - Use the required method (4.3)
            // It will navigate to cart, calculate the threshold, and verify the total
            await cartPage.AssertCartTotalNotExceeds(budgetPerItem, results.Count);
        }
    }
}
