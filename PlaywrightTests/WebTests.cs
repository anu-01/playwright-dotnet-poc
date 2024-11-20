using PlaywrightTests.Pages;
using PlaywrightTests.Reporting;


[TestFixture]
public class WebTests : ReportGenerator
{
    private LoginPage loginPage;
    private CartPage cartPage;
    private CheckoutPage checkoutPage;

    [SetUp]
    public void SetUp()
    {
        loginPage = new LoginPage(Page);
        cartPage = new CartPage(Page);
        checkoutPage = new CheckoutPage(Page);
    }

    [Test]
    [CategoryAttribute("End to End Test to verify placing an order successfully")]
    [CategoryAttribute("priority=1")]
    [Author("johndoe")]
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
        await Page.CloseAsync();
    }
}