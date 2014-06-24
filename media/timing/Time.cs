using System;
using AudioLib;

namespace urakawa.media.timing
{
    public partial class Time
    {
        // 1 ==> milliseconds
        // 1000 ==> microseconds
        // 1.000.000 ==> nanoseconds
        public static readonly long TIME_UNIT = AudioLibPCMFormat.TIME_UNIT;

        public static Time Zero
        {
            get { return new Time(); }
        }

        public static Time MaxValue
        {
            get { return new Time(TimeSpan.MaxValue); }
        }

        public static Time MinValue
        {
            get { return new Time(TimeSpan.MinValue); }
        }

        private TimeSpan m_TimeSpan;

        public Time()
        {
            m_TimeSpan = TimeSpan.Zero;
        }

        public Time(long time, bool ticks)
        {
            if (!ticks)
            {
                m_TimeSpan = ConvertFromLocalUnitsToTimeSpan(time);
            }
            else
            {
                m_TimeSpan = new TimeSpan(time);
            }
        }

        public Time(long timeInLocalUnits)
            : this(timeInLocalUnits, false)
        {
        }

        public Time(TimeSpan timeSpan)
        {
            //AsTimeSpan = ...
            m_TimeSpan = new TimeSpan(timeSpan.Ticks);
        }

        public Time(Time time)
            : this(time.m_TimeSpan)
        {
            //AsTimeSpan = ...
            //m_TimeSpan = new TimeSpan(time.m_TimeSpan.Ticks);
        }

        public Time(string stringRepresentation)
        {
            double timeMillisecondsDecimal = ParseToMilliseconds(stringRepresentation);

            int decimalPlaces = TIME_UNIT == 1 ? 0 : TIME_UNIT == 1000 ? 3 : TIME_UNIT == 1000000 ? 6 : 7;
            DebugFix.Assert(decimalPlaces != 7);
            timeMillisecondsDecimal = Math.Round(timeMillisecondsDecimal, decimalPlaces, MidpointRounding.AwayFromZero);

#if DEBUG
            try
            {
                TimeSpan timeSpan = TimeSpan.Parse(stringRepresentation);
                DebugFix.Assert(Math.Abs(timeSpan.TotalMilliseconds - timeMillisecondsDecimal) <= AudioLibPCMFormat.MILLISECONDS_TOLERANCE);
            }
            catch (FormatException ex)
            {
                ; // we can safely ignore 
            }
#endif //DEBUG


            double timeAsLocalUnitsDecimal = timeMillisecondsDecimal * TIME_UNIT;
            long timeAsLocalUnitsIntegral = (long)(AudioLibPCMFormat.USE_ROUND_NOT_TRUNCATE ? Math.Round(timeAsLocalUnitsDecimal) : Math.Truncate(timeAsLocalUnitsDecimal));

#if DEBUG
            // checking whether we are loosing fractions (of localunits) greater than 0.1
            DebugFix.Assert(((int)Math.Truncate((timeAsLocalUnitsDecimal - timeAsLocalUnitsIntegral) * 10)) <= 1);
#endif //DEBUG

            AsLocalUnits = timeAsLocalUnitsIntegral;
        }

        public long AsLocalUnits
        {
            get
            {
                if (m_TimeSpan == TimeSpan.MaxValue)
                {
                    return long.MaxValue;
                }
                if (m_TimeSpan == TimeSpan.MinValue)
                {
                    return long.MinValue;
                }
                if (m_TimeSpan == TimeSpan.Zero)
                {
                    return 0;
                }

                // ROUND: Convert.ToInt64(value)
                // TRUNCATE: (long)value

                double timeMillisecondsDecimal = m_TimeSpan.Ticks / ((double)TimeSpan.TicksPerMillisecond);

                int decimalPlaces = TIME_UNIT == 1 ? 0 : TIME_UNIT == 1000 ? 3 : TIME_UNIT == 1000000 ? 6 : 7;
                DebugFix.Assert(decimalPlaces != 7);
                timeMillisecondsDecimal = Math.Round(timeMillisecondsDecimal, decimalPlaces, MidpointRounding.AwayFromZero);

                double timeAsLocalUnitsDecimal = timeMillisecondsDecimal * TIME_UNIT;

                long timeAsLocalUnitsIntegral = (long)(AudioLibPCMFormat.USE_ROUND_NOT_TRUNCATE ? Math.Round(timeAsLocalUnitsDecimal) : Math.Truncate(timeAsLocalUnitsDecimal));

                // checking whether we are loosing fractions of milliseconds
                //////DebugFix.Assert(timeAsLocalUnitsDecimal == (double)timeAsLocalUnitsIntegral);

                return timeAsLocalUnitsIntegral;
            }
            private set
            {
                m_TimeSpan = ConvertFromLocalUnitsToTimeSpan(value);
            }
        }

        public static TimeSpan ConvertFromLocalUnitsToTimeSpan(long timeInLocalUnits)
        {
            if (timeInLocalUnits == long.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            if (timeInLocalUnits == long.MinValue)
            {
                return TimeSpan.MinValue;
            }
            if (timeInLocalUnits == 0)
            {
                return TimeSpan.Zero;
            }

            double timeMillisecondsDecimal = (double)timeInLocalUnits / TIME_UNIT;

            int decimalPlaces = TIME_UNIT == 1 ? 0 : TIME_UNIT == 1000 ? 3 : TIME_UNIT == 1000000 ? 6 : 7;
            DebugFix.Assert(decimalPlaces != 7);
            timeMillisecondsDecimal = Math.Round(timeMillisecondsDecimal, decimalPlaces, MidpointRounding.AwayFromZero);

            double ticksDecimal = timeMillisecondsDecimal * TimeSpan.TicksPerMillisecond;
            long ticksIntegral =
                (long)
                (AudioLibPCMFormat.USE_ROUND_NOT_TRUNCATE ? Math.Round(ticksDecimal) : Math.Truncate(ticksDecimal));

            // checking whether we are loosing fractions of milliseconds
            /////DebugFix.Assert(ticksDecimal == (double)ticksIntegral);

            return TimeSpan.FromTicks(ticksIntegral);
        }

        //public TimeSpan AsTimeSpan
        //{
        //    get { return new TimeSpan(m_TimeSpan.Ticks); }
        //    private set { m_TimeSpan = new TimeSpan(value.Ticks); }
        //}

        public double AsMilliseconds
        {
            get { return m_TimeSpan.TotalMilliseconds; }
        }

        public long AsTimeSpanTicks
        {
            get { return m_TimeSpan.Ticks; }
            //private set { m_TimeSpan = new TimeSpan(value); }
        }

        public Time Copy()
        {
            return new Time(this);
        }

        public void Add(Time other)
        {
            //m_TimeSpan = m_TimeSpan.Add(other.AsTimeSpan);

            m_TimeSpan += other.m_TimeSpan;
        }

        public void Substract(Time other)
        {
            //m_TimeSpan = m_TimeSpan.Subtract(other.AsTimeSpan);

            m_TimeSpan -= other.m_TimeSpan;
        }
    }
}