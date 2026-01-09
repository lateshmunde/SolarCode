namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Represents a single solar module or an equivalent unit panel.
    /// 
    /// This model deliberately contains only physical properties.
    /// No calculation logic is placed here to keep the domain clean.
    /// </summary>
    public class SolarPanel
    {
        /// <summary>
        /// Tilt angle from horizontal (degrees).
        /// Typical fixed-tilt plants in Gujarat use ~25°.
        /// </summary>
        public double TiltDeg { get; }

        /// <summary>
        /// Azimuth angle (degrees).
        /// 180° = facing true south in northern hemisphere.
        /// </summary>
        public double AzimuthDeg { get; }

        /// <summary>
        /// Vertical height of the panel from ground (meters).
        /// Required for row-to-row shading estimation.
        /// </summary>
        public double HeightMeters { get; }

        /// <summary>
        /// Rated DC power of the panel in kW.
        /// Example: 540 Wp = 0.54 kW.
        /// </summary>
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
