using System;

namespace urakawa.media.timing
{
    /// <summary>
    /// TimeDelta is the difference between two timestamps (<see cref="Time"/>s)
    /// </summary>
    public class TimeDelta
    {
        /// <summary>
        /// Gets a <see cref="TimeDelta"/> representing zero (00:00:00.000000)
        /// </summary>
        public static TimeDelta Zero
        {
            get { return new TimeDelta(); }
        }

        /// <summary>
        /// Gets the largest possible value of <see cref="TimeDelta"/>
        /// </summary>
        public static TimeDelta MaxValue
        {
            get { return new TimeDelta(TimeSpan.MaxValue); }
        }

        private TimeSpan mTimeDelta;

        /// <summary>
        /// Default constructor, initializes the difference to 0
        /// </summary>
        public TimeDelta()
        {
            mTimeDelta = TimeSpan.Zero;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public TimeDelta(TimeDelta other) : this()
        {
            if (other != null)
            {
                AddTimeDelta(other);
            }
        }

        /// <summary>
        /// Creates a copy of the <see cref="TimeDelta"/>
        /// </summary>
        /// <returns></returns>
        public TimeDelta Copy()
        {
            return new TimeDelta(this);
        }

        /// <summary>
        /// Constructor setting the difference to a given number of milliseconds
        /// </summary>
        /// <param name="val">The given number of milliseconds, 
        /// must not be negative</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown if <paramref localName="val"/> is negative
        /// </exception>
        public TimeDelta(long val)
        {
            TimeDeltaAsMillisecondLong = val;
        }

        /// <summary>
        /// Constructor setting the difference to a given millisecond value
        /// </summary>
        /// <param name="val">The millisecond valud</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown if <paramref localName="val"/> is negative
        /// </exception>
        public TimeDelta(double val)
        {
            TimeDeltaAsMillisecondFloat = val;
        }

        /// <summary>
        /// Constructor setting the difference to a given <see cref="TimeSpan"/> value
        /// </summary>
        /// <param name="val">The given <see cref="TimeSpan"/> value</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown if <paramref localName="val"/> is negative
        /// </exception>
        public TimeDelta(TimeSpan val)
        {
            TimeDeltaAsTimeSpan = val;
        }

        /// <summary>
        /// Gets the <see cref="TimeDelta"/> in milliseconds
        /// </summary>
        /// <returns>The number of milliseconds equivalent to the <see cref="TimeDelta"/>
        /// </returns>
        public long TimeDeltaAsMillisecondLong
        {
            get { return mTimeDelta.Ticks/TimeSpan.TicksPerMillisecond; }
            set { TimeDeltaAsTimeSpan = TimeSpan.FromTicks(value*TimeSpan.TicksPerMillisecond); }
        }

        /// <summary>
        /// Gets the <see cref="TimeDelta"/> as a <see cref="TimeSpan"/>
        /// </summary>
        /// <returns>The <see cref="TimeSpan"/></returns>
        public TimeSpan TimeDeltaAsTimeSpan
        {
            get { return mTimeDelta; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "Time Delta can not be negative");
                }
                mTimeDelta = value;
            }
        }

        /// <summary>
        /// Gets <c>this</c> as a millisecond floating point value
        /// /// </summary>
        /// <returns>The millisecond value</returns>
        public double TimeDeltaAsMillisecondFloat
        {
            get { return ((double) mTimeDelta.Ticks)/((double) TimeSpan.TicksPerMillisecond); }
            set { TimeDeltaAsTimeSpan = TimeSpan.FromTicks((long) (value*(double) TimeSpan.TicksPerMillisecond)); }
        }

        /// <summary>
        /// Adds another <see cref="TimeDelta"/> to <c>this</c>
        /// </summary>
        /// <param name="other">The other <see cref="TimeDelta"/></param>
        public TimeDelta AddTimeDelta(TimeDelta other)
        {
            return new TimeDelta(mTimeDelta += other.TimeDeltaAsTimeSpan);
        }

        /// <summary>
        /// Determines is <c>this</c> is less than a given other <see cref="TimeDelta"/>.
        /// </summary>
        /// <param name="other">The other TimeDelta</param>
        /// <returns>
        /// <c>true</c> if <c>this</c> is less than <paramref localName="other"/>, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="other"/> is <c>null</c>
        /// </exception>
        public bool IsLessThan(TimeDelta other)
        {
            if (other == null)
                throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
            return (mTimeDelta < other.mTimeDelta);
        }

        /// <summary>
        /// Determines is <c>this</c> is greater than a given other <see cref="TimeDelta"/>.
        /// </summary>
        /// <param name="other">The other TimeDelta</param>
        /// <returns>
        /// <c>true</c> if <c>this</c> is greater than <paramref localName="other"/>, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="other"/> is <c>null</c>
        /// </exception>
        public bool IsGreaterThan(TimeDelta other)
        {
            if (other == null)
                throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
            return other.IsLessThan(this);
        }

        /// <summary>
        /// Determines is <c>this</c> is equal to a given other <see cref="TimeDelta"/>.
        /// </summary>
        /// <param name="other">The other TimeDelta</param>
        /// <returns>
        /// <c>true</c> if <c>this</c> is equal to <paramref localName="other"/>, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="other"/> is <c>null</c>
        /// </exception>
        public bool IsEqualTo(TimeDelta other)
        {
            if (other == null)
                throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
            return (!IsLessThan(other)) && (!IsGreaterThan(other));
        }

        /// <summary>
        /// Gets a textual representation of the <see cref="TimeDelta"/>
        /// </summary>
        /// <returns>The testual representation</returns>
        public override string ToString()
        {
            return mTimeDelta.ToString();
        }
    }
}