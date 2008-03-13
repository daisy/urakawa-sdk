using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace urakawa.events
{
	public class DataModelChangedEventArgs : EventArgs
	{
		public DataModelChangedEventArgs(Object src)
		{
			SourceObject = src;
		}
		public readonly Object SourceObject;

		public override string ToString()
		{
			string res = String.Format(
				"{0}\n\tSourceObject={1}",
				base.ToString(),
				SourceObject);

			foreach (PropertyInfo prop in GetType().GetProperties())
			{
				Object o = prop.GetValue(this, null);
				if (!Object.ReferenceEquals(SourceObject, o))
				{
					res += String.Format("\n\t{0}={1}", prop.Name, o);
				}
			}
			foreach (FieldInfo field in GetType().GetFields())
			{
				Object o = field.GetValue(this);
				if (!Object.ReferenceEquals(SourceObject, o))
				{
					res += String.Format("\n\t{0}={1}", field.Name, o);
				}
			}
			return res;
		}
	}
}
