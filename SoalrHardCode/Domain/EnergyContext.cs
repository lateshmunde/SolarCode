using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Domain
{
    public class EnergyContext
    {
        // Immutable input
        public SolarIrradiance Irradiance { get; init; }

        // Geometry
        public double SunAltitudeDeg { get; set; }

        // Plane of Array
        public double Poa { get; set; }              // W/m²

        // Thermal
        public double CellTemperatureC { get; set; }

        // Electrical
        public double DcPowerKW { get; set; }
        public double AcPowerKW { get; set; }

        // Final
        public double EnergyKWh { get; set; }
    }
}
