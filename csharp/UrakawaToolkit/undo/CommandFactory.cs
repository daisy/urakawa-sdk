using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.undo
{
	public class CommandFactory : WithPresentation
	{
		public ICommand createCommand(string xukLocalName, string xukNamespaceUri)
		{
			if (xukNamespaceUri == ToolkitSettings.XUK_NS)
			{
				if (xukLocalName == typeof(CompositeCommand).Name)
				{
					return createCompositeCommand();
				}
			}
			return null;
		}

		public CompositeCommand createCompositeCommand()
		{
			return new CompositeCommand();
		}
	}
}
