using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherAppContext _context;
        private readonly string APIKEY = "Yia4PeS1jZ7iDxBbYOdyjGDVkg3CkKxe";
        const string AUTOCOMPLETEURL = "http://dataservice.accuweather.com/locations/v1/cities/autocomplete";
        const string CURRENTCONDITIONSURL = "http://dataservice.accuweather.com/currentconditions/v1/";
        //const string AccuWeatherUrl = "http://dataservice.accuweather.com";

        public WeatherController(WeatherAppContext context)
        {
            _context = context;
        }



        private HttpClient GetHttpClient(string url)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }


        [HttpGet("Search/{query}")]
        public async Task<IEnumerable<City>> AccuWeatherSearch([FromRoute] string query)
        {

            var client = GetHttpClient(AUTOCOMPLETEURL + "?apikey=" + APIKEY + "&q=" + query);
            HttpResponseMessage response = await client.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                var citiesList = await response.Content.ReadAsAsync<IEnumerable<City>>();
                return citiesList;
            }
            return null;
        }



        [HttpGet("GetCurrentWeather/{key}")]
        public async Task<IActionResult> GetCurrentWeather([FromRoute] string key)
        {

            var exitingWeather = await _context.CityWeather.FindAsync(key);

            if (exitingWeather != null)
            {
                return Ok(exitingWeather);
            }
            else
            {
                var client = GetHttpClient(CURRENTCONDITIONSURL + key + "?apikey=" + APIKEY);

                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic data = JArray.Parse(jsonString)[0];

                    var currentWeather = new Weather()
                    {
                        Key = key,
                        WeatherText = data.WeatherText,
                        Temperature = data.Temperature.Metric.Value,
                        Icon = data.WeatherIcon
                    };


                    //Save Weather for next time
                    _context.CityWeather.Add(currentWeather);
                    await _context.SaveChangesAsync();
                   

                    return Ok(currentWeather);
                }
            }
            return NotFound();
        }



        // GET: api/CityWeather
        [HttpGet]
        public IEnumerable<Weather> GetWeather()
        {
            return _context.CityWeather;
        }


        // GET: api/CityWeather/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeather([FromRoute] string id)
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
        public async Task<IActionResult> PostWeather([FromBody] Weather cityWeather)
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