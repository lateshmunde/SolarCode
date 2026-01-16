using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class SunPositionService : ISunPositionService
    {
        private const double StdMeridian = 82.5;
        

        public double GetSolarAltitude(DateTime localTime, Location loc)
        {
            double Lon = loc.Longitude;
            double Lat = loc.Latitude;
            int n = localTime.DayOfYear;

            //Solar Declination
            double decl = 23.45 * Math.Sin(DegToRad(360.0 * (284 + n) / 365.0));
            //Equation of Time
            double eot =
                9.87 * Math.Sin(DegToRad(2 * B(n)))
                - 7.53 * Math.Cos(DegToRad(B(n)))
                - 1.5 * Math.Sin(DegToRad(B(n)));

            double timeCorrection = 4 * (Lon - StdMeridian) + eot;

            double solarTime = localTime.Hour + localTime.Minute / 60.0 + timeCorrection / 60.0;

            double hourAngle = 15 * (solarTime - 12);

            double altitude =
                Math.Asin(
                    Math.Sin(DegToRad(Lat)) * Math.Sin(DegToRad(decl)) +
                    Math.Cos(DegToRad(Lat)) * Math.Cos(DegToRad(decl)) *
                    Math.Cos(DegToRad(hourAngle)));

            return RadToDeg(altitude);
        }

        private static double B(int n) => 360.0 * (n - 81) / 364.0;
        private static double DegToRad(double d) => d * Math.PI / 180.0;
        private static double RadToDeg(double r) => r * 180.0 / Math.PI;
    }

}