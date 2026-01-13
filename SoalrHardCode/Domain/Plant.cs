using System.Collections.Generic;

namespace SolarEnergyPOC.Domain
{
    // Represents a solar power plant as an aggregate root.
    public class Plant
    {
        /// Physical site location of the plant.
        public Location Location { get; }

        /// Collection of panels installed at the plant - identical panels
        public IReadOnlyList<SolarPanel> Panels { get; }

        /// Total DC capacity of the plant (kW).
        public double TotalDcCapacityKW { get; }

        public Plant( Location location,IReadOnlyList<SolarPanel> panels)
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
