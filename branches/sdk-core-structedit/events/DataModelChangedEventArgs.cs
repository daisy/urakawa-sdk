using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace urakawa.events
{
    /// <summary>
    /// Base class for arguments of all data model events
    /// </summary>
    public class DataModelChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor setting the source object of the event
        /// </summary>
        /// <param name="src">The source</param>
        public DataModelChangedEventArgs(Object src)
        {
            SourceObject = src;
        }

        /// <summary>
        /// The source object of the event
        /// </summary>
        public readonly Object SourceObject;

        /// <summary>
        /// Gets a textual representation of the arguments
        /// </summary>
        /// <returns>The textual representation</returns>
        public override string ToString()
        {
            string res = String.Format(
                "{0}" + Environment.NewLine + "\tSourceObject={1}",
                base.ToString(),
                SourceObject);

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                Object o = prop.GetValue(this, null);
                if (!Object.ReferenceEquals(SourceObject, o))
                {
                    res += String.Format(Environment.NewLine + "\t{0}={1}", prop.Name, o);
                }
            }
            foreach (FieldInfo field in GetType().GetFields())
            {
                Object o = field.GetValue(this);
                if (!Object.ReferenceEquals(SourceObject, o))
                {
                    res += String.Format(Environment.NewLine + "\t{0}={1}", field.Name, o);
                }
            }
            return res;
        }
    }
}