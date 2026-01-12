namespace SolarEnergyPOC.Interfaces
{
    /// <summary>
    /// Contract for computing shading losses.
    /// Enables multiple shading strategies (row, terrain, objects).
    /// </summary>
    public interface IShadingService
    {
        double GetShadingLoss(double panelHeight, double sunAltitudeDeg);
    }
}
