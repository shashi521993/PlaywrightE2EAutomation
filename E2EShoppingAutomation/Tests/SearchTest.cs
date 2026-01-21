using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using E2EShoppingAutomation.Pages;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace E2EShoppingAutomation.Tests
{
    //[TestClass]
    public class SearchPageTests
    {
        private IBrowser? _browser;
        private IPage? _page;

        //[TestInitialize]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // כדי לראות את הדפדפן
                SlowMo = 5000      // עיכוב קטן כדי לראות מה קורה
            });

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        //[TestCleanup]
        public async Task Cleanup()
        {
            if (_browser != null)
                await _browser.CloseAsync();
        }

        //[TestMethod]
        public async Task SearchItemsByNameUnderPrice_ShouldReturnProducts()
        {
            var searchPage = new SearchPage(_page!);

            // פתח את האתר
            await _page!.GotoAsync("https://demowebshop.tricentis.com/");


            try
            {
                // חפש מוצרים בשם "computer" עד מחיר 1000$
                List<string> results = await searchPage.SearchItemsByNameUnderPrice("Simple Computer", 1000, 5);

                // הדפס תוצאה לקונסול
                foreach (var url in results)
                {
                    Debug.WriteLine($"Test found product URL: {url}");
                    Debug.WriteLine($"Test found product URL: {url}");
                }

                // בדיקה בסיסית: מצא לפחות מוצר אחד
                Assert.IsNotEmpty(results, "No products found under the given price.");

                string currentUrl = _page!.Url;
                Assert.Contains("search", currentUrl, "Did not navigate to search results page.");

                var products = await _page.QuerySelectorAllAsync("div.product-item");
                Assert.IsNotEmpty(products, "Search page loaded but no products were displayed.");


                await Task.Delay(10000);

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("❌ TEST FAILED WITH EXCEPTION:");
                Debug.WriteLine(ex.ToString());

                Console.WriteLine("❌ TEST FAILED WITH EXCEPTION:");
                Console.WriteLine(ex);

                // ניסיון לצילום מסך לפני סגירת הדפדפן
                if (_page != null)
                {
                    try
                    {
                        await _page.ScreenshotAsync(new PageScreenshotOptions
                        {
                            Path = "SearchTest_Failure.png",
                            FullPage = true
                        });

                        Debug.WriteLine("📸 Screenshot saved: SearchTest_Failure.png");
                    }
                    catch (System.Exception screenshotEx)
                    {
                        Debug.WriteLine("⚠ Failed to take screenshot:");
                        Debug.WriteLine(screenshotEx.ToString());
                    }
                }

                throw; // ❗ חשוב – אחרת הטסט ייחשב ירוק
            }

        }
    }
}
