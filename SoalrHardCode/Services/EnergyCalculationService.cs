using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;
using System;
using System.Net;

namespace SolarEnergyPOC.Services
{
    /// Ideal energy calculation service (NO LOSSES).
    public class EnergyCalculationService : IEnergyCalculationService
    {
        private const double Albedo = 0.20;

        public double CalculateHourlyEnergy(
            SolarPanel panel,
            SolarIrradiance irr,
            double sunAltDeg,
            double shadingLoss)
        {
            // Night-time guard
            if (sunAltDeg <= 0)
                return 0.0;

            double tiltRad = panel.TiltDeg * Math.PI / 180.0; // β - tilt angle 
            double cosTilt = Math.Cos(tiltRad);

            // Isotropic Plane-of-Array (POA) model
            //POA = DNI·cosβ + DHI·(1 + cosβ) / 2 + GHI·ρg·(1−cosβ)/ 2
            double poaWm2 =
                irr.Dni * cosTilt +
                irr.Dhi * (1 + cosTilt) / 2.0 +
                irr.Ghi * Albedo * (1 - cosTilt) / 2.0;

            

            if (poaWm2 <= 0)
                return 0.0;

            // Hourly energy (kWh)
            double poaKWhPerM2 = poaWm2 / 1000.0;

            // Ideal DC energy output
            return poaKWhPerM2 * panel.RatedPowerKW;
        }
    }
}
