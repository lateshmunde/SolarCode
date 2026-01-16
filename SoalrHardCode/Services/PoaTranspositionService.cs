using System;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Services
{
    public class PoaTranspositionService
    {
        private const double Albedo = 0.20; //Ground reflected light

        public double CalculatePoa(SolarPanel panel, SolarIrradiance irr)
        {
            double tiltRad = panel.TiltDeg * Math.PI / 180.0;
            double cosTilt = Math.Cos(tiltRad);

           
            return 
                irr.Dni * cosTilt +
                irr.Dhi * (1 + cosTilt) / 2.0 + 
                irr.Ghi * Albedo * (1 - cosTilt) / 2.0; //tilted panel irradiance
        }
    }
}
