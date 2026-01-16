namespace SolarEnergyPOC.Domain
{
    /// Represents a single solar module or an equivalent unit panel.
    /// No calculation logic, contains only physical properties.
    public class SolarPanel
    {
        /// Tilt angle from horizontal (degrees).
        /// Typical fixed-tilt plants in Gujarat use ~25°.
        public double TiltDeg { get; }

        /// Azimuth angle (degrees).
        /// 180° = facing true south in northern hemisphere.
        public double AzimuthDeg { get; }

        /// Length of the panel from ground (meters).
        /// Required for row-to-row shading estimation.
        public double LengthMeters { get; }

        /// Rated DC power of the panel in kW.
        /// Example: 540 Wp = 0.54 kW.
        public double RatedPowerKW { get; }

        public double GroundSlopeDeg { get; }   // panel-specific

        public SolarPanel(double tiltDeg, double azimuthDeg, double lengthMeters, double ratedPowerKW, double groundSlopeDeg = 0)
        {
            TiltDeg = tiltDeg;
            AzimuthDeg = azimuthDeg;
            LengthMeters = lengthMeters;
            RatedPowerKW = ratedPowerKW;
            GroundSlopeDeg = groundSlopeDeg;
        }

        // Derived property (no storage)
        public double EffectiveHeightMeters =>
            LengthMeters * Math.Sin(TiltDeg * Math.PI / 180.0);

    }
}

