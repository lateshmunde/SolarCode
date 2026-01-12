using System.Collections.Generic;

namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Represents a solar power plant as an aggregate root.
    /// 
    /// A Plant:
    /// - Has a location
    /// - Contains many identical or similar solar panels
    /// - Is the main unit for energy reporting (GWh, CUF, PR)
    /// 
    /// This class intentionally holds NO calculation logic.
    /// </summary>
    public class Plant
    {
        /// <summary>
        /// Physical site location of the plant.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Collection of panels installed at the plant.
        /// For POC, panels are assumed identical.
        /// </summary>
        public IReadOnlyList<SolarPanel> Panels { get; }

        /// <summary>
        /// Total DC capacity of the plant (kW).
        /// </summary>
        public double TotalDcCapacityKW { get; }

        public Plant(
            Location location,
            IReadOnlyList<SolarPanel> panels)
        {
            Location = location;
            Panels = panels;

            double capacity = 0;
            foreach (var panel in panels)
                capacity += panel.RatedPowerKW;

            TotalDcCapacityKW = capacity;
        }
    }
}
