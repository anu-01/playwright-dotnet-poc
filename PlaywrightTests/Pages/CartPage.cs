using Microsoft.Playwright;
using Helpers;

namespace PlaywrightTests.Pages;
    public class CartPage
    {
        private readonly IPage Page;
        private readonly ILocator cart;
        private readonly ILocator shoppingCartLink;
        private readonly ILocator shoppingCartBadge;


        public CartPage(IPage page)
        {
            this.Page = page;
            this.cart = Page.Locator("[data-test=\"cart\"]");
            this.shoppingCartLink = Page.Locator("[data-test=\"shopping-cart-link\"]");
            this.shoppingCartBadge = Page.Locator("[data-test=\"shopping-cart-badge\"]");

        }

        public async Task AddToCartAsync(string itemName)
        {
            await Page.Locator($"[data-test=\"add-to-cart-{itemName}\"]").ClickAsync();
        }

        public async Task GoToCartAsync()
        {
            await shoppingCartLink.ClickAsync();
        }

        public async Task VerifyCart()
        {
            // await Assertions.Expect(shoppingCartBadge).ToContainTextAsync("1");
            await Helpers.AssertionHelper.AssertTextContainsAsync(shoppingCartBadge, "1");
            // await Assertions.Expect(cart).ToBeVisibleAsync();
            await Helpers.AssertionHelper.AssertVisibleAsync(cart);
        
        }
    }
