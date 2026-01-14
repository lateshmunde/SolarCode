using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services.Losses
{
    public class ShadingLoss : IEnergyLoss
    {
        private const double RowSpacing = 6.0;

        public string Name => "Row Shading";

        public void Apply(EnergyContext ctx, SolarPanel panel)
        {
            if (ctx.SunAltitudeDeg <= 0) { ctx.Poa = 0; return; }

            double altRad = ctx.SunAltitudeDeg * Math.PI / 180.0;
            double shadow = panel.HeightMeters / Math.Tan(altRad);

            if (shadow <= RowSpacing) return;

            double loss = Math.Min((shadow - RowSpacing) / panel.HeightMeters, 1.0);
            ctx.Poa *= (1 - loss);
        }
    }
}
