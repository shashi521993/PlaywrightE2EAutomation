using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace E2EShoppingAutomation.Tests
{
    //[TestClass]
    public class BrowserLaunchDebugTests
    {
        //[TestMethod]
        public async Task Debug_OpenSite_WithLogs()
        {
            Debug.WriteLine("=== START TEST ===");

            using var playwright = await Playwright.CreateAsync();
            Debug.WriteLine("Playwright created");

            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 300
            });
            Debug.WriteLine("Browser launched");

            var context = await browser.NewContextAsync();
            Debug.WriteLine("Browser context created");

            var page = await context.NewPageAsync();
            Debug.WriteLine("New page created");

            // ===== DEBUG EVENTS =====
            page.Load += (_, _) =>
            {
                Debug.WriteLine("EVENT: Page Load fired");
            };

            page.DOMContentLoaded += (_, _) =>
            {
                Debug.WriteLine("EVENT: DOMContentLoaded fired");
            };

            page.RequestFailed += (_, request) =>
            {
                Debug.WriteLine($"REQUEST FAILED: {request.Url}");
            };

            page.PageError += (_, error) =>
            {
                Debug.WriteLine($"PAGE ERROR: {error}");
            };

            try
            {
                Debug.WriteLine("Navigating to site...");
                await page.GotoAsync("https://demowebshop.tricentis.com/", new PageGotoOptions
                {
                    Timeout = 60000,
                    WaitUntil = WaitUntilState.Load
                });

                Debug.WriteLine("GotoAsync finished");

                // בדיקה מפורשת של ה־URL
                Debug.WriteLine($"Current URL: {page.Url}");

                // צילום מסך – גם אם זה ריק
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = "debug_screenshot.png"
                });

                Debug.WriteLine("Screenshot taken");

                await page.WaitForTimeoutAsync(10000);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION THROWN:");
                Debug.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                Debug.WriteLine("Closing browser");
                await browser.CloseAsync();
            }

            Debug.WriteLine("=== END TEST ===");
        }
    }
}
