using Microsoft.Playwright;

namespace PlaywrightTests.Pages.SharedComponents;
public class MenuPage
{
    private readonly IPage Page;
    private readonly ILocator menuButton;
    private readonly ILocator logoutButton;

    public MenuPage(IPage page)
    {
        this.Page = page;
        this.menuButton = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        this.logoutButton = Page.Locator("[data-test=\"logout-sidebar-link\"]");
    }

    public async Task OpenMenuAsync()
    {
        await menuButton.ClickAsync();
    }

    public async Task LogoutAsync()
    {
        await OpenMenuAsync();
        await logoutButton.ClickAsync();
    }
}