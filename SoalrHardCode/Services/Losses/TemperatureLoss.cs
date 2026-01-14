using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services.Losses
{
    public class TemperatureLoss : IEnergyLoss
    {
        private const double NOCT = 45;
        private const double TempCoeff = -0.004;

        public string Name => "Temperature";

        public void Apply(EnergyContext ctx, SolarPanel panel)
        {
            ctx.CellTemperatureC =
                ctx.CellTemperatureC +
                (ctx.Poa / 800.0) * (NOCT - 20);

            double factor = 1 + TempCoeff * (ctx.CellTemperatureC - 25);
            ctx.DcPowerKW *= factor;
        }
    }
}
