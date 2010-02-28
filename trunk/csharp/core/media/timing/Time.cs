using System;
using System.Diagnostics;

namespace urakawa.media.timing
{
    /// <summary>
    /// The Time object represents a timestamp.  
    /// </summary>
    public class Time
    {
        /// <summary>
        /// Gets a <see cref="Time"/> representing 00:00:00.000
        /// </summary>
        public static Time Zero
        {
            get { return new Time(); }
        }

        /// <summary>
        /// Gets the largest possible value for <see cref="Time"/>
        /// </summary>
        public static Time MaxValue
        {
            get { return new Time(TimeSpan.MaxValue); }
        }

        /// <summary>
        /// Gets the smallest possible value for <see cref="Time"/>
        /// </summary>
        public static Time MinValue
        {
            get { return new Time(TimeSpan.MinValue); }
        }

        private TimeSpan mTime;

        /// <summary>
        /// Default constructor initializing the instance to 0
        /// </summary>
        public Time()
        {
            mTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Constructor initializing the instance with a given number of milliseconds
        /// </summary>
        /// <param name="val">The given number of milliseconds</param>
        public Time(long val)
        {
            TimeAsMillisecondLong = val;
        }

        /// <summary>
        /// Constructor initializing the instance with a given number of milliseconds
        /// </summary>
        /// <param name="val">The given number of milliseconds</param>
        public Time(double val)
        {
            TimeAsMillisecondDouble = val;
        }

        /// <summary>
        /// Constructor initializing the instance with a given <see cref="TimeSpan"/>
        /// value
        /// </summary>
        /// <param name="val">The given <see cref="TimeSpan"/> value</param>
        public Time(TimeSpan val)
        {
            TimeAsTimeSpan = val;
        }

        /// <summary>
        /// Constructor initializing the instance with a given <see cref="string"/>
        /// representation of time.
        /// <see cref="ToString"/> member method of a description of the format 
        /// of the string representation.
        /// </summary>
        /// <param name="val">The <see cref="string"/> representation</param>
        public Time(string val)
        {
            TimeAsTimeSpan = Time.Parse(val).mTime;
        }

        /// <summary>
        /// Returns the <see cref="TimeSpan"/> equivalent of the instance
        /// </summary>
        /// <returns>The <see cref="TimeSpan"/> equivalent</returns>
        public TimeSpan TimeAsTimeSpan
        {
            get { return mTime; }
            set { mTime = value; }
        }

        /// <summary>
        /// Gets a string representation of the <see cref="Time"/>
        /// </summary>
        /// <returns>The string representation</returns>
        /// <remarks>
        /// The format of the string representation [-][d.]hh:mm:ss[.f],
        /// where d is a number of days, hh is two-digit hours between 00 and 23,
        /// mm is two-digit minutes between 00 and 59, 
        /// ss is two-digit seconds between 00 and 59 
        /// and where f is the second fraction with between 1 and 7 digits
        /// </remarks>
        public override string ToString()
        {
            return mTime.ToString();
        }

        /// <summary>
        /// Parses a string representation of a <see cref="Time"/>. 
        /// See <see cref="ToString"/> for a description of the format of the string representation
        /// </summary>
        /// <param name="stringRepresentation">The string representation</param>
        /// <returns>The parsed <see cref="Time"/></returns>
        /// <exception cref="exception.TimeStringRepresentationIsInvalidException">
        /// Thrown then the given string representation is not valid
        /// </exception>
        public static Time Parse(string stringRepresentation)
        {
            if (stringRepresentation == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not parse a null string");
            }
            if (stringRepresentation == String.Empty)
            {
                throw new exception.MethodParameterIsEmptyStringException(
                    "Can not parse an empty string");
            }

            return ParseTimeString(stringRepresentation);
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
            do
            {
                value += weight * (current - '0');
                weight *= 0.1;

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

        private static double parseClockValue(string str)
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
            return offset;
        }


        // See http://www.w3.org/TR/SMIL3/smil-timing.html#Timing-ClockValueSyntax
        // See http://svn.apache.org/viewvc/xmlgraphics/batik/trunk/sources/org/apache/batik/parser/TimingParser.java?view=co
        // See http://svn.apache.org/viewvc/xmlgraphics/batik/trunk/sources/org/apache/batik/parser/AbstractParser.java?view=co
        public static Time ParseTimeString(string str)
        {
            try
            {
                double s = parseClockValue(str);
                return new Time(TimeSpan.FromSeconds(s));
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                return new Time(TimeSpan.Parse(str));
            }
        }

        #region Time Members

        /// <summary>
        /// Determines if the instance represents a negative time value
        /// </summary>
        /// <returns><c>true</c> if negative, <c>false</c> else</returns>
        public bool IsNegativeTimeOffset
        {
            get { return (mTime < TimeSpan.Zero); }
        }

        /// <summary>
        /// Creates a copy of the <see cref="Time"/> instance
        /// </summary>
        /// <returns>The copy</returns>
        public Time Copy()
        {
            return new Time(mTime);
        }

        /// <summary>
        /// Gets the (absolute) <see cref="TimeDelta"/> between a given <see cref="Time"/> and <c>this</c>,
        /// that is <c>this-<paramref localName="t"/></c>
        /// </summary>
        /// <param name="t">The given <see cref="Time"/></param>
        /// <returns>
        /// The difference as an <see cref="TimeDelta"/>
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="t"/> is <c>null</c>
        /// </exception>
        public TimeDelta GetTimeDelta(Time t)
        {
            if (t == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The time with which to compare can not be null");
            }
            if (t is Time)
            {
                Time otherTime = (Time)t;
                if (mTime > otherTime.mTime)
                {
                    return new TimeDelta(mTime.Subtract(otherTime.mTime));
                }
                else
                {
                    return new TimeDelta(otherTime.mTime.Subtract(mTime));
                }
            }
            else
            {
                double msDiff = TimeAsMillisecondDouble - t.TimeAsMillisecondDouble;
                if (msDiff < 0) msDiff = -msDiff;
                return new TimeDelta(msDiff);
            }
        }

        /// <summary>
        /// Gets or sets the best approximation of the <see cref="Time"/> in whole milliseconds
        /// </summary>
        /// <returns>The number of milliseconds</returns>
        public long TimeAsMillisecondLong
        {
            get { return (long)Math.Round(TimeAsMillisecondDouble); }
            set { TimeAsTimeSpan = TimeSpan.FromTicks(value * TimeSpan.TicksPerMillisecond); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Time"/> as a floating point millisecond value
        /// </summary>
        /// <returns>The foaling point millisecond value</returns>
        public double TimeAsMillisecondDouble
        {
            get { return ((double)mTime.Ticks) / ((double)TimeSpan.TicksPerMillisecond); }
            set { TimeAsTimeSpan = TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond)); }
        }


        /// <summary>
        /// Adds another <see cref="Time"/> to the current <see cref="Time"/>
        /// </summary>
        /// <param name="other">The other <see cref="Time"/></param>
        public Time AddTime(Time other)
        {
            return new Time(mTime + other.TimeAsTimeSpan);
        }

        /// <summary>
        /// Adds a <see cref="TimeDelta"/> to the current <see cref="Time"/>
        /// </summary>
        /// <param name="other">The <see cref="TimeDelta"/> to add</param>
        public Time AddTimeDelta(TimeDelta other)
        {
            return new Time(mTime + other.TimeDeltaAsTimeSpan);
        }

        /// <summary>
        /// Subtracts a <see cref="Time"/> from the current <see cref="Time"/>
        /// </summary>
        /// <param name="other">The <see cref="Time"/> to add</param>
        public Time SubtractTime(Time other)
        {
            return new Time(mTime - other.TimeAsTimeSpan);
        }

        /// <summary>
        /// Subtracts a <see cref="TimeDelta"/> from the current <see cref="Time"/>
        /// </summary>
        /// <param name="other">The <see cref="TimeDelta"/> to add</param>
        public Time SubtractTimeDelta(TimeDelta other)
        {
            return new Time(mTime - other.TimeDeltaAsTimeSpan);
        }


        /// <summary>
        /// Determines <c>this</c> is greater than a given other <see cref="Time"/>
        /// </summary>
        /// <param name="otherTime">The other <see cref="Time"/></param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="otherTime"/> is <c>null</c>
        /// </exception>
        public bool IsGreaterThan(Time otherTime)
        {
            if (otherTime == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not compare to a null Time");
            }
            bool res;
            if (otherTime is Time)
            {
                res = (mTime > ((Time)otherTime).mTime);
            }
            else
            {
                res = (TimeAsMillisecondDouble > otherTime.TimeAsMillisecondDouble);
            }
            return res;
        }


        /// <summary>
        /// Determines <c>this</c> is less than a given other <see cref="Time"/>
        /// </summary>
        /// <param name="otherTime">The other <see cref="Time"/></param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="otherTime"/> is <c>null</c>
        /// </exception>
        public bool IsLessThan(Time otherTime)
        {
            if (otherTime == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not compare to a null Time");
            }
            return otherTime.IsGreaterThan(this);
        }

        /// <summary>
        /// Determines <c>this</c> is equal to a given other <see cref="Time"/>
        /// </summary>
        /// <param name="otherTime">The other <see cref="Time"/></param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="otherTime"/> is <c>null</c>
        /// </exception>
        public bool IsEqualTo(Time otherTime)
        {
            if (IsGreaterThan(otherTime)) return false;
            if (otherTime.IsGreaterThan(this)) return false;
            return true;
        }

        /// <summary>
        /// Determines <c>this</c> is greater than or equal to a given other <see cref="Time"/>
        /// </summary>
        /// <param name="otherTime">The other <see cref="Time"/></param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="otherTime"/> is <c>null</c>
        /// </exception>
        public bool IsGreaterThanOrEqualTo(Time otherTime)
        {
            return !IsLessThan(otherTime);
        }

        /// <summary>
        /// Determines <c>this</c> is less than or equal to a given other <see cref="Time"/>
        /// </summary>
        /// <param name="otherTime">The other <see cref="Time"/></param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="otherTime"/> is <c>null</c>
        /// </exception>
        public bool IsLessThanOrEqualTo(Time otherTime)
        {
            if (otherTime == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not compare to a null Time");
            }
            return otherTime.IsGreaterThanOrEqualTo(this);
        }

        #endregion
    }
}