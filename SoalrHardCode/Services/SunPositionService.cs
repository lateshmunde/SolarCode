using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class SunPositionService : ISunPositionService
    {
        // Gujarat / Ahmedabad defaults
        private const double LatitudeDeg = 23.0;
        private const double LongitudeDeg = 72.0;

        // IST standard meridian
        private const double StandardMeridianDeg = 82.5;

        public SunPosition GetSunPosition(DateTime localTime)
        {
            // 1️⃣ Convert clock time → solar time
            DateTime solarTime = ApplySolarTimeCorrection(localTime);

            // 2️⃣ Solar declination
            double declinationRad =
                23.45 * Math.Sin(
                    Deg2Rad(360.0 / 365.0 * (solarTime.DayOfYear - 81))
                ) * Math.PI / 180.0;

            // 3️⃣ Hour angle
            double solarHour =
                solarTime.Hour +
                solarTime.Minute / 60.0 +
                solarTime.Second / 3600.0;

            double hourAngleRad =
                Deg2Rad(15.0 * (solarHour - 12.0));

            // 4️⃣ Latitude
            double latRad = Deg2Rad(LatitudeDeg);

            // 5️⃣ Zenith angle
            double cosZenith =
                Math.Sin(latRad) * Math.Sin(declinationRad) +
                Math.Cos(latRad) * Math.Cos(declinationRad) *
                Math.Cos(hourAngleRad);

            cosZenith = Math.Clamp(cosZenith, -1.0, 1.0);

            double zenithDeg =
                Math.Acos(cosZenith) * 180.0 / Math.PI;

            return new SunPosition
            {
                Zenith = zenithDeg,
                Azimuth = 180 // south-facing approximation
            };
        }

        // -------------------------------
        // SOLAR TIME CORRECTION
        // -------------------------------

        private DateTime ApplySolarTimeCorrection(DateTime localTime)
        {
            int n = localTime.DayOfYear;

            // Equation of Time (minutes)
            double B = Deg2Rad((360.0 / 365.0) * (n - 81));
            double eot =
                9.87 * Math.Sin(2 * B) -
                7.53 * Math.Cos(B) -
                1.5 * Math.Sin(B);

            // Longitude correction (minutes)
            double longitudeCorrection =
                4.0 * (LongitudeDeg - StandardMeridianDeg);

            double totalMinutes =
                longitudeCorrection + eot;

            return localTime.AddMinutes(totalMinutes);
        }

        private static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }
    }
}
