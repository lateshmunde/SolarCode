using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    public interface IShadingService
    {
        double CalculateShadingFactor(
            SunPosition sunPosition,
            PanelGeometry panelGeometry
        );
    }
}
