using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class EnergyCalculationService : IEnergyCalculationService
    {
        private const double DcToAcEfficiency = 0.97;
        private const double TempCoeff = -0.004;
        private const double NOCT = 45;

        public double CalculateHourlyEnergy(
            SolarPanel panel,
            SolarIrradiance irr,
            double sunAltDeg,
            double shadingLoss)
        {
            if (sunAltDeg <= 0) return 0;

            double poa = irr.Ghi * 1.08 * (1 - shadingLoss);

            double cellTemp =
                irr.AmbientTempC + (poa / 800) * (NOCT - 20);

            double tempFactor =
                1 + TempCoeff * (cellTemp - 25);

            double dcPower =
                panel.RatedPowerKW * (poa / 1000) * tempFactor;

            double acEnergy =
                dcPower * DcToAcEfficiency;

            return acEnergy > 0 ? acEnergy : 0;
        }
    }
}
