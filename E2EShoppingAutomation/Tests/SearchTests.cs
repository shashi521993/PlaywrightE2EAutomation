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
    public class SearchTests : PageTest
    {
        private SearchPage _searchPage;
        private JObject _configData;

        [SetUp]
        public async Task Setup()
        {
            // Pointing to your existing Config folder and appsettings.json
            string configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Config", "appsettings.json");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Configuration file not found at: {configPath}");
            }

            var jsonContent = await File.ReadAllTextAsync(configPath);
            _configData = JObject.Parse(jsonContent);

            // Navigate to base URL from your config
            string baseUrl = _configData["baseUrl"]?.ToString() ?? "https://demowebshop.tricentis.com";
            await Page.GotoAsync(baseUrl);

            _searchPage = new SearchPage(Page);
        }

        [Test]
        [Description("Requirement 4.1: Search products and return up to limit results, 0 is a valid result")]
        public async Task SearchItemsUnderPrice_ShouldReturnFilteredResults()
        {
            // Arrange
            string query = _configData["searchQuery"]?.ToString() ?? "shoes";
            decimal maxPrice = _configData["maxPrice"]?.Value<decimal>() ?? 20m;
            int limit = _configData["itemsLimit"]?.Value<int>() ?? 5;

            // Act
            var results = await _searchPage.SearchItemsByNameUnderPrice(query, maxPrice, limit);

            // Assert
            Assert.That(results.Count, Is.LessThanOrEqualTo(limit),
                $"Expected up to {limit} results, but found {results.Count}.");

            if (results.Count == 0)
            {
                TestContext.WriteLine($"Search completed successfully: No products found for '{query}' under {maxPrice}. (Valid Result)");
            }
            else
            {
                foreach (var url in results)
                {
                    TestContext.WriteLine($"Product found within budget: {url}");
                }
            }

            await _searchPage.TakeScreenshot("Search_Execution_Finished");
        }

        [Test]
        [Description("Requirement 4.1: Search products and return up to limit results, 0 is a valid result")]
        public async Task SearchItemsUnderPrice_ShouldReturnFilteredResults2()
        {
            // Arrange
            string query = _configData["searchQuery"]?.ToString() ?? "shoes";
            decimal maxPrice = _configData["maxPrice"]?.Value<decimal>() ?? 20m;
            int limit = _configData["itemsLimit"]?.Value<int>() ?? 5;

            // Act
            var results = await _searchPage.SearchItemsByNameUnderPrice(query, maxPrice, limit);

            // Assert
            Assert.That(results.Count, Is.LessThanOrEqualTo(limit),
                $"Expected up to {limit} results, but found {results.Count}.");

            if (results.Count == 0)
            {
                TestContext.WriteLine($"Search completed successfully: No products found for '{query}' under {maxPrice}. (Valid Result)");
            }
            else
            {
                foreach (var url in results)
                {
                    TestContext.WriteLine($"Product found within budget: {url}");
                }
            }

            await _searchPage.TakeScreenshot("Search_Execution_Finished");
        }
    }
}
