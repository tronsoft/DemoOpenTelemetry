using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DemoOpenTelemetry.Clients;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using static DemoOpenTelemetry.Constants;

namespace DemoOpenTelemetry
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => o.Filters.Add(new ProducesAttribute("application/json")));
            services.AddHttpClient<WeatherClient>();
            services.AddSwaggerGen();

            // opentelemetry
            services.AddOpenTelemetryTracing(b =>
            {
                b.AddAspNetCoreInstrumentation()
                    .AddSource(ServiceName)
                    .AddZipkinExporter(o => o.Endpoint = new Uri("http://localhost:9411/api/v2/spans"))
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName, ServiceVersion))
                    .AddHttpClientInstrumentation();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
