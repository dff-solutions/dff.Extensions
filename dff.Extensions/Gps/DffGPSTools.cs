#region

using System;
using System.Globalization;

#endregion

namespace dff.Extensions.Gps
{
    /// <summary>
    ///     Sammlung von Hilfmethoden für die Verarbeitung von GPS Daten.
    /// </summary>
    public class DffGpsTools
    {
        #region Transformation von GPS Daten in versch. Formate

        /// <summary>
        ///     Liefert die Entfernung zwischen zwei Koordinaten im DEC Format in Metern.
        /// </summary>
        /// <param name="p1XDec">1. Koordinate Breite</param>
        /// <param name="p1YDec">1. Koordinate Länge</param>
        /// <param name="p2XDec">2. Koordinate Breite</param>
        /// <param name="p2YDec">2. Koordinate Länge</param>
        /// <returns>Entfernung in Metern</returns>
        public static double GetDistanceDec(string p1XDec,
            string p1YDec,
            string p2XDec,
            string p2YDec)
        {
            IFormatProvider format = new CultureInfo("en-US", true);

            var p1X = Double.Parse(p1XDec, format)/100000;
            var p1Y = Double.Parse(p1YDec, format)/100000;
            var p2X = Double.Parse(p2XDec, format)/100000;
            var p2Y = Double.Parse(p2YDec, format)/100000;

            const double pi = 3.1415926535897932384626433832795;
            const double cf = 180/pi;
            var c = (Math.Sin(p1Y/cf)*Math.Sin(p2Y/cf)) +
                    ((Math.Cos(p1Y/cf)*Math.Cos(p2Y/cf))*
                     Math.Cos((p2X/cf) - (p1X/cf)));
            return Math.Round((6378.136*Math.Acos(c)*1000));
        }

        /// <summary>
        ///     Liefert die Entfernung zwischen zwei Koordinaten im DME Format.
        /// </summary>
        /// <param name="p1XDme">1. Koordinate Breite</param>
        /// <param name="p1YDme">1. Koordinate Länge</param>
        /// <param name="p2XDme">2. Koordinate Breite</param>
        /// <param name="p2YDme">2. Koordinate Länge</param>
        /// <returns>Entfernung in Metern.</returns>
        public static double GetDistanceDme(string p1XDme,
            string p1YDme,
            string p2XDme,
            string p2YDme)
        {
            IFormatProvider format = new CultureInfo("en-US", true);

            //Werte in DEC wandeln
            var strP1X = "";
            var strP1Y = "";
            var strP2X = "";
            var strP2Y = "";
            Geodms2Geodec(p1XDme, p1YDme, ref strP1X, ref strP1Y);
            Geodms2Geodec(p2XDme, p2YDme, ref strP2X, ref strP2Y);
            var p1X = Double.Parse(strP1X, format);
            var p1Y = Double.Parse(strP1Y, format);
            var p2X = Double.Parse(strP2X, format);
            var p2Y = Double.Parse(strP2Y, format);

            //Entfernungsberechnung
            const double pi = 3.1415926535897932384626433832795;
            var teil1 = Math.Pow((p2Y - p1Y), 2);
            var teil2 = Math.Pow(((p2X - p1X)*(Math.Cos((p2Y + p1Y)*pi/360000000))), 2);
            return Math.Round((6.371*Math.Sqrt(teil1 + teil2)*pi/18));
        }

        /// <summary>
        ///     Wandelt Km/h in Knoten um
        /// </summary>
        /// <param name="kmh"></param>
        public static double Kmh2Knoten(double kmh)
        {
            return (kmh*0.539956803);
        }

