using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace GeoHashing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration["ConnectionStrings:GeoHashDB"];
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Foo")]
        public async Task<IActionResult> Foo(double precision, double lngSouthWest, double latSouthWest, double lngNorthEast, double latNorthEast)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sqlStatement =
                $"select count(aname1), ST_GeoHash(geom, {precision}), " +
                $"ST_X(ST_PointFromGeoHash(ST_GeoHash(geom, {precision}))), ST_Y(ST_PointFromGeoHash(ST_GeoHash(geom, {precision})))" +
                "from ( " +
                    "select aname1, geom from allcountries " +
                    //"where ST_Contains(ST_MakeEnvelope(-180,0,0,90, 4326), geom) " +
                    $"where ST_Contains(ST_MakeEnvelope({lngSouthWest}, {latSouthWest}, {lngNorthEast}, {latNorthEast}, 4326), geom) " +
                    //$"where ST_Contains(ST_MakeEnvelope({bottomRightLongitude}, {bottomRightLatitude}, {topLeftLongitude}, {topLeftLatitude}, 4326), geom) " +
                ") as bbox " +
                $"group by ST_GeoHash(geom, {precision});";

            await using var cmd = new NpgsqlCommand(sqlStatement, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            var data1List = await GetData1(reader).ConfigureAwait(false);

            return Ok(data1List);
        }

        private async Task<List<Data1>> GetData1(NpgsqlDataReader reader)
        {
            var dataList = new List<Data1>();
            while (await reader.ReadAsync())
            {
                dataList.Add(
                    new Data1 { 
                        Count = reader.GetInt64(0), 
                        GeoHash = reader.GetString(1),
                        Longitude = reader.GetDouble(2),
                        Latitude = reader.GetDouble(3)
                    });
            }
            return dataList;
        }
    }

    public class Data1
    {
        public long Count { get; set; }
        public string GeoHash { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
