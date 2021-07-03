using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtomicSharp.IdentityServer.Admin.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/identityserver/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] s_summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = s_summaries[rng.Next(s_summaries.Length)]
            })
                .ToArray();
        }
    }
}
