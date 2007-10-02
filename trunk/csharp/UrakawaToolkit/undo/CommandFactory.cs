using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;

namespace urakawa.undo
{
	/// <summary>
	/// Factory for creating <see cref="ICommand"/>s
	/// </summary>
	public class CommandFactory : WithPresentation, IXukAble
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

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="CommandFactory"/> from a CommandFactory xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read CommandFactory from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of CommandFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a CommandFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a CommandFactory xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a CommandFactory element to a XUK file representing the <see cref="CommandFactory"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void XukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination, baseUri);
				XukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of CommandFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a CommandFactory element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a CommandFactory element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="CommandFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="CommandFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
