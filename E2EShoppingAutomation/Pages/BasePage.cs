using Microsoft.Playwright;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page)
        {
            Page = page;
        }

        // מחכה שהדף יטען
        public async Task WaitForPageLoad()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // קליק על אלמנט כשנראה
        public async Task ClickWhenVisible(string selector)
        {
            await Page.WaitForSelectorAsync(selector);
            await Page.ClickAsync(selector);
        }

        // למלא שדה טקסט
        public async Task FillInput(string selector, string text)
        {
            await Page.WaitForSelectorAsync(selector);
            await Page.FillAsync(selector, text);
        }

        // צילומי מסך עם שם קובץ מותאם
        public async Task TakeScreenshot(string filename)
        {
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = filename,
                FullPage = true
            });
        }

        // שליפת טקסט מאלמנט
        public async Task<string> GetElementText(string selector)
        {
            await Page.WaitForSelectorAsync(selector);
            var element = await Page.QuerySelectorAsync(selector);
            return await element.InnerTextAsync();
        }
    }
}
