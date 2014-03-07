using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

            if (text.EndsWith(Environment.NewLine))
                text = text.Substring(0, text.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));

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

        public static string RemoveLast(this string source, int removeCharacters)
        {
            if (source.Length >= removeCharacters)
                return source.Substring(0, source.Length - removeCharacters);
            return source;
        }

        public static string RemoveFirst(this string source, int removeCharacters)
        {
            if (source.Length >= removeCharacters)
                return source.Substring(removeCharacters);
            return source;
        }

        public static string FillUpFirstCharacters(this string source, int totalLength, char fillUpWith)
        {
            if (source == null) source = string.Empty;
            while (source.Length < totalLength) source = fillUpWith + source;
            return source;
        }

        public static string FillUpLastCharacters(this string source, int totalLength, char fillUpWith)
        {
            if (source == null) source = string.Empty;
            while (source.Length < totalLength) source = source + fillUpWith;
            return source;
        }

        public static Bitmap BitmapFromBase64(string base64)
        {
            try
            {
                Bitmap bitmap;
                using (var memory = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    bitmap = new Bitmap(memory);
                }
                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static string GetMd5Hash(this string source)
        {
            try
            {
                if (source == null) return string.Empty;
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(source);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (var i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        public static string RemoveTextBetween(this string source, string startDelimiter, string endDelimiter)
        {
            try
            {
                var startIndex = source.IndexOf(startDelimiter, StringComparison.CurrentCulture);
                var text = source.Substring(0,startIndex);

                var endIndex = source.IndexOf(endDelimiter, startIndex, StringComparison.CurrentCulture);
                return text+source.Substring(endIndex + endDelimiter.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    }
}
