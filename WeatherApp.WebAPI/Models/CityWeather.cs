using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class CityWeather
    {
        [Key]
        public string Key { get; set; }

        public double Temperature { get; set; }

        public string WeatherText { get; set; }

        public int Icon { get; set; }
    }
}
