using System;
using System.Globalization;
using System.Linq;

namespace ContactPoint.Common
{
    public static class CommonExtensions
    {
        public static bool WasActive(this ICall call)
            => WasInState(call, CallState.ACTIVE);

        public static bool WasIncoming(this ICall call)
            => WasInState(call, CallState.INCOMING);

        public static bool WasConnecting(this ICall call)
            => WasInState(call, CallState.CONNECTING);

        public static bool WasAlerting(this ICall call)
            => WasInState(call, CallState.ALERTING);

        public static bool WasHolding(this ICall call)
            => WasInState(call, CallState.HOLDING);

        public static bool Check<T>(this T _, Func<T, bool> condition) where T : class
            => _ != null && condition(_);

        public static string ToFormattedString(this TimeSpan duration)
            => duration.ToString(duration.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss", CultureInfo.InvariantCulture);

        public static string ToFormattedString(this TimeSpan? duration, string defaultValue = null)
            => duration?.ToFormattedString() ?? defaultValue;

        private static bool WasInState(ICall call, CallState state)
            => call?.States.Any(x => x.State == state) == true;
    }
}
