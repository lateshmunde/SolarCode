using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

public class ShadingLoss : IEnergyLoss
{
    public string Name => "Row-to-Row Shading (Table-Based, Pitch-Driven)";

    // Shadow-casting row (front row)
    private readonly RowLayoutParameters FrontRow;

    // Shadow-receiving row (current row)
    private readonly RowLayoutParameters CurrentRow;

    // Empirical electrical non-linearity (bypass diode behaviour)
    private const double ElectricalExponent = 1.3;

    public ShadingLoss(
        RowLayoutParameters frontRow,
        RowLayoutParameters currentRow)
    {
        FrontRow = frontRow;
        CurrentRow = currentRow;
    }

    public void Apply(EnergyContext ctx, SolarPanel panel)
    {
        // No sun above horizon
        if (ctx.SunAltitudeDeg <= 0)
            return;

        // Effective sun altitude considering terrain slope
        double effectiveAltitudeDeg =
            ctx.SunAltitudeDeg - CurrentRow.RowSlopeDeg;

        if (effectiveAltitudeDeg <= 0)
            return;

        double effectiveAltitudeRad =
            effectiveAltitudeDeg * Math.PI / 180.0;

        // TABLE GEOMETRY 

        // Horizontal projection of table in row direction
        double tableLength = panel.LengthMeters * Math.Cos(panel.TiltDeg * Math.PI / 180.0);

        // Vertical projection of table due to tilt
        double tableVerticalHeight = panel.LengthMeters * Math.Sin(panel.TiltDeg * Math.PI / 180.0);

        // Total shadow-casting height (top edge of front table)
        double shadowCastingHeight = FrontRow.PanelMountedHeightMeters + tableVerticalHeight;
        //PanelMountedHeightMeters -  lower edge mounting height from ground

       // Clear spacing between tables (end-to-start)
       double rowSpacingMeters = CurrentRow.AppliedPitchMeters - tableLength;

        if (rowSpacingMeters <= 0)
            return;

        //SHADOW GEOMETRY

        // Ray projection:
        // shadedHeight = shadowCastingHeight − spacing * tan(sunAltitude)
        double shadedHeight = shadowCastingHeight - rowSpacingMeters * Math.Tan(effectiveAltitudeRad);

        // No shading
        if (shadedHeight <= 0)
            return;

        // Shadow cannot exceed table height
        shadedHeight = Math.Min(shadedHeight, shadowCastingHeight);

        // Fraction of table height shaded
        double verticalShadingFraction = shadedHeight / shadowCastingHeight;

        // Area-based shading assumption (triangular growth)
        double shadedAreaFraction = verticalShadingFraction * verticalShadingFraction;

        // Electrical impact (non-linear response)
        double powerLossFraction = Math.Pow(shadedAreaFraction, ElectricalExponent);

        // Apply loss at POA level
        // (future refinement: apply only to DNI)
        ctx.Poa *= (1 - powerLossFraction);
    }
}
