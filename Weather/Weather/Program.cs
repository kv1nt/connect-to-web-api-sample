using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.Xml;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace Weather
{
    class Program
    {
        static void  Main(string[] args)
        {
          var weather =  WeatherService.GetWeather("Paris").GetAwaiter().GetResult();
            Console.WriteLine("Hello World!");
        }
    }

    class WeatherService
    {
        private const string URL = "http://api.openweathermap.org/data/2.5/weather";
        private const string AppID = "&APPID=2d4624a41219040ba7d2e1d22b6e9f65";
        
        public static async Task<Weather> GetWeather(string cityName)
        {
            string urlParams = $"?q={cityName}";
            var weather = new Weather();
            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(URL+ urlParams + AppID).Result;

                var responce = await client.GetAsync(urlParams + AppID);
               
                if (responce.IsSuccessStatusCode)
                {
                    var weatherJsonString = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<Weather>(weatherJsonString);
                    deserialized.MainInfo.Temperature = deserialized.MainInfo.Temperature - 273.15f;  // Convert Kelvin to Celsius
                    deserialized.Country.CountryAttribure = GetRegionFullName(deserialized.Country.CountryAttribure);
                    weather = deserialized;
                }
            }
            return weather;
        }

        public static string GetRegionFullName(string shortName)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID))
                    .FirstOrDefault(region => region.Name.ToUpper() == shortName && shortName != null).EnglishName;
        }

       [Serializable]
       public class Weather
       {
            [JsonProperty("id")] public long Id { get; set; }
            [JsonProperty("timezone")] public long TimeZone { get; set; }
            [JsonProperty("name")] public string City { get; set; }
            [JsonProperty("sys")] public Country Country { get; set; }
            [JsonProperty("main")] public Main MainInfo { get; set; }
            [JsonProperty("coord")] public Coordinates Coordinates { get; set; }
        }

        [Serializable]
        public class Main
        {
            [JsonProperty("temp")] public float Temperature { get; set; }
            [JsonProperty("pressure")] public double Pressure { get; set; }
        }

        [Serializable]
        public class Coordinates
        {
            [JsonProperty("lon")] public double Longtitude { get; set; }
            [JsonProperty("lat")] public double Latitude { get; set; }
        }

        [Serializable]
        public class Country
        {
            [JsonProperty("country")] public string CountryAttribure { get; set; }
        }
    }
}
