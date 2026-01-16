
namespace SolarEnergyPOC.Domain
{
    /// Design-time layout parameters.
    /// These do NOT change during simulation.
    public class SiteLayoutParameters
    {
        /// Critical sun altitude (degrees) used to derive row spacing.
        /// Computed from latitude + date + time (winter design sun).
        /// Dec 21, 9:00 AM
        public double CriticalSunAltitudeDeg { get; init; }
    }
}

