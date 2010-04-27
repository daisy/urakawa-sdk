using System;
using System.Diagnostics;
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

        public Time(long timeInLocalUnits)
        {
            AsLocalUnits = timeInLocalUnits;
        }

        public Time(TimeSpan timeSpan)
        {
            AsTimeSpan = new TimeSpan(timeSpan.Ticks);
        }

        public Time(string stringRepresentation)
        {
            double timeMillisecondsDecimal = ParseToMilliseconds(stringRepresentation);

            int decimalPlaces = TIME_UNIT == 1 ? 0 : TIME_UNIT == 1000 ? 3 : TIME_UNIT == 1000000 ? 6 : 7;
            DebugFix.Assert(decimalPlaces != 7);
            timeMillisecondsDecimal = Math.Round(timeMillisecondsDecimal, decimalPlaces, MidpointRounding.AwayFromZero);

#if DEBUG
            TimeSpan timeSpan = TimeSpan.Parse(stringRepresentation);
            DebugFix.Assert(timeSpan.TotalMilliseconds == timeMillisecondsDecimal);
#endif //DEBUG


            double timeAsLocalUnitsDecimal = timeMillisecondsDecimal * TIME_UNIT;
            long timeAsLocalUnitsIntegral = (long)(AudioLibPCMFormat.USE_ROUND_NOT_TRUNCATE ? Math.Round(timeAsLocalUnitsDecimal) : Math.Truncate(timeAsLocalUnitsDecimal));

            // checking whether we are loosing fractions of milliseconds
            DebugFix.Assert(timeAsLocalUnitsDecimal == (double)timeAsLocalUnitsIntegral);

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
                if (m_TimeSpan == TimeSpan.MaxValue)
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
                if (value == long.MaxValue)
                {
                    AsTimeSpan = TimeSpan.MaxValue;
                    return;
                }
                if (value == long.MinValue)
                {
                    AsTimeSpan = TimeSpan.MinValue;
                    return;
                }
                if (value == 0)
                {
                    AsTimeSpan = TimeSpan.Zero;
                    return;
                }

                double timeMillisecondsDecimal = (double)value / TIME_UNIT;

                int decimalPlaces = TIME_UNIT == 1 ? 0 : TIME_UNIT == 1000 ? 3 : TIME_UNIT == 1000000 ? 6 : 7;
                DebugFix.Assert(decimalPlaces != 7);
                timeMillisecondsDecimal = Math.Round(timeMillisecondsDecimal, decimalPlaces, MidpointRounding.AwayFromZero);

                double ticksDecimal = timeMillisecondsDecimal * TimeSpan.TicksPerMillisecond;
                long ticksIntegral = (long)(AudioLibPCMFormat.USE_ROUND_NOT_TRUNCATE ? Math.Round(ticksDecimal) : Math.Truncate(ticksDecimal));

                // checking whether we are loosing fractions of milliseconds
                /////DebugFix.Assert(ticksDecimal == (double)ticksIntegral);

                AsTimeSpan = TimeSpan.FromTicks(ticksIntegral);
            }
        }

        public TimeSpan AsTimeSpan
        {
            get { return new TimeSpan(m_TimeSpan.Ticks); }
            private set { m_TimeSpan = new TimeSpan(value.Ticks); }
        }


        public Time Copy()
        {
            return new Time(m_TimeSpan);
        }

        public void Add(Time other)
        {
            //mTime = mTime.Add(other.AsTimeSpan);
            m_TimeSpan += other.AsTimeSpan;
        }

        public void Substract(Time time)
        {
            //mTime = mTime.Subtract(other.AsTimeSpan);
            m_TimeSpan -= time.AsTimeSpan;
        }

        //public void Add(Time other)
        //{
        //    //mTime = mTime.Add(other.AsTimeSpan);
        //    m_TimeSpan += other.AsTimeSpan;
        //}

        //public void Subtract(Time timeDelta)
        //{
        //    //mTime = mTime.Subtract(other.AsTimeSpan);
        //    m_TimeSpan -= timeDelta.AsTimeSpan;
        //}
    }
}