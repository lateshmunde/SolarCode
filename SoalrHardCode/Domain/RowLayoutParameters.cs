namespace SolarEnergyPOC.Domain
{
    public class RowLayoutParameters
    {
        public double AppliedPitchMeters { get; set; }        // center-to-center pitch (as-built)
        public double RowSlopeDeg { get; set; }               // terrain slope
        public double PanelMountedHeightMeters { get; set; }  // lower edge mounting height from ground
    }
}
