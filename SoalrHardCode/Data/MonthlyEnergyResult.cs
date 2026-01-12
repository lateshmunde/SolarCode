namespace SolarEnergyPOC.Domain
{
    public class MonthlyEnergyResult
    {
        public int Month { get; }
        public double EnergyKWh { get; }
        public double SpecificYield { get; }
        public double CUF { get; }

        public MonthlyEnergyResult(
            int month,
            double energyKWh,
            double specificYield,
            double cuf)
        {
            Month = month;
            EnergyKWh = energyKWh;
            SpecificYield = specificYield;
            CUF = cuf;
        }
    }
}
