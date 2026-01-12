namespace SolarEnergyPOC.Interfaces
{
    public interface IEnergyCalculationService
    {
        double CalculateFromPOA(double poaIrradiance);
    }
}
