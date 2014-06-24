using System;
using System.Diagnostics;
using AudioLib;

namespace urakawa.media.timing
{
    // See http://www.w3.org/TR/SMIL3/smil-timing.html#Timing-ClockValueSyntax
    // See http://svn.apache.org/viewvc/xmlgraphics/batik/trunk/sources/org/apache/batik/parser/TimingParser.java?view=co
    // See http://svn.apache.org/viewvc/xmlgraphics/batik/trunk/sources/org/apache/batik/parser/AbstractParser.java?view=co
    public partial class Time
    {
        public override string ToString()
        {
            // seven significant decimal digits for the millisecond fractional part
            // Use Format_Standard() when exporting to DAISY fileset (clip-begin/end time values in SMIL files)
            return m_TimeSpan.ToString();
        }

        //private static string Format_Npt(TimeSpan time)
        //{
        //    double dTime = Math.Round(time.TotalSeconds, 3, MidpointRounding.ToEven);
        //    return "npt=" + dTime.ToString() + "s";
        //}

        public string Format_Standard()
        {
            return Format_Standard(m_TimeSpan);
        }

        public string Format_StandardExpanded()
        {
            return Format_StandardExpanded(m_TimeSpan);
        }

        public static string Format_StandardExpanded(TimeSpan time)
        {
            if (time.Days >= 1)
            {
                string[] timeStringArray = time.ToString().Split(':');
                string strTIME = time.TotalHours.ToString().Split('.')[0];
                for (int i = 1; i < timeStringArray.Length; i++)
                {
                    strTIME += ":" + timeStringArray[i];
                }
                return strTIME;
            }

            return time.ToString();
        }

        public static string Format_Standard(TimeSpan time)
        {
            if (time.CompareTo(TimeSpan.Zero) == 0)
            {
                return "0";
            }
            if (time.Hours != 0)
            {
                return string.Format(time.Hours > 99 ? "{0:000}" : "{0:00}" + ":{1:00}:{2:00}.{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            }
            if (time.Minutes != 0)
            {
                return string.Format("{0:00}:{1:00}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
            }
            return string.Format("{0:00}.{1:000}", time.Seconds, time.Milliseconds);
        }

        public string Format_H_MN_S_MS()
        {
            return Format_H_MN_S_MS(m_TimeSpan);
        }

        public static string Format_H_MN_S_MS(TimeSpan time)
        {
            if (time.CompareTo(TimeSpan.Zero) == 0)
            {
                return "0s";
            }
            return
                (time.Hours != 0 ? time.Hours + "h " : "") +
                (time.Minutes != 0 ? time.Minutes + "mn " : "") +
                (time.Seconds != 0 ? time.Seconds + "s " : "") +
                (time.Milliseconds != 0 ? time.Milliseconds + "ms" : "");
        }

        private static double ParseToMilliseconds(string stringRepresentation)
        {
            try
            {
                double timeMillisecondsDecimal = 1000.0 * parseClockValueToSeconds(stringRepresentation);

                return timeMillisecondsDecimal;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                double timeMillisecondsDecimal = TimeSpan.Parse(stringRepresentation).TotalMilliseconds;
                return timeMillisecondsDecimal;
            }
        }

        private static void checkDigit(char current, string str, int index)
        {
            if (current < '0' || current > '9')
            {
                throw new exception.TimeStringRepresentationIsInvalidException(
                    string.Format("The character '{0}' at position {1} in string \"{2}\" is not a valid digit !", current,
                                  index, str));
            }
        }

        private static void throwBadChar(char current, string str, int index)
        {
            throw new exception.TimeStringRepresentationIsInvalidException(
                      string.Format("The character '{0}' at position {1} in string \"{2}\" is not a valid character !", current,
                                    index, str));
        }

        private static char advanceChar(string str, ref int index)
        {
            return index == (str.Length - 1) ? '\0' : str[++index];
        }

        private static int parseDigits(string str, ref int index)
        {
            char current = str[index];

            checkDigit(current, str, index);

            int value = 0;
            do
            {
                value = value * 10 + (current - '0');

                current = advanceChar(str, ref index);
            } while (current >= '0' && current <= '9');

            return value;
        }

        private static double parseFraction(string str, ref int index)
        {
            char current = str[index];

            checkDigit(current, str, index);

            double value = 0;
            double weight = 0.1;
            int decimalPlace = 1;
            do
            {
                value += weight * (current - '0');
                weight = Math.Round(weight * 0.1, ++decimalPlace, MidpointRounding.AwayFromZero);

                current = advanceChar(str, ref index);

            } while (current >= '0' && current <= '9');

            return value;
        }

        private static double parseUnit(string str, ref int index)
        {
            char current = str[index];

            if (current == 'h')
            {
                current = advanceChar(str, ref index);
                return 3600;
            }
            else if (current == 'm')
            {
                current = advanceChar(str, ref index);
                if (current == 'i')
                {
                    current = advanceChar(str, ref index);
                    if (current != 'n')
                    {
                        throwBadChar(current, str, index);
                    }
                    current = advanceChar(str, ref index);
                    return 60;
                }
                else if (current == 's')
                {
                    current = advanceChar(str, ref index);
                    return 0.001;
                }
                else
                {
                    throwBadChar(current, str, index);
                }
            }
            else if (current == 's')
            {
                current = advanceChar(str, ref index);
            }
            return 1;
        }

        private static double parseClockValueToSeconds(string str)
        {
            int index = 0; // we scan the string from left to right

            int d1 = parseDigits(str, ref index);
            char current = str[index];

            double offset;
            if (current == ':')
            {
                current = advanceChar(str, ref index); // skip ':' separator

                int d2 = parseDigits(str, ref index);
                current = str[index];

                if (current == ':')
                {
                    current = advanceChar(str, ref index); // skip ':' separator

                    int d3 = parseDigits(str, ref index);
                    current = str[index];

                    offset = d1 * 3600 + d2 * 60 + d3;
                }
                else
                {
                    offset = d1 * 60 + d2;
                }
                if (current == '.')
                {
                    current = advanceChar(str, ref index); // skip '.' separator

                    offset += parseFraction(str, ref index);
                    current = str[index];
                }
            }
            else if (current == '.')
            {
                current = advanceChar(str, ref index); // skip '.' separator

                double val = parseFraction(str, ref index) + d1;
                current = str[index];

                offset = val * parseUnit(str, ref index);
                current = str[index];
            }
            else
            {
                offset = d1 * parseUnit(str, ref index);
                current = str[index];
            }
            // May be imprecise due to machine/compiler-dependent double rouding errors. i.e. 0.1 * 0.1 can be 0.0100000002 !!
            // Solution: on the caller side, use Math.Round(VALUE, DECIMAL PLACES, MIDPOINT STRATEGY)
            return offset;
        }
    }
}
