using System;
using System.Globalization;

namespace dff.Extensions
{
    public static class StringExtensions
    {
        public static string ShortenString(this String text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (maxLength <= 3 | text.Length <= maxLength) return text;

            try
            {
                var mitte = Math.Floor((text.Length)*0.75);

                var ueberfluessig = text.Length - maxLength + 3;

                var vorn = text.Substring(0, (int) (mitte - (ueberfluessig*0.75)));
                var hinten = text.Substring((int) (mitte + (ueberfluessig*0.25)));

                text = vorn + "..." + hinten;
            }
            catch (Exception)
            {
                return text.Substring(0, maxLength - 3) + "...";
            }
            return text;
        }

        public static string RemoveLastSeperator(this String text)
        {
            if (text == null) return null;
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = text.TrimEnd(' ');

            while (text.EndsWith(",") |
                   text.EndsWith(";") |
                   text.EndsWith("|") |
                   text.EndsWith("@") |
                   text.EndsWith("#") |
                   text.EndsWith("+") |
                   text.EndsWith("*") |
                   text.EndsWith("-") |
                   text.EndsWith("_"))
            {
                text = text.Substring(0, text.Length - 1);
            }
            return text;
        }

        /// <summary>
        /// Im CF gibt es kein tyrParse. Also hier eine eigene Variante.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string value)
        {
            try
            {
                float.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Im CF gibt es kein tryParse. Macht ein Integer aus einem Sring, wenn möglich.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TryToInt(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gibt nach Übergabe eines Strings im Format "31.01.2004 12:00:00" das passende Date-Time Objekt zurück.
        /// </summary>
        /// <param name="date">String im Format 31.01.2004 12:00:00</param>
        /// <returns>DateTime Objekt</returns>
        public static DateTime GetDateTime(this string date)
        {
            try
            {
                IFormatProvider format = new CultureInfo("de-DE", true);
                return DateTime.Parse(date, format, DateTimeStyles.NoCurrentDateDefault);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static string GetLast(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }

    }
}
