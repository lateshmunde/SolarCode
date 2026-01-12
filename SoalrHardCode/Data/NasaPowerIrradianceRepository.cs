using System;
using System.Collections.Generic;
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

            foreach (var kv in ghi)
            {
                // NASA timestamps are UTC → MUST be marked as UTC
                var utcTime = DateTime.SpecifyKind(
                    DateTime.ParseExact(kv.Key, "yyyyMMddHH", null),
                    DateTimeKind.Utc
                );

                results.Add(new SolarIrradiance(
                    utcTime,
                    ghi[kv.Key],
                    dni[kv.Key],
                    dhi[kv.Key],
                    temp[kv.Key]
                ));
            }

            return results;
        }
    }
}
