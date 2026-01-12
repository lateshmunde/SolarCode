using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    /// <summary>
    /// Calculates energy output at the plant level.
    /// 
    /// Responsibilities:
    /// - Aggregate panel-level energy
    /// - Scale hourly data to annual values
    /// - Keep Program.cs free from business logic
    /// </summary>
    public class PlantEnergyService
    {
        private readonly ISunPositionService _sunService;
        private readonly IShadingService _shadingService;
        private readonly IEnergyCalculationService _energyService;

        public PlantEnergyService(
            ISunPositionService sunService,
            IShadingService shadingService,
            IEnergyCalculationService energyService)
        {
            _sunService = sunService;
            _shadingService = shadingService;
            _energyService = energyService;
        }

        /// <summary>
        /// Calculates total daily energy for the plant (kWh).
        /// </summary>
        public double CalculateDailyEnergy(
            Plant plant,
            IEnumerable<SolarIrradiance> hourlyData)
        {
            double dailyEnergy = 0;

            foreach (var irradiance in hourlyData)
            {
                double sunAltitude =
                    _sunService.GetSolarAltitudeDeg(irradiance.Hour);

                foreach (var panel in plant.Panels)
                {
                    double shadingLoss =
                        _shadingService.GetShadingLoss(
                            panel.HeightMeters,
                            sunAltitude
                        );

                    dailyEnergy += _energyService.CalculateHourlyEnergy(
                        panel,
                        irradiance,
                        sunAltitude,
                        shadingLoss
                    );
                }
            }

            return dailyEnergy;
        }

        /// <summary>
        /// Scales daily energy to yearly energy.
        /// 
        /// This assumes the provided day is a
        /// representative average day.
        /// </summary>
        public double ScaleDailyToYearly(double dailyEnergy)
        {
            const int DaysInYear = 365;
            return dailyEnergy * DaysInYear;
        }
    }
}
