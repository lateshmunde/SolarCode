using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class LossPipeline
    {
        private readonly List<IEnergyLoss> losses;

        public LossPipeline(IEnumerable<IEnergyLoss> losses)
        {
            this.losses = new List<IEnergyLoss>(losses);
        }

        public double Run(EnergyContext context, SolarPanel panel)
        {
            foreach (var loss in losses)
                loss.Apply(context, panel);

            return context.EnergyKWh;
        }
    }
}
