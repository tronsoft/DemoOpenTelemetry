using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DemoOpenTelemetry.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static DemoOpenTelemetry.Constants;

namespace DemoOpenTelemetry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ActivitySource _activitySource = new ActivitySource(ServiceName);
        private readonly WeatherClient _client;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token = default)
        {
            _logger.LogInformation("Get the weather");

            // Start the opentelemetry activity
            using var activity = _activitySource.StartActivity($"{nameof(WeatherForecastController)}.{nameof(Get)}");

            var weatherInfo = await _client.GetWeatherForecasts(token);
            return Ok(weatherInfo);
        }
    }
}