        /// <summary>
        ///     Wandlet einen GeoDME-Wert in einen GeoDMS Wert um.
        /// </summary>
        /// <param name="xGeodec"></param>
        /// <param name="yGeodec"></param>
        /// <param name="north"></param>
        /// <param name="east"></param>
        /// <param name="xGeodms"></param>
        /// <param name="yGeodms"></param>
        public static void Geodme2Geodms(string xGeodec,
            string yGeodec,
            bool north,
            bool east,
            ref string xGeodms,
            ref string yGeodms)
        {
            if (yGeodms == null) throw new ArgumentNullException("yGeodms");
            IFormatProvider format = new CultureInfo("en-US", true);

            //Ermittlung der Stunden
            var xGeodecDeg = xGeodec.Substring(0, (xGeodec.IndexOf(".", StringComparison.Ordinal) - 2));
            var yGeodecDeg = yGeodec.Substring(0, (yGeodec.IndexOf(".", StringComparison.Ordinal) - 2));

            //Abschneiden einer eventuell zusätzlichen 0 am Beginn
            if (xGeodecDeg.Length > 2)
            {
                xGeodecDeg = xGeodecDeg.Substring(xGeodecDeg.Length - 2);
            }
            if (yGeodecDeg.Length > 2)
            {
                yGeodecDeg = yGeodecDeg.Substring(yGeodecDeg.Length - 2);
            }

            //Ermittlung der Minuten aus dem String
            var xGeodecMin = xGeodec.Substring((xGeodec.IndexOf(".", StringComparison.Ordinal) - 2));
            var yGeodecMin = yGeodec.Substring((yGeodec.IndexOf(".", StringComparison.Ordinal) - 2));

            //Ermittlung der Minuten als Double 
            var xMin = (double.Parse(xGeodecMin, format));
            var yMin = (double.Parse(yGeodecMin, format));

            //Ermittlung der Sekunden
            var xSec = (xMin - Math.Floor(xMin))*600;
            var ySec = (yMin - Math.Floor(yMin))*600;

            //Runden
            xMin = Math.Floor(xMin);
            yMin = Math.Floor(yMin);
            xSec = Math.Round(xSec);
            ySec = Math.Round(ySec);

            //Auffüllen der Nullen bei Minuten
            if (xMin < 10)
            {
                xGeodecMin = "0" + xMin;
            }
            else
            {
                xGeodecMin = xMin + "";
            }

            //Nullen bei Sekunden auffüllen
            var xGeodecSec = xSec + "";
            if (xSec < 100)
            {
                if (xSec < 10)
                {
                    xGeodecSec = "00" + xSec;
                }
                else
                {
                    xGeodecSec = "0" + xSec;
                }
            }

            //Auffüllen der Nullen bei Minuten
            if (yMin < 10)
            {
                yGeodecMin = "0" + yMin;
            }
            else
            {
                yGeodecMin = yMin + "";
            }

            //Nullen bei Sekunden auffüllen
            var yGeodecSec = ySec + "";
            if (ySec < 100)
            {
                if (ySec < 10)
                {
                    yGeodecSec = "00" + ySec;
                }
                else
                {
                    yGeodecSec = "0" + ySec;
                }
            }
            //Zusammensetzten
            xGeodms = xGeodecDeg + xGeodecMin + xGeodecSec;
            yGeodms = yGeodecDeg + yGeodecMin + yGeodecSec;

            //Eventuell ein Minus setzten
            if (!east)
            {
                xGeodms = "-" + xGeodms;
            }
            if (!north)
            {
                yGeodms = "-" + yGeodms;
            }
        }

        /// <summary>
        ///     Wandelt einen GeoDMS-Wert in einen GeoDEC Wert.
        /// </summary>
        /// <param name="xGeodms"></param>
        /// <param name="yGeodms"></param>
        /// <param name="xGeodec"></param>
        /// <param name="yGeodec"></param>
        public static void Geodms2Geodec(string xGeodms,
            string yGeodms,
            ref string xGeodec,
            ref string yGeodec)
        {
            if (xGeodec == null) throw new ArgumentNullException("xGeodec");
            IFormatProvider format = new CultureInfo("en-US", true);

            var xDms = double.Parse(xGeodms);
            var yDms = double.Parse(yGeodms);

            var x_minus = false;
            if (xDms < 0)
            {
                x_minus = true;
            }

            var y_minus = false;
            if (yDms < 0)
            {
                y_minus = true;
            }

            //Sekunden
            var xSec = double.Parse(xGeodms.Substring(xGeodms.Length - 3), format);
            xSec = (xSec*0.2777777777777777777777777777777777)*10;

            var ySec = double.Parse(yGeodms.Substring(yGeodms.Length - 3), format);
            ySec = (ySec*0.2777777777777777777777777777777777)*10;

            //Minuten
            var xMin = double.Parse(xGeodms.Substring(xGeodms.Length - 5, 2), format);
            xMin = (xMin*1.6666666666666666666666666666666666)*1000;

            var yMin = double.Parse(yGeodms.Substring(yGeodms.Length - 5, 2), format);
            yMin = (yMin*1.6666666666666666666666666666666666)*1000;

            //Grad
            var xDeg = double.Parse(xGeodms.Substring(xGeodms.Length - 7, 2), format)*100000;

            var yDeg = double.Parse(yGeodms.Substring(yGeodms.Length - 7, 2), format)*100000;

            xGeodec = Math.Round(xDeg) + Math.Round(xMin) + Math.Round(xSec) + "";
            yGeodec = Math.Round(yDeg) + Math.Round(yMin) + Math.Round(ySec) + "";

            if (x_minus)
            {
                xGeodec = "-" + xGeodec;
            }
            if (y_minus)
            {
                yGeodec = "-" + yGeodec;
            }
        }

