using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DemoOpenTelemetry.Clients;

namespace DemoOpenTelemetry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherHttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token = default)
        {
            _logger.LogInformation("Get the weather");
            using var response = await _httpClient.Client.GetAsync("/WeatherForecast", HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var weatherInfo = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(stream, _jsonSerializerOptions, token);
            return Ok(weatherInfo);
        }
    }
}
