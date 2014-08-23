using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratum.WorldEngine
{
    public static class Astronomy
    {
        #region [ Julian Date Conversion ]

        public static bool isJulianDate(int year, int month, int day)
        {
            // All dates prior to 1582 are in the Julian calendar
            if (year < 1582)
                return true;
            // All dates after 1582 are in the Gregorian calendar
            else if (year > 1582)
                return false;
            else
            {
                // If 1582, check before October 4 (Julian) or after October 15 (Gregorian)
                if (month < 10)
                    return true;
                else if (month > 10)
                    return false;
                else
                {
                    if (day < 5)
                        return true;
                    else if (day > 14)
                        return false;
                    else
                        // Any date in the range 10/5/1582 to 10/14/1582 is invalid 
                        throw new ArgumentOutOfRangeException(
                            "This date is not valid as it does not exist in either the Julian or the Gregorian calendars.");
                }
            }
        }

        static private double DateToJD(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            // Determine correct calendar based on date
            bool JulianCalendar = isJulianDate(year, month, day);

            int M = month > 2 ? month : month + 12;
            int Y = month > 2 ? year : year - 1;
            double D = day + hour / 24.0 + minute / 1440.0 + (second + millisecond * 1000) / 86400.0;
            int B = JulianCalendar ? 0 : 2 - Y / 100 + Y / 100 / 4;

            return (int)(365.25 * (Y + 4716)) + (int)(30.6001 * (M + 1)) + D + B - 1524.5;
        }

        static public double JD(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            return DateToJD(year, month, day, hour, minute, second, millisecond);
        }


        static public double JulianDate(DateTime date)
        {
            return DateToJD(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        public static DateTime JulianToDateTime(double julianDate)
        {
            DateTime date;
            double dblA, dblB, dblC, dblD, dblE, dblF;
            double dblZ, dblW, dblX;
            int day, month, year;
            try
            {
                dblZ = Math.Floor(julianDate + 0.5);
                dblW = Math.Floor((dblZ - 1867216.25) / 36524.25);
                dblX = Math.Floor(dblW / 4);
                dblA = dblZ + 1 + dblW - dblX;
                dblB = dblA + 1524;
                dblC = Math.Floor((dblB - 122.1) / 365.25);
                dblD = Math.Floor(365.25 * dblC);
                dblE = Math.Floor((dblB - dblD) / 30.6001);
                dblF = Math.Floor(30.6001 * dblE);
                day = Convert.ToInt32(dblB - dblD - dblF);
                if (dblE > 13)
                {
                    month = Convert.ToInt32(dblE - 13);
                }
                else
                {
                    month = Convert.ToInt32(dblE - 1);
                }
                if ((month == 1) || (month == 2))
                {
                    year = Convert.ToInt32(dblC - 4715);
                }
                else
                {
                    year = Convert.ToInt32(dblC - 4716);
                }
                date = new DateTime(year, month, day);
                return date;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                date = new DateTime(0);
            }
            catch (Exception ex)
            {
                date = new DateTime(0);
            }
            return date;
        } 

        #endregion
    }
}
