using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

public class ShadingLoss : IEnergyLoss
{
    public string Name => "Row-to-Row Shading (Area-Based)";

    private readonly SiteLayoutParameters Layout; // criticalAltitudeRad 
    private const double ElectricalExponent = 1.3; // bypass diode behavior

    public ShadingLoss(SiteLayoutParameters layout)
    {
        Layout = layout;
    }
    public void Apply(EnergyContext ctx, SolarPanel panel)
    {
        // No sun above horizon
        if (ctx.SunAltitudeDeg <= 0)
        {
            ctx.Poa = 0;
            return;
        }

        // Effective sun altitude (panel-specific slope)
        double effectiveAltitudeDeg = ctx.SunAltitudeDeg - panel.GroundSlopeDeg;

        if (effectiveAltitudeDeg <= 0)
        {
            ctx.Poa = 0;
            return;
        }

        double effectiveAltitudeRad = effectiveAltitudeDeg * Math.PI / 180.0;

        // Effective vertical height of panel
        double panelHeightMeters = panel.EffectiveHeightMeters;

        // Row spacing logic
        double criticalAltitudeRad = Layout.CriticalSunAltitudeDeg * Math.PI / 180.0;

        double rowSpacingMeters = panelHeightMeters / Math.Tan(criticalAltitudeRad);

        // Exact ray-projection shaded vertical height
        // h_s = max(0, H - P * tan(alpha))
        double shadedHeightMeters = panelHeightMeters - rowSpacingMeters * Math.Tan(effectiveAltitudeRad);

        // No shading
        if (shadedHeightMeters <= 0)
            return;

        // Cap to full panel height
        shadedHeightMeters = Math.Min(shadedHeightMeters, panelHeightMeters);

        // Vertical fraction shaded
        double verticalShadingFraction = shadedHeightMeters / panelHeightMeters;

        // Area-based shading (triangular growth)
        double shadedAreaFraction = verticalShadingFraction * verticalShadingFraction;

        // Electrical response (nonlinear, bypass-diode aware)
        double powerLossFraction = Math.Pow(shadedAreaFraction, ElectricalExponent);

        // Apply loss to POA
        ctx.Poa *= (1 - powerLossFraction);
    }
}
