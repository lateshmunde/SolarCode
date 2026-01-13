using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    /// Contract for converting irradiance into energy output.
    /// Keeps physics logic isolated and testable.
    public interface IEnergyCalculationService
    {
        double CalculateHourlyEnergy(SolarPanel panel, SolarIrradiance irradiance, double sunAltitudeDeg, double shadingLoss);
    }
}
