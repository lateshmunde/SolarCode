using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Data
{
    public class NasaPowerIrradianceRepository : IIrradianceRepository
    {
        private readonly Location _location;
        private readonly int _year;
        private readonly TimeZoneInfo _ist =
            TimeZoneInfo.CreateCustomTimeZone("IST", TimeSpan.FromHours(5.5), "IST", "IST");

        public NasaPowerIrradianceRepository(Location location, int year)
        {
            _location = location;
            _year = year;
        }

        public IEnumerable<SolarIrradiance> GetHourlyData()
        {
            using var client = new HttpClient();

            string url =
                "https://power.larc.nasa.gov/api/temporal/hourly/point" +
                $"?parameters=ALLSKY_SFC_SW_DWN,ALLSKY_SFC_SW_DNI,ALLSKY_SFC_SW_DIFF,T2M" +
                $"&community=RE" +
                $"&latitude={_location.Latitude}" +
                $"&longitude={_location.Longitude}" +
                $"&start={_year}" +
                $"&end={_year}" +
                $"&format=JSON";

            var json = client.GetStringAsync(url).Result;
            var response = JsonSerializer.Deserialize<NasaPowerResponse>(json);

            var ghi = response.Properties.Parameter["ALLSKY_SFC_SW_DWN"];
            var dni = response.Properties.Parameter["ALLSKY_SFC_SW_DNI"];
            var dhi = response.Properties.Parameter["ALLSKY_SFC_SW_DIFF"];
            var temp = response.Properties.Parameter["T2M"];

            var results = new List<SolarIrradiance>();
            var txtPath = $"nasa_power_{_location.Latitude}_{_location.Longitude}_{_year}.txt";

            using var writer = new StreamWriter(txtPath);
            writer.WriteLine("DateTimeLocal | GHI | DNI | DHI | Temp");

            foreach (var key in ghi.Keys)
            {
                DateTime utc =
                    DateTime.ParseExact(key, "yyyyMMddHH", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, _ist);

                writer.WriteLine(
                    $"{local:yyyy-MM-dd HH:mm} | {ghi[key]:F2} | {dni[key]:F2} | {dhi[key]:F2} | {temp[key]:F2}");

                results.Add(new SolarIrradiance(
                    utc,
                    local,
                    ghi[key],
                    dni[key],
                    dhi[key],
                    temp[key]));
            }

            return results;
        }
    }
}
