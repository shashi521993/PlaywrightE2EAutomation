using Microsoft.Playwright;

namespace E2EShoppingAutomation.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page) => Page = page;

        public async Task ClickWhenVisible(string selector)
        {
            await Page.Locator(selector).First.ClickAsync();
        }

        public async Task FillInput(string selector, string text)
        {
            await Page.Locator(selector).First.FillAsync(text);
        }

        public async Task TakeScreenshot(string fileName)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string screenshotFolder = Path.Combine(projectRoot, "Screenshots");

            if (!Directory.Exists(screenshotFolder))
            {
                Directory.CreateDirectory(screenshotFolder);
            }

            string filePath = Path.Combine(screenshotFolder, $"{fileName}_{DateTime.Now:HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = filePath });
            TestContext.WriteLine($"Screenshot saved: {filePath}");
        }


        public async Task WaitForNetworkIdle() =>
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);


        public async Task WaitForPageLoad() => await WaitForNetworkIdle();
    }
}
