namespace SolarEnergyPOC.Domain
{
    public class PanelGeometry
    {
        // Degrees
        public double Tilt { get; set; }
        public double Azimuth { get; set; }

        // Ground reflectance (Rajasthan default)
        public double Albedo { get; set; } = 0.30;
    }
}
