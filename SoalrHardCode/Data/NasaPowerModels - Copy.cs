using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SolarEnergyPOC.Data
{
    /// Root response model for NASA POWER API.
    /// Only required fields are modeled.
    public class NasaPowerResponse
    {
        [JsonPropertyName("properties")]
        public NasaPowerProperties Properties { get; set; }
    }

    public class NasaPowerProperties
    {
        [JsonPropertyName("parameter")]
        public Dictionary<string, Dictionary<string, double>> Parameter { get; set; }
    }
}
