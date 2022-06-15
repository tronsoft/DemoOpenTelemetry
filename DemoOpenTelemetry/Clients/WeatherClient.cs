using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DemoOpenTelemetry.Clients
{
    public class WeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherClient> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public WeatherClient(HttpClient httpClient, ILogger<WeatherClient> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:6000/api/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<WeatherForecast>>GetWeatherForecasts(CancellationToken token = default)
        {
            _logger.LogInformation($"{nameof(WeatherClient)}.{nameof(GetWeatherForecasts)} entered.");

            using var response = await _httpClient.GetAsync("/WeatherForecast", HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(stream, _jsonSerializerOptions, token);
        }
    }
}
