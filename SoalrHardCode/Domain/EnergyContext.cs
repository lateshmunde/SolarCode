namespace SolarEnergyPOC.Domain
{
    /// Carries energy state through the loss pipeline (PVcase-style)
    public class EnergyContext
    {
        // Irradiance
        public double Ghi;
        public double Dni;
        public double Dhi;

        // Geometry
        public double SunAltitudeDeg;

        // Plane of Array
        public double Poa; // W/m2

        // Thermal
        public double CellTemperatureC;

        // Power states
        public double DcPowerKW;
        public double AcPowerKW;

        // Final energy
        public double EnergyKWh;
    }
}
