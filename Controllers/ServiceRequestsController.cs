using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Polly_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ServiceRequestsController : ControllerBase
    {
        private readonly ILogger<ServiceRequestsController> _logger;
        public ServiceRequestsController(ILogger<ServiceRequestsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetData")]
        public async Task<IActionResult> Get()
        {
            ConnectToApi();
            return Ok();
        }

        private async void ConnectToApi()
        {
            var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/search?query?%3CREQUIRED%3E";

            var client = new RestClient();

            var request = new RestRequest(url, Method.Get);

            request.AddHeader("accept", "application/json");
            request.AddHeader("X-RapidAPI-Key", "f96c5910eamsh7308e044612b000p11a98bjsne7bf35010cd6");
            request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
            }
        }
    }
}
