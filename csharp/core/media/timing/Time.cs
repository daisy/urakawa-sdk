using System;

namespace urakawa.media.timing
{
    public partial class Time
    {
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

        public Time(double milliseconds)
        {
            AsMilliseconds = milliseconds;
        }

        public Time(TimeSpan timeSpan)
        {
            AsTimeSpan = new TimeSpan(timeSpan.Ticks);
        }

        public Time(string stringRepresentation)
        {
            AsMilliseconds = Parse(stringRepresentation);
        }

        public Time Copy()
        {
            return new Time(m_TimeSpan);
        }

        public TimeSpan AsTimeSpan
        {
            get { return new TimeSpan(m_TimeSpan.Ticks); }
            private set { m_TimeSpan = new TimeSpan(value.Ticks); }
        }

        public double AsMilliseconds
        {
            get { return m_TimeSpan.Ticks / ((double)TimeSpan.TicksPerMillisecond); }
            private set { AsTimeSpan = TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond)); }
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