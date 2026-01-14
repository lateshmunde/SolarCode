using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services.Losses
{
    public class InverterLoss : IEnergyLoss
    {
        private readonly double efficiency;

        public string Name => "Inverter";

        public InverterLoss(double efficiency)
        {
            this.efficiency = efficiency;
        }

        public void Apply(EnergyContext ctx, SolarPanel panel)
        {
            ctx.AcPowerKW = ctx.DcPowerKW * efficiency;
            ctx.EnergyKWh = ctx.AcPowerKW;
        }
    }
}
