namespace SolarEnergyPOC.Domain
{
    /// Represents a physical geographic location on Earth.
    public class Location
    {
        /// Latitude in decimal degrees (positive for North).
        /// Example: Gujarat ≈ 23.0
        public double Latitude { get; }

        /// Longitude in decimal degrees (positive for East).
        /// Example: Gujarat ≈ 72.0
        public double Longitude { get; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