        /// <summary>
        ///     Wandlet einen GeoDEC-Wert in einen GeoDMS Wert um.
        /// </summary>
        /// <param name="xGeodec"></param>
        /// <param name="yGeodec"></param>
        /// <param name="xGeodms"></param>
        /// <param name="yGeodms"></param>
        public static void Geodec2Geodms(string xGeodec,
            string yGeodec,
            ref string xGeodms,
            ref string yGeodms)
        {
            if (xGeodms == null) throw new ArgumentNullException("xGeodms");
            var xDec = double.Parse(xGeodec);
            var yDec = double.Parse(yGeodec);

            var x_minus = false;
            if (xDec < 0)
            {
                x_minus = true;
            }

            var y_minus = false;
            if (yDec < 0)
            {
                y_minus = true;
            }

            //degree
            var xGeoDeg = Math.Floor(xDec/100000);
            var yGeoDeg = Math.Floor(yDec/100000);

            //minutes
            var xGeoMin = (((xDec/100000) - xGeoDeg)*100)/1.6666666666666666666666666;
            var yGeoMin = (((yDec/100000) - yGeoDeg)*100)/1.6666666666666666666666666;

            //seconds
            var xGeoSec = Math.Abs(xGeoMin - Math.Floor(xGeoMin))*600;
            var yGeoSec = Math.Abs(yGeoMin - Math.Floor(yGeoMin))*600;

            //GEODMS

            xGeoMin = Math.Abs(Math.Floor(xGeoMin));
            yGeoMin = Math.Abs(Math.Floor(yGeoMin));

            xGeoSec = Math.Abs(Math.Round(xGeoSec));
            yGeoSec = Math.Abs(Math.Round(yGeoSec));

            var strXGeoMin = xGeoMin + "";
            var strYGeoMin = yGeoMin + "";
            if (xGeoMin < 10)
            {
                strXGeoMin = "0" + xGeoMin;
            }
            if (yGeoMin < 10)
            {
                strYGeoMin = "0" + yGeoMin;
            }

            var strXGeoSec = xGeoSec + "";
            var strYGeoSec = yGeoSec + "";
            if (xGeoSec < 100)
            {
                if (xGeoSec < 10)
                {
                    strXGeoSec = "00" + xGeoSec;
                }
                else
                {
                    strXGeoSec = "0" + xGeoSec;
                }
            }
            if (yGeoSec < 100)
            {
                if (yGeoSec < 10)
                {
                    strYGeoSec = "00" + yGeoSec;
                }
                else
                {
                    strYGeoSec = "0" + yGeoSec;
                }
            }

            xGeodms = xGeoDeg + strXGeoMin + strXGeoSec;
            yGeodms = yGeoDeg + strYGeoMin + strYGeoSec;

            if (x_minus)
            {
                xGeodms = "-" + xGeodms;
            }
            if (y_minus)
            {
                yGeodms = "-" + yGeodms;
            }
        }

        public static void GeodecimalToMercator(double fLongitude,
            double fLatitude,
            ref int mercatorX)
        {
            int mercatorY = 0;
            GeodecimalToMercator(fLongitude, fLatitude, ref mercatorX, ref mercatorY);
        }

        public static void GeodecimalToMercator(double fLongitude,
            double fLatitude,
            ref int mercatorX,
            ref int MercatorY)
        {
            mercatorX = (int) ((float) fLongitude*(6371000*(Math.PI/180)));
            MercatorY =
                (int)
                    (Math.Log(Math.Tan((((float) fLatitude*(Math.PI/360)) + (Math.PI/4))))*
                     6371000);
        }

        public static void MercatorToGeodecimal(int mercatorX,
            int mercatorY,
            ref double fLongitude,
            ref double fLatitude)
        {
            fLongitude = Math.Round(((mercatorX*(180/Math.PI))/6371000), 5);
            fLatitude =
                Math.Round(
                    (2*(180/Math.PI))*
                    (Math.Atan(Math.Exp(mercatorY/(double) 6371000)) - (Math.PI/4)), 5);
        }

