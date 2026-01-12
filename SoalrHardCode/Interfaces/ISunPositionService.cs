namespace SolarEnergyPOC.Interfaces
{
    /// <summary>
    /// Contract for providing solar position data.
    /// Allows swapping simple or advanced sun models.
    /// </summary>
    public interface ISunPositionService
    {
        double GetSolarAltitudeDeg(int hour);
    }
}
