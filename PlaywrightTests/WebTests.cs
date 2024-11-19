using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;
using PlaywrightTests.Pages;

[TestFixture]
public class WebTests : PageTest
{
    private LoginPage loginPage;
    private CartPage cartPage;
    private CheckoutPage checkoutPage;

    [SetUp]
    public async Task SetUp()
    {
        loginPage = new LoginPage(Page);
        cartPage = new CartPage(Page);
        checkoutPage = new CheckoutPage(Page);
    }

    [Test]
    public async Task CartCheckout()
    {
        await loginPage.Login("standard_user", "secret_sauce");
        await loginPage.VerifyLogin();
        await cartPage.AddToCartAsync("sauce-labs-backpack");
        await cartPage.GoToCartAsync();
        await checkoutPage.CheckoutAsync();
        await checkoutPage.EnterCheckoutDetailsAsync("John", "Doe", "12345");
        await checkoutPage.VerifySummary("Sauce Labs Backpack", "1", "$29.99", "$32.39");
        await checkoutPage.ConfirmOrderAsync();
        await checkoutPage.VerifyOrderConfirmation();
        await checkoutPage.GoBackToProductsAsync();
        await loginPage.LogoutAsync(); 
    }

    [TearDown]
    public async Task TearDown()
    {
        Page.CloseAsync();
    }
}