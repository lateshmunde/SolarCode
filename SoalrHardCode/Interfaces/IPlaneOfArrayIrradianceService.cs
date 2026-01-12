using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    public interface IPlaneOfArrayIrradianceService
    {
        double CalculatePOA(
            IrradianceInput irradiance,
            SunPosition sunPosition,
            PanelGeometry panelGeometry,
            double shadingFactor
        );
    }
}
