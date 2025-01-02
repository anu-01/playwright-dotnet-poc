using Microsoft.Playwright;
using Newtonsoft.Json;
using PlaywrightTests.Reporting;
using Helpers;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace PlaywrightTests;

[TestFixture]
public class APITests : ReportGenerator
{
    private static IAPIRequestContext Request = null;
    

   [SetUp]
    public async Task SetupAPI() 
    {
       var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        // Add authorization token to all requests.
        // Assuming personal access token available in the environment.

        // Call Authentication method and fetch token
        await GetAuthTokenAsync();
        string? API_TOKEN = Environment.GetEnvironmentVariable("API_TOKEN");
        headers.Add("Cookie", "token=" + API_TOKEN);

        Request = await this.Playwright.APIRequest.NewContextAsync(new() {
            // All requests we send go to this API endpoint.
            BaseURL = "https://restful-booker.herokuapp.com",
            ExtraHTTPHeaders = headers,
        });
    }

    public async Task GetAuthTokenAsync()
    {   

        var request= await this.Playwright.APIRequest.NewContextAsync();
        var authentication = await request.PostAsync("https://restful-booker.herokuapp.com/auth", new APIRequestContextOptions
            {
                DataObject = new
                {
                    username = "admin",
                    password = "password123"
                }
            });
 
            var authResponse = await authentication.JsonAsync();
            var token = authResponse?.GetProperty("token").GetString();

            Environment.SetEnvironmentVariable("API_TOKEN", token);

    }

    [Test]
        public async Task ShouldGetAllBookingDetails()
        {
            // Send GET request to /booking
            var response = await Request.GetAsync("/booking");

            // Assert response status and print response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = await response.JsonAsync();
            Console.WriteLine(responseBody); // Log response for debugging
        }

        [Test]
        public async Task ShouldGetSpecificBookingDetails()
        {
            // Send GET request to /booking
            var response = await Request.GetAsync("/booking/1");

            // Assert response status and print response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = await response.JsonAsync();
            Console.WriteLine(responseBody); // Log response for debugging
        }

       [Test]
        public async Task ShouldGetSubsetOfBookingDetailsUsingQueryParameters()
        {
            // Send GET request with query parameters
            var response = await Request.GetAsync("/booking", new APIRequestContextOptions
            {
                Params = new Dictionary<string, object>
                {
                    { "firstname", "Susan" },
                    { "lastname", "Jackson" }
                }
            });
            // Assert response status and print response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = await response.JsonAsync();
            Console.WriteLine(responseBody); // Log response for debugging
        }

        [Test]
        public async Task ShouldCreateABooking()
        {
            // Booking data payload
            var bookingData = new
            {
                firstname = "Jim",
                lastname = "Brown",
                totalprice = 111,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2023-06-01",
                    checkout = "2023-06-15"
                },
                additionalneeds = "Breakfast"
            };

            // Send POST request
            var response = await Request.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = bookingData
            });

            // Assert response status and validate response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");
 
            var responseText = await response.TextAsync();
          
            var responseBody = System.Text.Json.JsonSerializer.Deserialize<JsonNode>(responseText);
            Console.WriteLine(responseBody?["booking"]); // Log response for debugging
            var booking = responseBody?["booking"];

           Assert.AreEqual("Jim", booking?["firstname"]?.ToString(), "Firstname does not match");
            Assert.AreEqual("Brown", booking?["lastname"]?.ToString(), "Lastname does not match");
            Assert.AreEqual("111", booking?["totalprice"]?.ToString(), "Totalprice does not match");
            Assert.IsTrue((bool)booking?["depositpaid"], "Depositpaid does not match");
            Assert.AreEqual("2023-06-01", booking?["bookingdates"]?["checkin"]?.ToString(), "Checkin date does not match");
            Assert.AreEqual("2023-06-15", booking?["bookingdates"]?["checkout"]?.ToString(), "Checkout date does not match");
            Assert.AreEqual("Breakfast", booking?["additionalneeds"]?.ToString(), "Additionalneeds does not match");
        }


        [Test]
        public async Task ShouldCreateABookingFromJSON()
        {

            var jsonString  = File.ReadAllTextAsync("../net8.0/PlaywrightTests/TestData/bookingDetails.json");
            var bookingDetails = JsonConvert.DeserializeObject<Booking>(jsonString.Result);
            // Send POST request
            var response = await Request.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = bookingDetails
            });

            // Assert response status and validate response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = System.Text.Json.JsonSerializer.Deserialize<BookingResponse>(
                await response.TextAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            Console.WriteLine(response.TextAsync().Result); // Log response for debugging

            Assert.AreNotEqual(0, responseBody.BookingId);
            Assert.AreEqual("John", responseBody?.Booking.Firstname, "Firstname does not match");
            Assert.AreEqual("Doe", responseBody.Booking.Lastname, "Lastname does not match");
            Assert.AreEqual(1000, responseBody.Booking.Totalprice, "Totalprice does not match");
            Assert.IsTrue(responseBody.Booking.Depositpaid, "Depositpaid does not match");
            Assert.AreEqual("2025-02-01", responseBody.Booking.Bookingdates.Checkin, "Checkin date does not match");
            Assert.AreEqual("2025-02-15", responseBody.Booking.Bookingdates.Checkout, "Checkout date does not match");
            Assert.AreEqual("Lunch", responseBody.Booking.Additionalneeds, "Additionalneeds does not match");
        }

    
        [Test]
        public async Task ShouldUpdateBookingDetails()
        {

            var jsonString  = File.ReadAllText("../net8.0/PlaywrightTests/TestData/bookingDetails.json");
            var bookingDetails = JsonConvert.DeserializeObject<Booking>(jsonString);
            bookingDetails.Firstname = "Jerry";
            // Send POST request
            var response = await Request.PutAsync($"/booking/{CreateBooking().Result}", new APIRequestContextOptions
            {
                DataObject = bookingDetails
            });

            // Assert response status and validate response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = response.JsonAsync<JsonNode>().Result;
            
            Assert.AreEqual("Jerry", responseBody["firstname"].ToString(), "Firstname does not match");
            
            }

        [Test]
        public async Task ShouldDeleteBooking()
        {            
            
            // Send POST request
            var response = await Request.DeleteAsync($"/booking/{CreateBooking().Result}");

            // Assert response status and validate response body
            Assert.AreEqual(201, response.Status, "Response status is not 201");
   
        }

        public async Task<int> CreateBooking()
        {
            var jsonString  = File.ReadAllText("../net8.0/PlaywrightTests/TestData/bookingDetails.json");
            var bookingDetails = JsonConvert.DeserializeObject<Booking>(jsonString);
            // Send POST request
            var response = await Request.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = bookingDetails
            });

            // Assert response status and validate response body
            Assert.IsTrue(response.Ok, "Response is not OK");
            Assert.AreEqual(200, response.Status, "Response status is not 200");

            var responseBody = System.Text.Json.JsonSerializer.Deserialize<BookingResponse>(
                await response.TextAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            Console.WriteLine(response.TextAsync().Result); // Log response for debugging

            return responseBody.BookingId;
        }

        

     [TearDown]
    public async Task TearDownAPI()
    {
        await Request.DisposeAsync();
    }
}