        public static int GetDistanceMercator(int p1X, int p1Y, int p2X, int p2Y)
        {
            double dx = p1X - p2X;
            double dy = p1Y - p2Y;

            double airDis = Math.Round(
                (Math.Cos(2*(Math.Atan(Math.Exp(((p1Y + p2Y)/2)/6371000)) - Math.PI/4))*
                 Math.Sqrt(dx*dx + dy*dy)), 0);
            return (int) airDis;
        }

        public static int GetDistanceDffStrings(string dff1, string dff2)
        {
            var x1 = 0;
            var y1 = 0;
            var x2 = 0;
            var y2 = 0;
            DffPositionStringToMercator(dff1, ref x1, ref y1);
            DffPositionStringToMercator(dff2, ref x2, ref y2);
            return GetDistanceMercator(x1, y1, x2, y2);
        }

        public static void DffPositionStringToMercator(string dff, ref int mercX, ref int mercY)
        {
            if (string.IsNullOrEmpty(dff)) return;

            IFormatProvider format = new CultureInfo("en-US", true);

            var items = dff.Split("|".ToCharArray());
            var yDms = items[0] + "." + items[1] + items[2];
            var xDms = items[3] + "." + items[4] + items[5];
            var dmsLatitude = double.Parse(xDms, format);
            var dmsLongitude = double.Parse(yDms, format);

            var decLatString = "";
            var decLongString = "";

            var latNeg = false;
            var longNeg = false;
            var dmsLatString = dmsLatitude.ToString(format);
            var dmsLongString = dmsLongitude.ToString(format);
            if (dmsLatString.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                dmsLatString = dmsLatString + ".00000";
            }
            if (dmsLongString.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                dmsLongString = dmsLongString + ".00000";
            }

            if (dmsLatString.StartsWith("-"))
            {
                latNeg = true;
                dmsLatString = dmsLatString.Substring(1);
            }
            if (dmsLongString.StartsWith("-"))
            {
                longNeg = true;
                dmsLongString = dmsLongString.Substring(1);
            }

            while (dmsLatString.Substring(dmsLatString.IndexOf(".", StringComparison.Ordinal) + 1).Length < 5)
            {
                dmsLatString = dmsLatString + "0";
            }
            while (dmsLongString.Substring(dmsLongString.IndexOf(".", StringComparison.Ordinal) + 1).Length < 5)
            {
                dmsLongString = dmsLongString + "0";
            }
            while (dmsLatString.Length < 8)
            {
                dmsLatString = "0" + dmsLatString;
            }
            while (dmsLongString.Length < 8)
            {
                dmsLongString = "0" + dmsLongString;
            }
            if (latNeg)
            {
                dmsLatString = "-" + dmsLatString;
            }
            if (longNeg)
            {
                dmsLongString = "-" + dmsLongString;
            }

            Geodms2Geodec(dmsLatString.Replace(".", ""), dmsLongString.Replace(".", ""),
                ref decLatString, ref decLongString);
            var decLongitude = double.Parse(decLongString, format)/100000;
            var decLatitude = double.Parse(decLatString, format)/100000;

            mercX = (int) ((float) decLongitude*(6371000*(Math.PI/180)));
            mercY =
                (int)
                    (Math.Log(Math.Tan((((float) decLatitude*(Math.PI/360)) + (Math.PI/4))))*
                     6371000);
        }

        public static int CheckIfDeviceIsInGeoFenceDff(string location, string fence, int toleranz)
        {
            var inFenceMarker = false;
            try
            {
                //Das hintere Semikolon abschneiden
                if (fence.EndsWith(";"))
                {
                    fence = fence.Substring(0, fence.Length - 1);
                }

                var dffFormatLastLocation = location.Replace("|", "");
                var breiteLldms = dffFormatLastLocation.Substring(0, 7);
                var laengeLldms = dffFormatLastLocation.Substring(7, 7);
                var breiteLldec = "";
                var laengeLldec = "";
                Geodms2Geodec(breiteLldms, laengeLldms, ref breiteLldec, ref laengeLldec);

                var fences = fence.Split(';');
                for (var z = 0; z < fences.Length; z++)
                {
                    if (fences[z].Length == 0)
                    {
                        continue; //Falls ein Markt keine Position hat.
                    }
                    var rad = int.Parse(fences[z].Substring(0, fences[z].IndexOf("@", StringComparison.Ordinal)));
                    var pos = fences[z].Substring(fences[z].IndexOf("@", StringComparison.Ordinal) + 1).Replace("|", "");
                    var breiteFenceDms = pos.Substring(0, 7);
                    var laengeFenceDms = pos.Substring(7, 7);
                    var breiteFenceDec = "";
                    var laengeFenceDec = "";
                    Geodms2Geodec(breiteFenceDms, laengeFenceDms, ref breiteFenceDec,
                        ref laengeFenceDec);

                    var dis = GetDistanceDec(breiteLldec, laengeLldec, breiteFenceDec,
                        laengeFenceDec);
                    if (dis <= (rad + toleranz + 10))
                    {
                        inFenceMarker = true;
                    }
                }
            }
            catch
            {
                return -1;
            }
            if (inFenceMarker)
            {
                return 1;
            }
            return 0;
        }

