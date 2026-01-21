using Microsoft.Playwright;
using System.Threading.Tasks;

namespace E2EShoppingAutomation.Pages
{
    public class LoginPage : BasePage
    {
        private readonly string emailInput = "#Email";
        private readonly string passwordInput = "#Password";
        private readonly string loginButton = "button.login-button";



        public LoginPage(IPage page) : base(page) { }

        public async Task LoginAsync(string email, string password)
        {
            await FillInput(emailInput, email);         // משתמש בפונקציה מה-BasePage
            await FillInput(passwordInput, password);   // משתמש בפונקציה מה-BasePage
            await ClickWhenVisible(loginButton);        // משתמש בפונקציה מה-BasePage
            await WaitForPageLoad();                     // מוודא שהדף נטען
        }
    }
}


