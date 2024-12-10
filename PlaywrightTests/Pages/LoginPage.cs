using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Helpers;

namespace PlaywrightTests.Pages;

    public class LoginPage
    {
        private readonly IPage Page;
        private readonly ILocator usernameInput;
        private readonly ILocator passwordInput;
        private readonly ILocator loginButton;
        private readonly ILocator title;
        private readonly ILocator menuButton;
        private readonly ILocator logoutButton;
        
        public LoginPage(IPage page)
        {
            this.Page = page;
            this.usernameInput = Page.Locator("[data-test=\"username\"]");
            this.passwordInput = Page.Locator("[data-test=\"password\"]");
            this.loginButton = Page.Locator("[data-test=\"login-button\"]");
            this.title = Page.Locator("[data-test=\"title\"]");
            this.menuButton = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
            this.logoutButton = Page.Locator("[data-test=\"logout-sidebar-link\"]");
        }
        
        public async Task EnterUsernameAsync(string username)
        {
            await usernameInput.FillAsync(username);
        }
        public async Task EnterPasswordAsync(string password)
        {
            await passwordInput.FillAsync(password);
        }

        public async Task ClickLoginBtnAsync()
        {
            await loginButton.ClickAsync();
        }

        public async Task Login(string username, string password)
        {
            await Page.GotoAsync("https://www.saucedemo.com/");
            await EnterUsernameAsync(username);
            await EnterPasswordAsync(password);
            await ClickLoginBtnAsync();
            await VerifyLogin();
        }

        public async Task VerifyLogin()
        {
            await Helpers.AssertionHelper.AssertTextContainsAsync(title, "Products");
            await Helpers.AssertionHelper.AssertVisibleAsync(title, 20_000);
          // await Assertions.Expect(title).ToBeVisibleAsync(new() { Timeout= 10_000});
        }

        public async Task LogoutAsync()
        {
            await menuButton.ClickAsync();
            await logoutButton.ClickAsync();
            await Helpers.AssertionHelper.AssertVisibleAsync(loginButton, 20_000);
           // await Assertions.Expect(loginButton).ToBeVisibleAsync();
        }
    }
