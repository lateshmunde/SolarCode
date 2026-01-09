namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Holds solar and weather data for a single time step.
    /// 
    /// This is intentionally time-agnostic:
    /// - Can represent hourly, daily, or monthly data
    /// - Source can be API, CSV, or hardcoded
    /// </summary>
    public class SolarIrradiance
    {
        /// <summary>
        /// Hour of the day (0–23).
        /// Used only for orchestration, not physics.
        /// </summary>
        public int Hour { get; }

        /// <summary>
        /// Global Horizontal Irradiance (W/m²).
        /// Total solar radiation received on a horizontal surface.
        /// </summary>
        public double Ghi { get; }

        /// <summary>
        /// Direct Normal Irradiance (W/m²).
        /// Radiation received directly from the sun.
        /// </summary>
        public double Dni { get; }

        /// <summary>
        /// Diffuse Horizontal Irradiance (W/m²).
        /// Scattered radiation from the sky.
        /// </summary>
        public double Dhi { get; }

        /// <summary>
        /// Ambient air temperature (°C).
        /// Used for estimating cell temperature.
        /// </summary>
        public double AmbientTempC { get; }

        public SolarIrradiance(int hour, double ghi, double dni, double dhi, double ambientTempC)
        {
            Hour = hour;
            Ghi = ghi;
            Dni = dni;
            Dhi = dhi;
            AmbientTempC = ambientTempC;
        }
    }
}
