using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace E2EShoppingAutomation.Pages
{
    public class ProductPage : BasePage
    {
        // Generic selector: Taking the first visible "Add to cart" button on the page
        private ILocator AddToCartButton => Page.GetByRole(AriaRole.Button, new() { Name = "Add to cart" }).First;

        private ILocator SuccessNotification => Page.Locator(".bar-notification.success");

        // Generic selector for any mandatory attributes (Dropdowns, Radio buttons, Checkboxes)
        private ILocator ProductAttributes => Page.Locator(".attributes select, .attributes input[type='radio'], .attributes input[type='checkbox']");

        public ProductPage(IPage page) : base(page) { }

        public async Task AddItemsToCart(List<string> urls)
        {
            foreach (var url in urls)
            {
                await Page.GotoAsync(url);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Generic Logic: Find all possible options and select the first available for each
                var attributes = await ProductAttributes.AllAsync();
                foreach (var attribute in attributes)
                {
                    if (await attribute.IsVisibleAsync())
                    {
                        string tagName = await attribute.EvaluateAsync<string>("node => node.tagName");

                        if (tagName == "SELECT")
                        {
                            // Select the second option (index 1) because index 0 is usually "Select size"
                            await attribute.SelectOptionAsync(new[] { new SelectOptionValue { Index = 1 } });
                        }
                        else
                        {
                            // Click for Radio buttons or Checkboxes
                            await attribute.ClickAsync(new() { Force = true });
                        }
                    }
                }

                // Click the main Add to cart button
                await AddToCartButton.ClickAsync();

                // Wait for success bar
                await Expect(SuccessNotification).ToBeVisibleAsync(new() { Timeout = 10000 });

                await TakeScreenshot($"Item_Added_{urls.IndexOf(url)}");
            }
        }
    }
}
