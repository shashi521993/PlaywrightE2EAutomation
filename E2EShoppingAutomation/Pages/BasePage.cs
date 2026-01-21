using Microsoft.Playwright;

namespace E2EShoppingAutomation.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page) => Page = page;

        // פונקציה ללחיצה על אלמנט - כוללת המתנה אוטומטית של פליירייט
        public async Task ClickWhenVisible(string selector)
        {
            await Page.Locator(selector).First.ClickAsync();
        }

        // פונקציה למילוי טקסט
        public async Task FillInput(string selector, string text)
        {
            await Page.Locator(selector).First.FillAsync(text);
        }

        // צילום מסך - דרישת חובה במטלה
        public async Task TakeScreenshot(string stepName)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, $"{stepName}_{DateTime.Now:HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
            Console.WriteLine($"Screenshot saved: {path}");
        }

        // המתנה לטעינת הדף
        public async Task WaitForNetworkIdle() =>
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // פונקציית עזר למקרה שקראת לה בשם אחר בדפים אחרים
        public async Task WaitForPageLoad() => await WaitForNetworkIdle();
    }
}
