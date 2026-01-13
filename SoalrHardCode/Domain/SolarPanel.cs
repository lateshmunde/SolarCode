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

        /// Vertical height of the panel from ground (meters).
        /// Required for row-to-row shading estimation.
        public double HeightMeters { get; }

        /// Rated DC power of the panel in kW.
        /// Example: 540 Wp = 0.54 kW.
        public double RatedPowerKW { get; }

        public SolarPanel(double tiltDeg, double azimuthDeg, double heightMeters, double ratedPowerKW)
        {
            TiltDeg = tiltDeg;
            AzimuthDeg = azimuthDeg;
            HeightMeters = heightMeters;
            RatedPowerKW = ratedPowerKW;
        }
    }
}
