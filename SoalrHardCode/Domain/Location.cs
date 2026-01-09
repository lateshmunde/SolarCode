namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Represents a physical geographic location on Earth.
    /// 
    /// This class is intentionally simple and immutable.
    /// In future iterations, this will be used by:
    /// - Sun position calculations
    /// - Weather / irradiation API queries
    /// - GIS integrations
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Latitude in decimal degrees (positive for North).
        /// Example: Gujarat ≈ 23.0
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Longitude in decimal degrees (positive for East).
        /// Example: Gujarat ≈ 72.0
        /// </summary>
        public double Longitude { get; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