        public static string GetMiddleOfPositionsDff(string pos1Dff, string pos2Dff)
        {
            //--1
            pos1Dff = pos1Dff.Replace("|", "");
            var breite1Dms = pos1Dff.Substring(0, 7);
            var laenge1Dms = pos1Dff.Substring(7, 7);
            var breite1Dec = "";
            var laenge1Dec = "";
            Geodms2Geodec(breite1Dms, laenge1Dms, ref breite1Dec, ref laenge1Dec);

            //--2
            pos2Dff = pos2Dff.Replace("|", "");
            var breite2Dms = pos2Dff.Substring(0, 7);
            var laenge2Dms = pos2Dff.Substring(7, 7);
            var breite2Dec = "";
            var laenge2Dec = "";
            Geodms2Geodec(breite2Dms, laenge2Dms, ref breite2Dec, ref laenge2Dec);

            var newBreite = (double.Parse(breite1Dec) + double.Parse(breite2Dec))/2;
            var newLaenge = (double.Parse(laenge1Dec) + double.Parse(laenge2Dec))/2;

            var newDmsBreite = "";
            var newDmsLaenge = "";

            Geodec2Geodms(newBreite.ToString(), newLaenge.ToString(), ref newDmsBreite,
                ref newDmsLaenge);

            if (!newDmsBreite.StartsWith("-"))
            {
                while (newDmsBreite.Length < 7)
                {
                    newDmsBreite = "0" + newDmsBreite;
                }
            }
            else
            {
                newDmsBreite = newDmsBreite.Substring(1);
                while (newDmsBreite.Length < 7)
                {
                    newDmsBreite = "0" + newDmsBreite;
                }
                newDmsBreite = "-" + newDmsBreite;
            }

            if (!newDmsLaenge.StartsWith("-"))
            {
                while (newDmsLaenge.Length < 7)
                {
                    newDmsLaenge = "0" + newDmsLaenge;
                }
            }
            else
            {
                newDmsLaenge = newDmsLaenge.Substring(1);
                while (newDmsLaenge.Length < 7)
                {
                    newDmsLaenge = "0" + newDmsLaenge;
                }
                newDmsLaenge = "-" + newDmsLaenge;
            }

            var position = "";
            try
            {
                position = newDmsBreite.Substring(0, 2) + "|" + newDmsBreite.Substring(2, 2) + "|" +
                           newDmsBreite.Substring(4, 3) + "|";
                position += newDmsLaenge.Substring(0, 2) + "|" + newDmsLaenge.Substring(2, 2) + "|" +
                            newDmsLaenge.Substring(4, 3);
            }
            catch
            {
            }
            return position.Trim();
        }

        public static int CheckIfDeviceIsInGeoFenceDec(string location, string fence, int toleranz)
        {
            var inFenceMarker = false;
            try
            {
                //Das hintere Semikolon abschneiden
                if (fence.EndsWith(";"))
                {
                    fence = fence.Substring(0, fence.Length - 1);
                }

                var dffFormatLastLocation = location.Replace("|", "");
                var breiteLldms = dffFormatLastLocation.Substring(0, 7);
                var laengeLldms = dffFormatLastLocation.Substring(7, 7);
                var breiteLldec = "";
                var laengeLldec = "";
                Geodms2Geodec(breiteLldms, laengeLldms, ref breiteLldec, ref laengeLldec);

                var fences = fence.Split(';');
                for (var z = 0; z < fences.Length; z++)
                {
                    if (fences[z].Length == 0)
                    {
                        continue; //Falls ein Markt keine Position hat.
                    }
                    var rad = int.Parse(fences[z].Substring(0, fences[z].IndexOf("@", StringComparison.Ordinal)));
                    var pos = fences[z].Substring(fences[z].IndexOf("@", StringComparison.Ordinal) + 1);
                    var breiteFenceDec = pos.Substring(0, pos.IndexOf("|", StringComparison.Ordinal));
                    var laengeFenceDec = pos.Substring(pos.IndexOf("|", StringComparison.Ordinal) + 1);

                    var dis = GetDistanceDec(breiteLldec, laengeLldec, breiteFenceDec,
                        laengeFenceDec);
                    if (dis <= (rad + toleranz + 10))
                    {
                        inFenceMarker = true;
                    }
                }
            }
            catch
            {
                return -1;
            }
            if (inFenceMarker)
            {
                return 1;
            }
            return 0;
        }

