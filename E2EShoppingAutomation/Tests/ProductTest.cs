using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using E2EShoppingAutomation.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Tests
{
    [TestClass]
    public class ProductPageTests
    {
        private IBrowser _browser;
        private IPage _page;

        [TestInitialize]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 300
            });

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _browser.CloseAsync();
        }

        [TestMethod]
        public async Task AddItemsToCart_ShouldClickButtonAndUpdateCart()
        {
            var productPage = new ProductPage(_page);

            // URL של מוצר לבדיקה
            var urls = new List<string>
            {
                "https://demowebshop.tricentis.com/simple-computer"
            };

            // הרצת הפונקציה
            await productPage.AddItemsToCart(urls);

            // המתנה קצרה כדי לראות את העגלה עודכן
            await Task.Delay(5000);
        }
    }
}
