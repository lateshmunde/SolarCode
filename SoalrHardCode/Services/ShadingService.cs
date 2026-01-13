using SolarEnergyPOC.Interfaces;
using System;

namespace SolarEnergyPOC.Services
{
    /// Calculates basic row-to-row shading losses.
    public class ShadingService : IShadingService
    {
        // Typical row spacing for utility-scale plants (meters)
        private const double RowSpacingMeters = 6.0;

        /// Computes fractional shading loss (0–1) based on
        /// panel height and sun altitude.
        public double GetShadingLoss(double panelHeight, double sunAltitudeDeg)
        {
            //double sunAltitudeRad = sunAltitudeDeg * Math.PI / 180.0;
            //double shadowLength = panelHeight / Math.Tan(sunAltitudeRad);

            //if (shadowLength <= RowSpacingMeters)
            //    return 0.0;

            //return Math.Min((shadowLength - RowSpacingMeters) / panelHeight, 1.0 );
            return 0;
        }
    }
}
