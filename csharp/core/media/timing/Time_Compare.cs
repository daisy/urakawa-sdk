using System;
using AudioLib;

namespace urakawa.media.timing
{
    public partial class Time
    {
        public static bool operator ==(Time a, Time b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.IsEqualTo(b);
        }

        public static bool operator !=(Time a, Time b)
        {
            return !(a == b);
        }

        public bool IsEqualTo(Time otherTime)
        {
            if (IsGreaterThan(otherTime)) return false;
            if (otherTime.IsGreaterThan(this)) return false;
            return true;
        }

        public bool IsGreaterThan(Time time)
        {
            TimeSpan t1 = AsTimeSpan;
            TimeSpan t2 = time.AsTimeSpan;

            // one millisecond resolution
            long t1ms = (long)Math.Truncate(t1.TotalMilliseconds);
            long t2ms = (long)Math.Truncate(t2.TotalMilliseconds);
            if (t1ms == t2ms) return false;

            if (Math.Abs(t1ms - t2ms) <= AudioLibPCMFormat.MILLISECONDS_TOLERANCE) return false;

            return t1.CompareTo(t2) > 0;
            //return AsLocalUnits > time.AsLocalUnits;
        }

        public bool IsLessThan(Time time)
        {
            return time.IsGreaterThan(this);
        }

        public bool IsGreaterThanOrEqualTo(Time time)
        {
            return !IsLessThan(time);
        }

        public bool IsLessThanOrEqualTo(Time time)
        {
            return time.IsGreaterThanOrEqualTo(this);
        }

        public bool IsNegative
        {
            get { return (m_TimeSpan < TimeSpan.Zero); }
        }

        public Time GetDifference(Time time)
        {
            if (m_TimeSpan > time.m_TimeSpan)
            {
                return new Time(m_TimeSpan.Subtract(time.m_TimeSpan));
            }
            return new Time(time.m_TimeSpan.Subtract(m_TimeSpan));
        }

        //public Time GetDifference(Time time)
        //{
        //    if (m_TimeSpan > time.m_TimeSpan)
        //    {
        //        return new Time(m_TimeSpan.Subtract(time.m_TimeSpan));
        //    }
        //    return new Time(time.m_TimeSpan.Subtract(m_TimeSpan));
        //}
    }
}
