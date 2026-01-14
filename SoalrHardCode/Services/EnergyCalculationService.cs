using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class EnergyCalculationService : IEnergyCalculationService
    {
        private const double DcToAcEfficiency = 0.97;
        private const double TempCoeff = -0.004; // temp loss
        private const double NOCT = 45; //NOCT model (industry standard) // temp loss

        private const double Albedo = 0.20; //for ideal case
        public double CalculateHourlyEnergy(SolarPanel panel, SolarIrradiance irr, double sunAltDeg, double shadingLoss)
        {
            if (sunAltDeg <= 0) return 0;

            double poa = irr.Ghi * 1.08 * (1 - shadingLoss);

            double cellTemp = irr.AmbientTempC + (poa / 800) * (NOCT - 20);

            double tempFactor = 1 + TempCoeff * (cellTemp - 25);

            double dcPower = panel.RatedPowerKW * (poa / 1000) * tempFactor;
            //RatedPower : indicates its maximum DC power output under ideal lab conditions

            double acEnergy = dcPower * DcToAcEfficiency;

            return acEnergy > 0 ? acEnergy : 0;
        }

        public double CalculateHourlyEnergyIdeal(SolarPanel panel, SolarIrradiance irr, double sunAltDeg, double shadingLoss)
        {
            // Night-time guard
            if (sunAltDeg <= 0)
                return 0.0;

            double tiltRad = panel.TiltDeg * Math.PI / 180.0; // β - tilt angle 
            double cosTilt = Math.Cos(tiltRad);

            // Isotropic Plane-of-Array (POA) model
            //POA = DNI·cosβ + DHI·(1 + cosβ) / 2 + GHI·ρg·(1−cosβ)/ 2
            double poa =
                irr.Dni * cosTilt +
                irr.Dhi * (1 + cosTilt) / 2.0 +
                irr.Ghi * Albedo * (1 - cosTilt) / 2.0;

            if (poa <= 0)
                return 0.0;

            // Hourly energy (kWh)
            double poaKWhPerM2 = poa / 1000.0;

            // Ideal DC energy output
            return poaKWhPerM2 * panel.RatedPowerKW;
        }
    }
}