        private string DecToDffPositionString(double decLatitude, double decLongitude)
        {
            var position = "";
            var dmsLat = "";
            var dmsLon = "";
            double dmsLatitude;
            double dmsLongitude;
            var decLat = decLatitude.ToString().Replace(".", ",");
            var decLon = decLongitude.ToString().Replace(".", ",");

            while (decLat.Substring(decLat.IndexOf(",", StringComparison.Ordinal) + 1).Length < 5) decLat = decLat + "0";
            while (decLon.Substring(decLon.IndexOf(",", StringComparison.Ordinal) + 1).Length < 5) decLon = decLon + "0";

            decLat = decLat.Substring(0, decLat.IndexOf(",", StringComparison.Ordinal)) + "," + decLat.Substring(decLat.IndexOf(",", StringComparison.Ordinal) + 1, 5);
            decLon = decLon.Substring(0, decLon.IndexOf(",", StringComparison.Ordinal)) + "," + decLon.Substring(decLon.IndexOf(",", StringComparison.Ordinal) + 1, 5);
            decLat = decLat.Replace(",", "");
            decLon = decLon.Replace(",", "");
            Geodec2Geodms(decLat, decLon, ref dmsLat, ref dmsLon);
            dmsLatitude = double.Parse(dmsLat)/100000;
            dmsLongitude = double.Parse(dmsLon)/100000;

            if (dmsLatitude != 0 & dmsLongitude != 0)
            {
                IFormatProvider format = new CultureInfo("en-US", true);
                var breiteNeg = false;
                var laengeNeg = false;
                var breite = dmsLatitude.ToString(format);
                var laenge = dmsLongitude.ToString(format);
                if (breite.IndexOf(".", StringComparison.Ordinal) == -1) breite = breite + ".00000";
                if (laenge.IndexOf(".", StringComparison.Ordinal) == -1) laenge = laenge + ".00000";

                if (breite.StartsWith("-"))
                {
                    breiteNeg = true;
                    breite = breite.Substring(1);
                }
                if (laenge.StartsWith("-"))
                {
                    laengeNeg = true;
                    laenge = laenge.Substring(1);
                }

                while (breite.Substring(breite.IndexOf(".", StringComparison.Ordinal) + 1).Length < 5) breite = breite + "0";
                while (laenge.Substring(laenge.IndexOf(".", StringComparison.Ordinal) + 1).Length < 5) laenge = laenge + "0";
                while (breite.Substring(0, breite.IndexOf(".", StringComparison.Ordinal)).Length < 2) breite = "0" + breite;
                while (laenge.Substring(0, laenge.IndexOf(".", StringComparison.Ordinal)).Length < 2) laenge = "0" + laenge;
                if (laengeNeg) laenge = "-" + laenge;
                if (breiteNeg) breite = "-" + breite;

                breite = breite.Replace(".", "");
                laenge = laenge.Replace(".", "");


                if (!laenge.StartsWith("-"))
                    position = laenge.Substring(0, 2) + "|" + laenge.Substring(2, 2) + "|" + laenge.Substring(4, 3) +
                               "|";
                else
                    position = laenge.Substring(0, 3) + "|" + laenge.Substring(3, 2) + "|" + laenge.Substring(5, 3) +
                               "|";

                if (!breite.StartsWith("-"))
                    position += breite.Substring(0, 2) + "|" + breite.Substring(2, 2) + "|" + breite.Substring(4, 3) +
                                "||" + 0 + "|" + 0;
                else
                    position += breite.Substring(0, 3) + "|" + breite.Substring(3, 2) + "|" + breite.Substring(5, 3) +
                                "||" + 0 + "|" + 0;
            }
            return position;
        }

        #endregion
    }
}