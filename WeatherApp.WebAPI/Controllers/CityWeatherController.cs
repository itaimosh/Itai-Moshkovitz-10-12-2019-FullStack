using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityWeatherController : ControllerBase
    {
        private readonly WeatherAppContext _context;

        public CityWeatherController(WeatherAppContext context)
        {
            _context = context;
        }

        // GET: api/CityWeather
        [HttpGet]
        public IEnumerable<CityWeather> GetCityWeather()
        {
            return _context.CityWeather;
        }

       
        // GET: api/CityWeather/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityWeather([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cityWeather = await _context.CityWeather.FindAsync(id);

            if (cityWeather == null)
            {
                return NotFound();
            }

            return Ok(cityWeather);
        }

       

        // POST: api/CityWeather
        [HttpPost]
        public async Task<IActionResult> PostCityWeather([FromBody] CityWeather cityWeather)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = await _context.CityWeather.FindAsync(cityWeather.Key);

            if (city == null)
            {
                _context.CityWeather.Add(cityWeather);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction("GetCityWeather", new { id = cityWeather.Key }, cityWeather);
        }

      
    }
}