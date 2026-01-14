using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services.Losses
{
    public class SoilingLoss : IEnergyLoss
    {
        private readonly double loss;

        public string Name => "Soiling";

        public SoilingLoss(double lossFraction)
        {
            loss = lossFraction;
        }

        public void Apply(EnergyContext ctx, SolarPanel panel)
        {
            ctx.Poa *= (1 - loss);
        }
    }
}
