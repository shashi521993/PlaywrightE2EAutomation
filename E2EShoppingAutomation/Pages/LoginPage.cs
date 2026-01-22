using Microsoft.Playwright;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public class LoginPage : BasePage
    {
        private ILocator LoginMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Log in" });
        private ILocator EmailField => Page.Locator("#Email");
        private ILocator PasswordField => Page.Locator("#Password");
        private ILocator LoginSubmitButton => Page.Locator("input.login-button");

        public LoginPage(IPage page) : base(page) { }

        public async Task LoginAsync(string email, string password)
        {
            await LoginMenuLink.ClickAsync();

            await EmailField.FillAsync(email);
            await PasswordField.FillAsync(password);

            await LoginSubmitButton.ClickAsync();

            await Microsoft.Playwright.Assertions.Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Log out" })).ToBeVisibleAsync(new() { Timeout = 15000 });
            await TakeScreenshot("Login_Attempt");
        }
    }
}
