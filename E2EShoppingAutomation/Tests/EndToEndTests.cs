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
    public class EndToEndTests : PageTest
    {
        private JObject _config= null!;
        private JObject _creds=null!;

        [SetUp]
        public async Task Setup()
        {
            // Requirement 15% Data-Driven: Loading from Config folder
            string configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Config", "appsettings.json");
            string credsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Config", "credentials.json");

            _config = JObject.Parse(await File.ReadAllTextAsync(configPath));
            _creds = JObject.Parse(await File.ReadAllTextAsync(credsPath));

            await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });

            await Page.GotoAsync(_config["baseUrl"].ToString());
        }

        [Test]
        [Description("Full E2E Scenario: Login -> Search -> Add to Cart -> Budget Validation")]
        public async Task FullShoppingFlow_E2E()
        {
            //POM Initialization
            var loginPage = new LoginPage(Page);
            var searchPage = new SearchPage(Page);
            var productPage = new ProductPage(Page);
            var cartPage = new CartPage(Page);

            TestContext.WriteLine("Starting E2E Flow...");

            // Step 1: Login
            TestContext.WriteLine("Step 1: Performing Login");
            await loginPage.LoginAsync(_creds["email"].ToString(), _creds["password"].ToString());

            // Clear cart
            await cartPage.ClearCartAsync();

            // Step 2: Search with Price Filtering & Paging
            string query = _config["searchQuery"].ToString();
            decimal maxPrice = _config["maxPrice"].Value<decimal>();
            int limit = _config["itemsLimit"].Value<int>();

            TestContext.WriteLine($"Step 2: Searching for '{query}' under {maxPrice}");
            var productUrls = await searchPage.SearchItemsByNameUnderPrice(query, maxPrice, limit);

            TestContext.WriteLine($"Found {productUrls.Count} items matching criteria.");

            // Step 3: Add all found items to cart
            if (productUrls.Count > 0)
            {
                TestContext.WriteLine("Step 3: Adding items to cart and handling variants");
                await productPage.AddItemsToCart(productUrls);
            }

            // Step 4: Budget Validation
            decimal budgetPerItem = _config["budgetPerItem"].Value<decimal>();
            TestContext.WriteLine($"Step 4: Validating total cart budget (Limit per item: {budgetPerItem})");

            await cartPage.AssertCartTotalNotExceeds(budgetPerItem, productUrls.Count);

            TestContext.WriteLine("E2E Scenario Completed Successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string reportsFolder = Path.Combine(projectRoot, "TestReports");

            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }

            string tracePath = Path.Combine(reportsFolder, $"trace_{DateTime.Now:HHmmss}.zip");

            await Context.Tracing.StopAsync(new() { Path = tracePath });

            TestContext.WriteLine($"Trace Report saved to: {tracePath}");
            TestContext.WriteLine("To view: Drag the zip file to https://trace.playwright.dev");
        }
    }
}

