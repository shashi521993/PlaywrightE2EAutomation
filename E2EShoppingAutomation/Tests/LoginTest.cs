using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using E2EShoppingAutomation.Pages;

namespace E2EShoppingAutomation.Tests
{
    //[TestClass]
    public class LoginTest : PageTest
    {
        //[TestMethod]
        public async Task Login_ShouldWork()
        {
            // 1️⃣ קריאה מהקובץ JSON של ה־URL
            var json = File.ReadAllText("Config/appsettings.json");
            var config = JObject.Parse(json);
            string baseUrl = config["baseUrl"].ToString();

            // 2️⃣ פתיחת הדף
            await Page.GotoAsync(baseUrl);

            // 3️⃣ קריאה ל־JSON של האימייל והסיסמה
            var credsJson = File.ReadAllText("TestData/credentials.json");
            var creds = JObject.Parse(credsJson);
            string email = creds["email"].ToString();
            string password = creds["password"].ToString();

            // 4️⃣ יצירת Page Object והתחברות
            var loginPage = new LoginPage(Page);
            await loginPage.LoginAsync(email, password);

            // 5️⃣ Assertion בסיסי
            Assert.IsTrue(await Page.IsVisibleAsync(".account")); // לדוגמה אלמנט שמופיע אחרי Login

            // 5️⃣ שמירת Screenshot
            await loginPage.TakeScreenshot("login_success.png");
        }
    }
}

