using Allure.NUnit;
using E2EShoppingAutomation.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Ignore("one test run only")]
    public class ProductTests : PageTest
    {
        private SearchPage _searchPage;
        private ProductPage _productPage;
        private JObject _configData;

        [SetUp]
        public async Task Setup()
        {
            // Robust pathing to load the config file
            string configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Config", "appsettings.json");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Configuration file not found at: {configPath}");
            }

            _configData = JObject.Parse(await File.ReadAllTextAsync(configPath));

            // Navigate to base URL from your config
            string baseUrl = _configData["baseUrl"]?.ToString() ?? "https://demowebshop.tricentis.com";
            await Page.GotoAsync(baseUrl);

            _searchPage = new SearchPage(Page);
            _productPage = new ProductPage(Page);
        }

        [Test]
        [Description("Requirement 4.2: Add multiple items to cart from URL list")]
        public async Task AddMultipleItemsToCart_ShouldSucceed()
        {
            // 1. Get search results (URLs)
            string query = _configData["searchQuery"]?.ToString() ?? "Computer";
            decimal maxPrice = _configData["maxPrice"]?.Value<decimal>() ?? 1200m;
            int limit = _configData["itemsLimit"]?.Value<int>() ?? 3;

            var productUrls = await _searchPage.SearchItemsByNameUnderPrice(query, maxPrice, limit);
            Assert.That(productUrls.Count, Is.GreaterThan(0), "No products found to add.");

            // 2. Add all found items to cart using the required function signature
            await _productPage.AddItemsToCart(productUrls);

            // 3. Final Verification: Check if cart quantity matches the number of URLs added
            await Expect(Page.Locator(".cart-qty")).ToContainTextAsync($"({productUrls.Count})", new() { Timeout = 10000 });


            await _productPage.TakeScreenshot("Cart_Updated_Success");
        }

        [Test]
        [Description("Requirement 4.2: Add multiple items to cart from URL list")]
        public async Task AddMultipleItemsToCart_ShouldSucceed2()
        {
            // 1. Get search results (URLs)
            string query = _configData["searchQuery"]?.ToString() ?? "Computer";
            decimal maxPrice = _configData["maxPrice"]?.Value<decimal>() ?? 1200m;
            int limit = _configData["itemsLimit"]?.Value<int>() ?? 3;

            var productUrls = await _searchPage.SearchItemsByNameUnderPrice(query, maxPrice, limit);
            Assert.That(productUrls.Count, Is.GreaterThan(0), "No products found to add.");

            // 2. Add all found items to cart using the required function signature
            await _productPage.AddItemsToCart(productUrls);

            // 3. Final Verification: Check if cart quantity matches the number of URLs added
            await Expect(Page.Locator(".cart-qty")).ToContainTextAsync($"({productUrls.Count})", new() { Timeout = 10000 });


            await _productPage.TakeScreenshot("Cart_Updated_Success");
        }
    }
}
