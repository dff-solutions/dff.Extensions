// dff solutions GmbH Göttingen, Germany - www.dff-solutions.de / info@dff-solutions.de

#region

using System;
using System.Globalization;

#endregion

namespace dff.Extensions.Gps
{
    public class DffGpsPosition
    {
        private double _decLatitude;

        private double _decLongitude;

        private double _dmsLatitude;

        private double _dmsLongitude;

        private int _mrcX;

        private int _mrcY;
        private Exception _ex;

        public DffGpsPosition(string dffPositionFormatString)
        {
            if (string.IsNullOrEmpty(dffPositionFormatString)) return;

            //Wenn es sich um mehrere Fences handelt nur den ersten nehmen.
            if (dffPositionFormatString.Contains(";"))
                dffPositionFormatString = dffPositionFormatString.Split(";".ToCharArray())[0];
            else if (dffPositionFormatString.Contains(","))
                dffPositionFormatString = dffPositionFormatString.Split(",".ToCharArray())[0];

            if (string.IsNullOrEmpty(dffPositionFormatString)) return;

            //Wenn es sich um einen Fence handelt, den Radius entfernen
            if (dffPositionFormatString.Contains("@"))
            {
                var temp = dffPositionFormatString.Substring(dffPositionFormatString.IndexOf("@", StringComparison.Ordinal) + 1);
                var radiusString = dffPositionFormatString.Substring(0, dffPositionFormatString.IndexOf("@", StringComparison.Ordinal));
                dffPositionFormatString = temp;
                try
                {
                    FenceRadiusMeter = int.Parse(radiusString);
                }
                catch
                {
                }
            }

            DffGpsTools.DffPositionStringToMercator(dffPositionFormatString, ref _mrcX, ref _mrcY);
            DffGpsTools.MercatorToGeodecimal(_mrcX, _mrcY, ref _decLongitude, ref _decLatitude);

            if (!dffPositionFormatString.Contains("||")) return;
            try
            {
                var speedDirInfo = dffPositionFormatString.Substring(dffPositionFormatString.IndexOf("||", StringComparison.Ordinal) + 2);
                var infos = speedDirInfo.Split("|".ToCharArray());
                if (infos.Length > 0 && !string.IsNullOrEmpty(infos[0])) Speed = double.Parse(infos[0]);
                if (infos.Length > 1 && !string.IsNullOrEmpty(infos[1])) Heading = int.Parse(infos[1]);
            }
            catch
            {
            }
        }

        public DffGpsPosition(int mercatorX, int mercatorY)
        {
            MercatorX = mercatorX;
            MercatorY = mercatorY;
            DffGpsTools.MercatorToGeodecimal(_mrcX, _mrcY, ref _decLongitude, ref _decLatitude);
        }

        public DffGpsPosition(double decx, double decy)
        {
            DecLatitude = decy;
            DecLongitude = decx;
        }

        public DffGpsPosition()
        {
        }

        public int FenceRadiusMeter { get; set; }

        public DateTime PositionTimestamp { get; set; }

        public double DecLongitude
        {
            get
            {
                if (_decLongitude != 0) return _decLongitude;
                if (_dmsLongitude != 0)
                {
                    CalcualteDecValues();
                    return _decLongitude;
                }
                return 0;
            }
            set { _decLongitude = value; }
        }

        public double DecLatitude
        {
            get
            {
                if (_decLatitude != 0) return _decLatitude;
                if (_dmsLatitude != 0)
                {
                    CalcualteDecValues();
                    return _decLatitude;
                }
                return _decLatitude;
            }
            set { _decLatitude = value; }
        }

        public double DmsLongitude
        {
            get { return _dmsLongitude; }
            set { _dmsLongitude = value; }
        }

        public double DmsLatitude
        {
            get { return _dmsLatitude; }
            set { _dmsLatitude = value; }
        }

        public int MercatorX
        {
            get
            {
                if (_mrcX != 0) return _mrcX;
                CalculateMercatorValues();
                return _mrcX;
            }
            set { _mrcX = value; }
        }

        public int MercatorY
        {
            get
            {
                if (_mrcY != 0) return _mrcY;
                CalculateMercatorValues();
                return _mrcY;
            }
            set { _mrcY = value; }
        }

        public double Speed { get; set; }

        public double Heading { get; set; }

        public double Height { get; set; }

        public int Sats { get; set; }

