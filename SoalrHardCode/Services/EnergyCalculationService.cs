using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class EnergyCalculationService : IEnergyCalculationService
    {
        private const double PanelRatedPowerKW = 0.54; // 540 Wp
        private const double SystemLosses = 0.95;
        private const double TimeStepHours = 1.0;

        public double CalculateFromPOA(double poa)
        {
            // POA (W/m²) → normalized irradiance fraction
            double normalizedIrradiance = poa / 1000.0;

            // kWh per panel per hour
            return
                normalizedIrradiance *
                PanelRatedPowerKW *
                SystemLosses *
                TimeStepHours;
        }
    }
}
