using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests.Pages;
  
    public class CheckoutPage
    {
        private readonly IPage Page;
        private readonly ILocator checkoutButton;
        private readonly ILocator firstNameInput;
        private readonly ILocator lastNameInput;
        private readonly ILocator postalCodeInput;
        private readonly ILocator continueButton;
        private readonly ILocator itemName;
        private readonly ILocator itemQuantity;
        private readonly ILocator itemPrice;
        private readonly ILocator total;
        private readonly ILocator confirmOrderButton;
        private readonly ILocator orderConfirmationHeader;
        private readonly ILocator orderConfirmationText;
        private readonly ILocator backToProductsButton;
        private readonly ILocator title;

        public CheckoutPage(IPage page)
        {
            this.Page = page;
            this.checkoutButton = Page.Locator("[data-test=\"checkout\"]");
            this.firstNameInput = Page.Locator("[data-test=\"firstName\"]");
            this.lastNameInput = Page.Locator("[data-test=\"lastName\"]");
            this.postalCodeInput = Page.Locator("[data-test=\"postalCode\"]");
            this.continueButton = Page.Locator("[data-test=\"continue\"]");
            this.itemName = Page.Locator("[data-test=\"inventory-item-name\"]");
            this.itemQuantity = Page.Locator("[data-test=\"item-quantity\"]");
            this.itemPrice = Page.Locator("[data-test=\"inventory-item-price\"]");
            this.total = Page.Locator("[data-test=\"total-label\"]");
            this.confirmOrderButton = Page.Locator("[data-test=\"finish\"]");
            this.orderConfirmationHeader = Page.Locator("[data-test=\"complete-header\"]");
            this.orderConfirmationText = Page.Locator("[data-test=\"complete-text\"]");
            this.backToProductsButton = Page.Locator("[data-test=\"back-to-products\"]");
            this.title = Page.Locator("[data-test=\"title\"]");
        }

        public async Task CheckoutAsync()
        {
            await checkoutButton.ClickAsync();
        }

        public async Task EnterCheckoutDetailsAsync(string firstName, string lastName, string postalCode)
        {
            await EnterFirstNameAsync(firstName);
            await EnterLastNameAsync(lastName);
            await EnterPostalCodeAsync(postalCode);
            await ContinueAsync();
        }

        public async Task EnterFirstNameAsync(string firstName)
        {
            await firstNameInput.FillAsync(firstName);
        }

        public async Task EnterLastNameAsync(string lastName)
        {
            await lastNameInput.FillAsync(lastName);
        }
        
        public async Task EnterPostalCodeAsync(string postalCode)
        {
            await postalCodeInput.FillAsync(postalCode);
        }

        public async Task ContinueAsync()
        {
            await continueButton.ClickAsync();
        }

        public async Task VerifySummary(string itemName, string quantity, string itemprice, string total)
        {
            await Assertions.Expect(title).ToContainTextAsync("Checkout: Overview");
            await VerifyItemName(itemName);
            await VerifyItemQuantity(quantity);
            await VerifyItemPrice(itemprice);
            await VerifyTotal(total);
        }

        public async Task VerifyItemName(string name)
        {
            await Assertions.Expect(itemName).ToContainTextAsync(name);
        }

        public async Task VerifyItemQuantity(string quantity)
        {
            await Assertions.Expect(itemQuantity).ToContainTextAsync(quantity);
        }

        public async Task VerifyItemPrice(string price)
        {
            await Assertions.Expect(itemPrice).ToContainTextAsync(price);
        }

        public async Task VerifyTotal(string value)
        {
            await Assertions.Expect(total).ToContainTextAsync(value);
        }

        public async Task ConfirmOrderAsync()
        {
            await confirmOrderButton.ClickAsync();

        }

        public async Task VerifyOrderConfirmation()
        {
            await Assertions.Expect(title).ToContainTextAsync("Checkout: Complete!");
            await Assertions.Expect(orderConfirmationHeader).ToContainTextAsync("Thank you for your order!");
            await Assertions.Expect(orderConfirmationText).ToContainTextAsync("Your order has been dispatched, and will arrive just as fast as the pony can get there!");

        }

        public async Task GoBackToProductsAsync()
        {
            await backToProductsButton.ClickAsync();
        }
    }
