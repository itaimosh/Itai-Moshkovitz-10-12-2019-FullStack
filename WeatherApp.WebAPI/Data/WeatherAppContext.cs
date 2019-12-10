using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Models
{
    public class WeatherAppContext : DbContext
    {
        public WeatherAppContext (DbContextOptions<WeatherAppContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherApp.Models.CityWeather> CityWeather { get; set; }

        public DbSet<WeatherApp.Models.Favorites> Favorites { get; set; }
    }
}
