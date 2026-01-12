using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Data
{
    /// <summary>
    /// Retrieves hourly solar irradiance data from NASA POWER API.
    /// 
    /// Characteristics:
    /// - Free
    /// - Commercially usable
    /// - No API key required
    /// - Deterministic (historical data)
    /// 
    /// This repository returns domain-level SolarIrradiance objects.
    /// </summary>
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

            var response = client.GetStringAsync(url).Result;

            var nasaResponse =
                JsonSerializer.Deserialize<NasaPowerResponse>(response);

            var ghiData =
                nasaResponse.Properties.Parameter["ALLSKY_SFC_SW_DWN"];
            var dniData =
                nasaResponse.Properties.Parameter["ALLSKY_SFC_SW_DNI"];
            var dhiData =
                nasaResponse.Properties.Parameter["ALLSKY_SFC_SW_DIFF"];
            var tempData =
                nasaResponse.Properties.Parameter["T2M"];

            var results = new List<SolarIrradiance>();

            foreach (var timestamp in ghiData.Keys)
            {
                // Timestamp format: YYYYMMDDHH
                int hour = int.Parse(timestamp.Substring(8, 2));

                results.Add(new SolarIrradiance(
                    hour: hour,
                    ghi: ghiData[timestamp],
                    dni: dniData[timestamp],
                    dhi: dhiData[timestamp],
                    ambientTempC: tempData[timestamp]
                ));
            }

            return results;
        }
    }
}
