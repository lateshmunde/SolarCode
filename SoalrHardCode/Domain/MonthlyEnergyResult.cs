namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Represents aggregated energy output for a single month.
    /// 
    /// This model is used purely for reporting and analytics.
    /// It contains no calculation logic.
    /// </summary>
    public class MonthlyEnergyResult
    {
        /// <summary>
        /// Month number (1 = January, 12 = December).
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Total energy produced in this month (kWh).
        /// </summary>
        public double EnergyKWh { get; }

        public MonthlyEnergyResult(int month, double energyKWh)
        {
            Month = month;
            EnergyKWh = energyKWh;
        }
    }
}
