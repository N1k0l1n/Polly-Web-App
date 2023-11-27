using Microsoft.AspNetCore.Mvc;
using Polly;
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

            #region Retry
            ////Implememt Retry-Policy
            //var retryPolicy = Policy.Handle<Exception>()
            //                        .RetryAsync(5, onRetry: (exception, retryCount) =>
            //                        {
            //                            Console.WriteLine($"Error: {exception.Message} .... Retry Count                 {retryCount}");
            //                        });

            //Excecute policy
            // await retryPolicy.ExecuteAsync(ConnectToApi); 
            #endregion



            #region Wait-Retry
            //Implement the Wait Policy
            //var amountToPause = TimeSpan.FromSeconds(15);

            //var retryWaitPolicy = Policy.Handle<Exception>()
            //                            .WaitAndRetryAsync(5, i => amountToPause, onRetry: (exception, retryCount) =>
            //                            {
            //                                Console.WriteLine($"Error: {exception.Message} .... Retry Count                 {retryCount}");
            //                            });

            //await retryWaitPolicy.ExecuteAsync(ConnectToApi);
            #endregion


            #region Circuit Breaker
            //Implement  Circuit Breaker
            var amountToPause = TimeSpan.FromSeconds(15);

            var retryPolicy = Policy.Handle<Exception>()
                                    .WaitAndRetry(5, i => amountToPause, (exception, retryCount) =>
                                    {
                                        Console.WriteLine($"Error: {exception.Message} .... Retry Count                 {retryCount}");
                                    });

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                                             .CircuitBreaker(3, TimeSpan.FromSeconds(30));

            //Retry and Circuit Breaker in one Policy
            var finalPolicy = retryPolicy.Wrap(circuitBreakerPolicy);
            finalPolicy.Execute(async () =>
            {
                Console.WriteLine("Excecuting");
                await ConnectToApi();
            });
            #endregion


            return Ok();
        }

        private async Task ConnectToApi()
        {
            var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";

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
                throw new Exception("Not able to connect to the service");
            }
        }
    }
}
