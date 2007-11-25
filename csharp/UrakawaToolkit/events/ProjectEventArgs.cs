using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class ProjectEventArgs : DataModelChangedEventArgs
	{
		public ProjectEventArgs(Project source)
			: base(source)
		{
			SourceProject = source;
		}
		public readonly Project SourceProject;
	}
}
