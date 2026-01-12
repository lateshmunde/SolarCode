using System.Collections.Generic;

namespace SolarEnergyPOC.Domain
{
    public class Plant
    {
        public Location Location { get; }
        public IReadOnlyList<SolarPanel> Panels { get; }
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
