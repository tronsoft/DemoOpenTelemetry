using System;
using System.Net.Http;

namespace DemoOpenTelemetry.Clients
{
    public class WeatherHttpClient
    {
        public WeatherHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
            Client.BaseAddress = new Uri("http://localhost:6000/api/");
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
        }

        public HttpClient Client { get; }
    }
}
