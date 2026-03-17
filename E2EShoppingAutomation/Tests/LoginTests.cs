using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using E2EShoppingAutomation.Pages;
using System.IO;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Tests
{
    [TestFixture]
    public class LoginTests : PageTest
    {
        private string _baseUrl;
        private string _email;
        private string _password;

        [SetUp]
        public void LoadTestData()
        {
            // TestDirectory points to the bin folder where the app actually runs
            string baseDirectory = TestContext.CurrentContext.TestDirectory;

            string configPath = Path.Combine(baseDirectory, "Config", "appsettings.json");
            string credsPath = Path.Combine(baseDirectory, "Config", "credentials.json");

            var config = JObject.Parse(File.ReadAllText(configPath));
            _baseUrl = config["baseUrl"]?.ToString() ?? "https://demowebshop.tricentis.com";

            var creds = JObject.Parse(File.ReadAllText(credsPath));
            _email = creds["email"]?.ToString();
            _password = creds["password"]?.ToString();
        }

        [Test]
        public async Task Login_ShouldWorkSuccessfully()
        {
            await Page.GotoAsync(_baseUrl);

            var loginPage = new LoginPage(Page);
            await loginPage.LoginAsync(_email, _password);

            // Verified indicator for successful login on DemoWebShop
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Log out" })).ToBeVisibleAsync(new()
            {
                Timeout = 10000
            });

            await loginPage.TakeScreenshot("Login_Success");
        }

        [Test]
        public async Task Login_ShouldWorkSuccessfully2()
        {
            await Page.GotoAsync(_baseUrl);

            var loginPage = new LoginPage(Page);
            await loginPage.LoginAsync(_email, _password);

            // Verified indicator for successful login on DemoWebShop
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Log out" })).ToBeVisibleAsync(new()
            {
                Timeout = 10000
            });

            await loginPage.TakeScreenshot("Login_Success");
        }
    }
}
