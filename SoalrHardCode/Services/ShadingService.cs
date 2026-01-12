using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class ShadingService : IShadingService
    {
        public double CalculateShadingFactor(
            SunPosition sun,
            PanelGeometry panel)
        {
            if (!sun.IsSunUp)
                return 0;

            // Hardcoded winter-morning shading loss
            if (sun.Zenith > 70)
                return 0.7;

            return 1.0;
        }
    }
}
