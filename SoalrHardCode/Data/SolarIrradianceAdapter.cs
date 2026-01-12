using System;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Services
{
    public static class SolarIrradianceAdapter
    {
        private static readonly TimeZoneInfo IndiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public static IrradianceInput ToInput(SolarIrradiance s)
        {
            if (s.Timestamp.Kind != DateTimeKind.Utc)
                throw new InvalidOperationException(
                    "SolarIrradiance timestamp must be UTC");

            var localTime =
                TimeZoneInfo.ConvertTimeFromUtc(
                    s.Timestamp,
                    IndiaTimeZone);

            return new IrradianceInput(
                localTime,
                s.Ghi,
                s.Dni,
                s.Dhi
            );
        }
    }
}
