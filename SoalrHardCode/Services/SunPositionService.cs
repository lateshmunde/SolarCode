namespace SolarEnergyPOC.Services
{
    /// <summary>
    /// Provides solar position information.
    /// 
    /// NOTE:
    /// This is a deliberately simplified model suitable for a POC.
    /// It avoids heavy astronomical formulas while preserving realism.
    /// 
    /// Can later be replaced with:
    /// - SPA / NREL models
    /// - External solar libraries
    /// </summary>
    public class SunPositionService
    {
        /// <summary>
        /// Returns approximate solar altitude angle (degrees) for a given hour.
        /// 
        /// Higher altitude → shorter shadows → higher energy.
        /// </summary>
        public double GetSolarAltitudeDeg(int hour)
        {
            return hour switch
            {
                <= 9 => 25,
                10 => 35,
                11 => 45,
                12 => 55,
                13 => 50,
                14 => 40,
                _ => 30
            };
        }
    }
}
