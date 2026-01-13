namespace SolarEnergyPOC.Domain
{
    /// Represents aggregated energy output for a single month.
    /// This model is used purely for reporting and analytics -no calculation logic.
    public class MonthlyEnergyResult
    {
        /// Month number (1 = January, 12 = December).
        public int Month { get; }

        /// Total energy produced in this month (kWh).
        public double EnergyKWh { get; }

        public MonthlyEnergyResult(int month, double energyKWh)
        {
            Month = month;
            EnergyKWh = energyKWh;
        }
    }
}
