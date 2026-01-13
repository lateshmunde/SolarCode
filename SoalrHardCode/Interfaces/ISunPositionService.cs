namespace SolarEnergyPOC.Interfaces
{
    /// Contract for providing solar position data.
    /// Allows swapping simple or advanced sun models.
    public interface ISunPositionService
    {
        double GetSolarAltitudeDeg(int hour);
    }
}
