using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;

namespace urakawa.command
{
    /// <summary>
    /// Factory for creating <see cref="ICommand"/>s
    /// </summary>
    public class CommandFactory : WithPresentation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        internal protected CommandFactory()
        {
        }

        /// <summary>
        /// Creates a <see cref="ICommand"/> matching a given Xuk QName
        /// </summary>
        /// <param name="xukLocalName">The local name part of the Xuk QName</param>
        /// <param name="xukNamespaceUri">The namespace uri part of the Xuk QName</param>
        /// <returns>The created command or <c>null</c> if the Xuk QName is not recognized</returns>
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

        /// <summary>
        /// Creates a <see cref="CompositeCommand"/>
        /// </summary>
        /// <returns>The created composite command</returns>
        public CompositeCommand createCompositeCommand()
        {
            CompositeCommand newCmd = new CompositeCommand();
            newCmd.setPresentation(getPresentation());
            return newCmd;
        }

		

    }
}