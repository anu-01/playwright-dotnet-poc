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

    /// <summary>
    /// http://extentreports.com/docs/versions/4/net/
    /// </summary>
    public class ReportGenerator : PageTest
    {        
        private static readonly NameValueCollection ReportSettings = ConfigurationManager.GetSection("reporting") as NameValueCollection;
        private static readonly bool Enabled = ReportSettings != null && bool.Parse(ReportSettings["Enabled"]);
        private static readonly bool ShowSteps = Enabled && bool.Parse(ReportSettings["ShowSteps"]);
        private static readonly bool DarkTheme = Enabled && bool.Parse(ReportSettings["DarkTheme"]);
        private static ExtentReports extent;  
        private static ExtentTest extentTest;
    
        [SetUp]
        public void BeforeTest()
        {
            string currentdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string reportPath = Path.Combine(currentdir, "TestResults " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")+ ".html");
            Console.WriteLine("Report Path" + reportPath);
            string style = "body {font-family: 'Segoe UI';}";
            ExtentSparkReporter htmlReporter = new ExtentSparkReporter(reportPath);
            htmlReporter.Config.Theme = DarkTheme ? Theme.Dark : Theme.Standard;
            htmlReporter.Config.CSS = style;
            extent = new ExtentReports();
            extent.AddSystemInfo("Host", Environment.MachineName);
            extent.AddSystemInfo("Env", ConfigurationManager.AppSettings["Env"]);
            extent.AddSystemInfo("User", Environment.UserName);
            extent.AttachReporter(htmlReporter);
            extentTest = extent.CreateTest(TestContext.CurrentContext.Test.Name);
            var categories = TestContext.CurrentContext.Test.Properties["Category"];
            var author = TestContext.CurrentContext.Test.Properties.Get("Author")?.ToString() ?? "Unknown";
            AddMetadata(extentTest, categories, author);
            Console.WriteLine("Report Location "+ reportPath);
        }

        [TearDown]
        public void AfterTest()
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

            var node = extentTest.CreateNode(TestContext.CurrentContext.Test.MethodName);
            node.Log(logstatus, "Test Execution status - " + logstatus + stacktrace);
            extent.Flush();
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