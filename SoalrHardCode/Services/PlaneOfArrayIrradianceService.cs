using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class PlaneOfArrayIrradianceService : IPlaneOfArrayIrradianceService
    {
        private const double Deg2Rad = Math.PI / 180.0;

        public double CalculatePOA(
            IrradianceInput irr,
            SunPosition sun,
            PanelGeometry panel,
            double shadingFactor)
        {
            double theta =
                CalculateAngleOfIncidence(
                    sun.Zenith,
                    sun.Azimuth,
                    panel.Tilt,
                    panel.Azimuth);

            double cosTheta = Math.Max(0, Math.Cos(theta));

            // Beam
            double beam = irr.DNI * cosTheta * shadingFactor;

            // Diffuse (isotropic)
            double diffuse =
                irr.DHI * (1 + Math.Cos(panel.Tilt * Deg2Rad)) / 2.0;

            // Ground reflected
            double reflected =
                irr.GHI * panel.Albedo *
                (1 - Math.Cos(panel.Tilt * Deg2Rad)) / 2.0;

            return beam + diffuse + reflected;
        }

        private double CalculateAngleOfIncidence(
            double zenith,
            double sunAzimuth,
            double tilt,
            double panelAzimuth)
        {
            return Math.Acos(
                Math.Cos(zenith * Deg2Rad) *
                Math.Cos(tilt * Deg2Rad) +
                Math.Sin(zenith * Deg2Rad) *
                Math.Sin(tilt * Deg2Rad) *
                Math.Cos((sunAzimuth - panelAzimuth) * Deg2Rad)
            );
        }
    }
}