        public string DffPositionString
        {
            get
            {
                var position = "";
                //Zusammensetzen des Positionsstrings im dff-Format                    
                try
                {
                    if ((DmsLatitude == 0 & DmsLongitude == 0) & (MercatorX != 0 & MercatorY != 0))
                    {
                        double tempDecLat = 0;
                        double tempDecLon = 0;
                        DffGpsTools.MercatorToGeodecimal(MercatorX, MercatorY, ref tempDecLon, ref tempDecLat);
                        DecLatitude = tempDecLat;
                        DecLongitude = tempDecLon;
                    }
                    if ((DmsLatitude == 0 & DmsLongitude == 0) &
                        (DecLatitude != 0 & DecLongitude != 0))
                    {
                        var dmsLat = "";
                        var dmsLon = "";
                        var decLat = DecLatitude.ToString().Replace(".", ",");
                        var decLon = DecLongitude.ToString().Replace(".", ",");

                        while (decLat.Substring(decLat.IndexOf(",", StringComparison.Ordinal) + 1).Length < 5) decLat = decLat + "0";
                        while (decLon.Substring(decLon.IndexOf(",", StringComparison.Ordinal) + 1).Length < 5) decLon = decLon + "0";

                        decLat = decLat.Substring(0, decLat.IndexOf(",", StringComparison.Ordinal)) + "," +
                                 decLat.Substring(decLat.IndexOf(",", StringComparison.Ordinal) + 1, 5);
                        decLon = decLon.Substring(0, decLon.IndexOf(",", StringComparison.Ordinal)) + "," +
                                 decLon.Substring(decLon.IndexOf(",", StringComparison.Ordinal) + 1, 5);
                        decLat = decLat.Replace(",", "");
                        decLon = decLon.Replace(",", "");
                        DffGpsTools.Geodec2Geodms(decLat, decLon, ref dmsLat, ref dmsLon);
                        DmsLatitude = double.Parse(dmsLat)/100000;
                        DmsLongitude = double.Parse(dmsLon)/100000;
                    }
                    if (DmsLatitude != 0 & DmsLongitude != 0)
                    {
                        IFormatProvider format = new CultureInfo("en-US", true);
                        var breiteNeg = false;
                        var laengeNeg = false;
                        var breite = DmsLatitude.ToString(format);
                        var laenge = DmsLongitude.ToString(format);
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
                            position = laenge.Substring(0, 2) + "|" + laenge.Substring(2, 2) + "|" +
                                       laenge.Substring(4, 3) + "|";
                        else
                            position = laenge.Substring(0, 3) + "|" + laenge.Substring(3, 2) + "|" +
                                       laenge.Substring(5, 3) + "|";

                        if (!breite.StartsWith("-"))
                            position += breite.Substring(0, 2) + "|" + breite.Substring(2, 2) + "|" +
                                        breite.Substring(4, 3) + "||" + Speed + "|" + Heading;
                        else
                            position += breite.Substring(0, 3) + "|" + breite.Substring(3, 2) + "|" +
                                        breite.Substring(5, 3) + "||" + Speed + "|" + Heading;
                    }
                }
                catch (Exception e)
                {
                    _ex = e;
                    //dffToolboxForPPC_V3.Debug.dffDebug.internalDebugMessage(e, "Fehler beim Zusammensetzen der Position.", 87, dffToolboxForPPC_V3.Debug.dffDebug.debugType.Error);
                }
                position = position.Trim();
                return position;
            }
        }

