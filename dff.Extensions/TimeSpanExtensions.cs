using System;

namespace dff.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string GetDescription(this TimeSpan ts)
        {
            var diff = ts.Ticks > 0 ? "vor" + " " : "in" + " ";
            var min = ts.Minutes;
            var std = ts.Hours;
            var tage = ts.Days;
            if (tage == 0)
            {
                if (std > 1) diff += std + " " + "Stunden" + " ";
                else if (std == 1) diff += std + " " + "Stunde" + " ";
                if (min == 0) diff += min + " " + "Minute";
                else diff += min + " " + "Minuten";
            }
            else
            {
                if (tage == 1) diff += tage + " " + "Tag";
                else diff += tage + " " + "Tagen";
            }
            return diff;
        }

        public static DateTime Ago (this TimeSpan timeSpan)
        {
            return (DateTime.Now.Subtract(timeSpan));
        }
    }
}