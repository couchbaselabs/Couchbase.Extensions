using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Couchbase.Extensions.Caching.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        [HttpGet("WeatherForecast/AbsoluteExpiration", Name = "GetWeatherForecastAbsoluteExpiration")]
        public async Task<IEnumerable<WeatherForecast>> GetAbsoluteExpiration([FromServices] ICouchbaseCache couchbaseCache)
        {
            var weatherForcast = await couchbaseCache.GetAsync<IEnumerable<WeatherForecast>>("weatherForecastAbsolute");

            if(weatherForcast == null)
            {
                logger.LogInformation("Cache miss!");
                weatherForcast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                await couchbaseCache.SetAsync("weatherForecastAbsolute", weatherForcast,
                    new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10)));

                return weatherForcast;
            }
            logger.LogInformation("Cache hit!");
            return weatherForcast;
        }

        [HttpGet("WeatherForecast/SlidingExpiration", Name = "GetWeatherForecastSlidingExpiration")]
        public async Task<IEnumerable<WeatherForecast>> GetSlidingExpiration([FromServices] ICouchbaseCache couchbaseCache)
        {
            var weatherForcast = await couchbaseCache.GetAsync<IEnumerable<WeatherForecast>>("weatherForecastSliding");

            if(weatherForcast == null)
            {
                logger.LogInformation("Cache miss!");
                weatherForcast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                await couchbaseCache.SetAsync("weatherForecastSliding", weatherForcast,
                    new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10)));

                return weatherForcast;
            }
            logger.LogInformation("Cache hit!");
            return weatherForcast;
        }

        [HttpGet("WeatherForecast/Hybrid", Name = "GetWeatherForecastHybrid")]
        public async Task<IEnumerable<WeatherForecast>> GetHybrid([FromServices] HybridCache hybridCache)
        {
            var weatherForcast = await hybridCache.GetOrCreateAsync<IEnumerable<WeatherForecast>>(
                "weatherForecastAbsolute",
                async (cancellationToken) =>
                {
                    logger.LogInformation("Cache miss!");
                    var weatherForcast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }).ToArray();

                    return weatherForcast;
                },
                new HybridCacheEntryOptions()
                {
                    LocalCacheExpiration = TimeSpan.FromSeconds(3),
                    Expiration = TimeSpan.FromSeconds(10),
                });

            return weatherForcast;
        }
    }
}