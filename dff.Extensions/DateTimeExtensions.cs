using System;
using System.Globalization;

namespace dff.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gibt nach Übergabe eines DateTime-Objektes einen String im Format "31.01.2004 12:00:00" zurück.
        /// </summary>
        /// <param name="date">DateTime Objekt</param>
        /// <returns>String im Format "31.01.2004 12:00:00"</returns>
        public static string GetDateTimeString(this DateTime date)
        {
            try
            {
                IFormatProvider format = new CultureInfo("de-DE", true);
                return date.ToString("G", format);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gibt nach Übergabe eines DateTime-Objektes einen String im Format "31.01.2004 12:00:00" zurück.
        /// </summary>
        /// <param name="date">DateTime Objekt</param>
        /// <returns>String im Format "31.01.2004 12:00:00"</returns>
        public static string GetDateTimeStringIso(this DateTime date)
        {
            try
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}