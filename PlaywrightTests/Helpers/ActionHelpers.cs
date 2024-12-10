using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Helpers;
public static class ActionHelpers
{
    public static async Task EnterTextAsync(ILocator selector, string text)
    {
        await selector.ClearAsync();
        await selector.FillAsync(text);
    }

    public static async Task GoToUrlAsync(IPage page, string url)
    {
        await page.GotoAsync(url);
    }

    public static async Task ClickButtonAsync(ILocator selector)
    {
        await selector.ClickAsync();
    }

    public static async Task SelectCheckboxAsync(ILocator selector)
    {
        await selector.CheckAsync();
    }

    public static async Task SelectOptionsAsync(ILocator selector, string value)
    {
        await selector.SelectOptionAsync(value);
    }

    public static async Task DoubleClickAsync(ILocator selector)
    {
        await selector.DblClickAsync();
    }

    public static async Task RightClickAsync(ILocator selector)
    {
        await selector.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
    }

    public static async Task ShiftClickAsync(ILocator selector)
    {
        await selector.ClickAsync(new LocatorClickOptions { Modifiers = new[] { KeyboardModifier.Shift } });
    }

    public static async Task MouseHoverAsync(ILocator selector)
    {
        await selector.HoverAsync();
    }

    public static async Task WaitForElementAsync(ILocator selector, int timeout)
    {
        await selector.WaitForAsync(new LocatorWaitForOptions { Timeout = timeout });
    }

    public static async Task ScrollToElementAsync(ILocator selector)
    {
        await selector.ScrollIntoViewIfNeededAsync();
    }

    public static async Task DragAndDropAsync(ILocator source, ILocator target)
    {
        await source.DragToAsync(target);
    }

    public static Task DialogHandlerAsync(IPage page, bool accept)
    {
        page.Dialog += async (_, e) =>
        {
            if (accept)
            {
                await e.AcceptAsync();
            }
            else
            {
                await e.DismissAsync();
            }
        };
        return Task.CompletedTask;
    }

    public static async Task SelectRadioButtonAsync(ILocator selector)
    {
        await selector.CheckAsync();
    }
}
