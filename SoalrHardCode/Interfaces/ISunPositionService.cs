using System;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    public interface ISunPositionService
    {
        SunPosition GetSunPosition(DateTime dateTime);
    }
}
