using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    /// Atomic loss unit (PVcase-style)
    public interface IEnergyLoss
    {
        string Name { get; }
        void Apply(EnergyContext context, SolarPanel panel);
    }
}
