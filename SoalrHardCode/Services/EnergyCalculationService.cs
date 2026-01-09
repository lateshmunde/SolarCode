using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Services
{
    /// <summary>
    /// Converts irradiance and environmental conditions
    /// into electrical energy output.
    /// 
    /// This service encapsulates all energy physics logic
    /// and keeps the rest of the system clean.
    /// </summary>
    public class EnergyCalculationService
    {
        private const double TemperatureCoefficient = -0.004;
        private const double NOCT = 45;

        /// <summary>
        /// Calculates energy produced by one panel in one hour (kWh).
        /// </summary>
        public double CalculateHourlyEnergy(SolarPanel panel, SolarIrradiance irradiance, double sunAltitudeDeg, double shadingLoss)
        {
            // Approximate plane-of-array irradiance with tilt gain
            double poa = irradiance.Ghi * 1.08;

            // Apply shading loss
            poa *= (1 - shadingLoss);

            // Estimate cell temperature
            double cellTemp = irradiance.AmbientTempC + (poa / 800.0) * (NOCT - 20);

            // Temperature derating
            double tempLossFactor = 1 + TemperatureCoefficient * (cellTemp - 25);

            // DC power output (kW)
            double powerKW = panel.RatedPowerKW * (poa / 1000.0) * tempLossFactor;

            return powerKW > 0 ? powerKW : 0;
        }
    }
}
