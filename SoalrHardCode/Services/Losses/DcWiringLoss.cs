using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services.Losses
{
    public class DcWiringLoss : IEnergyLoss
    {
        private readonly double loss;

        public string Name => "DC Wiring";

        public DcWiringLoss(double lossFraction)
        {
            loss = lossFraction;
        }

        public void Apply(EnergyContext ctx, SolarPanel panel)
        {
            ctx.DcPowerKW *= (1 - loss);
        }
    }
}
