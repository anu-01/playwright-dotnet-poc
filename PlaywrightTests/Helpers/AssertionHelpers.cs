using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Helpers;
public static class AssertionHelper
{
    public static async Task AssertTextContainsAsync(ILocator locator, string expectedText)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToContainTextAsync(expectedText);
    }

    public static async Task AssertElementCountAsync(ILocator locator, int expectedCount)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToHaveCountAsync(expectedCount);
    }

    public static async Task AssertUrlContainsAsync(IPage page, string expectedSubstring)
    {
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        var currentUrl = await Task.FromResult(page.Url);
        if (!currentUrl.Contains(expectedSubstring))
        {
            throw new PlaywrightException($"URL does not contain the expected text: '{expectedSubstring}'. Current URL: '{currentUrl}'.");
        }
    }

    public static async Task AssertVisibleAsync(ILocator locator, int timeout = 10_000)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout= timeout});
    }

    public static async Task AssertHiddenAsync(ILocator locator)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToBeHiddenAsync();
    }

    public static async Task AssertEnabledAsync(ILocator locator, int timeout = 10_000)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToBeEnabledAsync(new() { Timeout= timeout});
    }

    public static async Task AssertDisabledAsync(ILocator locator, int timeout = 10_000)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToBeDisabledAsync(new() { Timeout= timeout});
    }

    public static async Task AssertCheckedAsync(ILocator locator)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToBeCheckedAsync();
    }

    public static async Task AssertNotCheckedAsync(ILocator locator)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).Not.ToBeCheckedAsync();
    }

    public static async Task AssertValueAsync(ILocator locator, string expectedValue)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        await Assertions.Expect(locator).ToHaveValueAsync(expectedValue);
    }

    public static async Task AssertTitleAsync(IPage page, string expectedTitle)
    {
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        var currentTitle = await page.TitleAsync();
        if (!currentTitle.Contains(expectedTitle))
        {
            throw new PlaywrightException($"Title does not contain the expected text: '{expectedTitle}'. Current title: '{currentTitle}'.");
        }
    }

    public static async Task AssertAttributeAsync(ILocator locator, string attributeName, string expectedValue)
    {
        if (locator == null)
        {
            throw new ArgumentNullException(nameof(locator));
        }

        var actualValue = await locator.GetAttributeAsync(attributeName);
        if (actualValue != expectedValue)
        {
            throw new PlaywrightException($"Attribute '{attributeName}' does not have the expected value: '{expectedValue}'. Actual value: '{actualValue}'.");
        }
    }
}
