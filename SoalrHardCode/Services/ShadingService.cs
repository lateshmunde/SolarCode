using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    /// Ideal shading service: NO SHADING LOSSES.
    public class ShadingService : IShadingService
    {
        public double GetShadingLoss(double panelHeight, double sunAltitudeDeg)
        {
            // Ideal case: no shading
            return 0.0;
        }
    }
}
