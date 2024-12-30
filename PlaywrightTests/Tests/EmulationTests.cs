using Microsoft.Playwright;
namespace PlaywrightTests;

[TestFixture]
public class EmulationTests
{
    private IBrowser? browser;

[SetUp]
    public async Task SetUp()
    {
         var playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false // Set to true if you want headless mode
            });
    }
  
[Test]
public async Task EmulateDeviceConfigs()
        {
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/535.1 (KHTML, like Gecko) Mobile/15E148",
                ViewportSize = new ViewportSize() { Width = 375, Height = 667 },
                HasTouch = true,
                Permissions = new[] { "geolocation" },
                Geolocation = new Geolocation() { Longitude = 41.890221F, Latitude = 12.492348F },
                Locale = "en-US",
                TimezoneId = "America/Los_Angeles"
            });

            var page = await context.NewPageAsync();
            await page.GotoAsync("https://www.saucedemo.com");

            // Assertions to verify the emulation settings
            var userAgent = await page.EvaluateAsync<string>("navigator.userAgent");
            Assert.IsTrue(userAgent.Contains("iPhone"));
        }
}