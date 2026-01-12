namespace SolarEnergyPOC.Domain
{
    public class SunPosition
    {
        // Degrees
        public double Zenith { get; set; }
        public double Azimuth { get; set; }

        public bool IsSunUp => Zenith < 90;
    }
}