        public string CosPositionString
        {
            get
            {
                IFormatProvider format = new CultureInfo("en-US", true);
                IFormatProvider formatDe = new CultureInfo("de-DE", true);

                if ((DmsLatitude == 0 & DmsLongitude == 0) &
                    (DecLatitude != 0 & DecLongitude != 0))
                {
                    var dmsLat = "";
                    var dmsLon = "";
                    var decLat = DecLatitude.ToString(formatDe);
                    var decLon = DecLongitude.ToString(formatDe);

                    while (decLat.Substring(decLat.IndexOf(",") + 1).Length < 5) decLat = decLat + "0";
                    while (decLon.Substring(decLon.IndexOf(",") + 1).Length < 5) decLon = decLon + "0";

                    decLat = decLat.Substring(0, decLat.IndexOf(",")) + "," +
                             decLat.Substring(decLat.IndexOf(",") + 1, 5);
                    decLon = decLon.Substring(0, decLon.IndexOf(",")) + "," +
                             decLon.Substring(decLon.IndexOf(",") + 1, 5);
                    decLat = decLat.Replace(",", "");
                    decLon = decLon.Replace(",", "");
                    DffGpsTools.Geodec2Geodms(decLat, decLon, ref dmsLat, ref dmsLon);
                    DmsLatitude = double.Parse(dmsLat)/100000;
                    DmsLongitude = double.Parse(dmsLon)/100000;
                }

                var breite = string.Format(formatDe, "{0:0.00}", (DmsLatitude*100));
                var laenge = string.Format(formatDe, "{0:0.00}", (DmsLongitude*100));

                var breiteSp = breite.Split(',');
                var laengeSp = laenge.Split(',');

                var breiteVorn = breiteSp[0].Length == 4 ? "0" + breiteSp[0] : "00" + breiteSp[0];
                var laengeVorn = laengeSp[0].Length == 4 ? "0" + laengeSp[0] : "00" + laengeSp[0];

                var breiteHinten = ((decimal.Parse(breiteSp[1])/60)*100).ToString(formatDe);
                breiteHinten = breiteHinten.Replace(",", "");
                breiteHinten = breiteHinten.Length >= 10 ? breiteHinten.Substring(0, 9) : breiteHinten;

                for (var i = breiteHinten.Length; i < 10; i++)
                {
                    breiteHinten = breiteHinten + "0";
                }

                var laengeHinten = ((decimal.Parse(laengeSp[1])/60)*100).ToString(formatDe);
                laengeHinten = laengeHinten.Replace(",", "");
                laengeHinten = laengeHinten.Length >= 10 ? laengeHinten.Substring(0, 9) : laengeHinten;

                for (var i = laengeHinten.Length; i < 10; i++)
                {
                    laengeHinten = laengeHinten + "0";
                }

                var newBreite = breiteVorn + "." + breiteHinten;
                var newLaenge = laengeVorn + "." + laengeHinten;

                var north = DecLatitude > 0 ? "N" : "S";
                var east = DecLongitude > 0 ? "E" : "W";

                var knoten = DffGpsTools.Kmh2Knoten(Speed).ToString(format);

                var cosString = "A#" + newBreite + "#" + north + "#" + newLaenge + "#" + east + "#" + knoten + "#" +
                                Heading;
                return cosString;
            }
        }

        private void CalcualteDecValues()
        {
            IFormatProvider format = new CultureInfo("en-US", true);
            var decLatString = "";
            var decLongString = "";

            var latNeg = false;
            var longNeg = false;
            var dmsLatString = _dmsLatitude.ToString(format);
            var dmsLongString = _dmsLongitude.ToString(format);
            if (dmsLatString.IndexOf(".") == -1) dmsLatString = dmsLatString + ".00000";
            if (dmsLongString.IndexOf(".") == -1) dmsLongString = dmsLongString + ".00000";

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

            while (dmsLatString.Substring(dmsLatString.IndexOf(".") + 1).Length < 5) dmsLatString = dmsLatString + "0";
            while (dmsLongString.Substring(dmsLongString.IndexOf(".") + 1).Length < 5)
                dmsLongString = dmsLongString + "0";
            while (dmsLatString.Length < 8) dmsLatString = "0" + dmsLatString;
            while (dmsLongString.Length < 8) dmsLongString = "0" + dmsLongString;
            if (latNeg) dmsLatString = "-" + dmsLatString;
            if (longNeg) dmsLongString = "-" + dmsLongString;

            DffGpsTools.Geodms2Geodec(dmsLatString.Replace(".", ""), dmsLongString.Replace(".", ""), ref decLatString,
                ref decLongString);
            _decLongitude = double.Parse(decLongString)/100000;
            _decLatitude = double.Parse(decLatString)/100000;
        }

        private void CalculateMercatorValues()
        {
            if ((_decLongitude == 0 & _decLatitude == 0) & (DmsLatitude != 0 & DmsLongitude != 0))
            {
                CalcualteDecValues();
            }
            var newMercX = 0;
            var newMercY = 0;
            DffGpsTools.GeodecimalToMercator(_decLongitude, _decLatitude, ref newMercX, ref newMercY);
            _mrcX = newMercX;
            _mrcY = newMercY;
        }

        public TimeSpan GetAgeOfPosition()
        {
            return new TimeSpan(DateTime.Now.Ticks - PositionTimestamp.Ticks);
        }
    }
}