namespace SolarEnergyPOC.Interfaces
{
    /// Contract for computing shading losses.
    /// Enables multiple shading strategies (row, terrain, objects).
    public interface IShadingService
    {
        double GetShadingLoss(double panelHeight, double sunAltitudeDeg);
        double GetShadingLossIdeal(double panelHeight, double sunAltitudeDeg);
    }
}
