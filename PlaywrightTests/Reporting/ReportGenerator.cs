// <copyright file="ReportGenerator.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace PlaywrightTests.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using AventStack.ExtentReports;
    using AventStack.ExtentReports.Config;
    using AventStack.ExtentReports.Reporter;
    using AventStack.ExtentReports.Reporter.Config;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using Microsoft.Playwright.NUnit;
    using System.Threading.Tasks;

    /// <summary>
    /// http://extentreports.com/docs/versions/4/net/
    /// </summary>
    [SetUpFixture]
    public class ReportGenerator : PageTest
    {        
        private static readonly NameValueCollection ReportSettings = ConfigurationManager.GetSection("reporting") as NameValueCollection ?? new NameValueCollection();
        private static readonly bool Enabled = ReportSettings != null && bool.TryParse(ReportSettings["Enabled"], out bool enabledValue) && enabledValue;
        private static readonly bool ShowSteps = Enabled && ReportSettings != null && bool.TryParse(ReportSettings["ShowSteps"], out bool showStepsValue) && showStepsValue;
        private static readonly bool DarkTheme = Enabled && ReportSettings != null && bool.TryParse(ReportSettings["DarkTheme"], out bool darkThemeValue) && darkThemeValue;
   
        private static ExtentReports? extent;  
        public static ExtentTest? extentTest;
        
    
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            string currentdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string reportPath = Path.Combine(currentdir, "TestReports", "TestResults " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")+ ".html");
            Console.WriteLine("Report Path " + reportPath);
            string style = "body {font-family: 'Segoe UI';}";
            ExtentSparkReporter htmlReporter = new ExtentSparkReporter(reportPath);
            htmlReporter.Config.Theme = DarkTheme ? Theme.Dark : Theme.Standard;
            htmlReporter.Config.CSS = style;
            extent = new ExtentReports();
            extent.AddSystemInfo("Host", Environment.MachineName);
            extent.AddSystemInfo("Env", ConfigurationManager.AppSettings["Env"]);
            extent.AddSystemInfo("User", Environment.UserName);
            extent.AttachReporter(htmlReporter);
        }

        [OneTimeTearDown]
        public static void ClassTearDown()
        {
        extent?.Flush();
        }

        [SetUp]
        public async Task BeforeTestAsync()
        {
            extentTest = extent?.CreateTest(NUnit.Framework.TestContext.CurrentContext.Test.ClassName);
            var categories = NUnit.Framework.TestContext.CurrentContext.Test.Properties["Category"];
            var author = TestContext.CurrentContext.Test.Properties.Get("Author")?.ToString() ?? "Unknown";
            if (extentTest != null)
            {
                AddMetadata(extentTest, categories, author);
            }

            // Start tracing
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task AfterTestAsync()
        {
            Status logstatus;
            TestStatus status = TestContext.CurrentContext.Result.Outcome.Status;
            Console.WriteLine(TestContext.CurrentContext.Test.Name + " " + status);
            string stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.Message);

            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    // Stop Tracing
                    await Context.Tracing.StopAsync(new()
                    {
                        Path = Path.Combine(
                            TestContext.CurrentContext.WorkDirectory,
                            "playwright-traces",
                            $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                        ),
                    });
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    logstatus = Status.Pass;
                    break;
            }

            var node = extentTest?.CreateNode(TestContext.CurrentContext.Test.MethodName);
            node?.Log(logstatus, "Test Execution status - " + logstatus + stacktrace);
        }

        private static void AddMetadata(ExtentTest node, IEnumerable<object> categories, string author)
        {
            try
            {
                if (categories.Count() > 0 && ! string.IsNullOrEmpty(author))
                {
                    node.AssignCategory(categories.Cast<string>().ToArray());
                    node.AssignAuthor(author);
                }                  
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
            }
        }
    }
}