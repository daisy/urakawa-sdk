using System;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// An implementation of <see cref="IMediaLocation"/> based on a simple Src string value
	/// representing the uri or path of the media location
	/// </summary>
	public class SrcMediaLocation : ISrcMediaLocation
	{
		private string mSrc = "";
		private IMediaFactory mFactory;

		/// <summary>
		/// Constructor initializing the <see cref="SrcMediaLocation"/> with a 
		/// </summary>
		protected internal SrcMediaLocation(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The media factory can not be null");
			}
			mFactory = fact;
		}

		/// <summary>
		/// Gets the Src value of <c>this</c>
		/// </summary>
		/// <returns>The Src value</returns>
		public string getSrc()
		{
			return mSrc;
		}

		/// <summary>
		/// Sets the Src value of this
		/// </summary>
		/// <param name="newSrc">The new Src value - must not be <c>null</c></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new Src value is <c>null</c>
		/// </exception>
		public void setSrc(string newSrc)
		{
			if (newSrc == null)
			{
				throw new exception.MethodParameterIsNullException("The Src can not be null");
			}
			mSrc = newSrc;
		}

		/// <summary>
		/// Returns Src as <see cref="string"/> representation of <c>this</c>
		/// </summary>
		/// <returns>The Src value prefixed with MediaLocation=</returns>
		public override string ToString()
		{
			return String.Format("{0}={1}", GetType().Name, getSrc());
		}

		# region IMediaLocation members
		IMediaLocation IMediaLocation.copy()
		{
			return copy();
		}

		/// <summary>
		/// Copy the media location object.
		/// </summary>
		/// <returns>The copy</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the associated <see cref="IMediaFactory"/> 
		/// can not create a <see cref="SrcMediaLocation"/> instance
		/// </exception>
		public SrcMediaLocation copy()
		{
			IMediaLocation iCopyLoc = getMediaFactory().createMediaLocation(
				getXukLocalName(), getXukNamespaceUri());
			if (iCopyLoc == null || !(GetType().IsAssignableFrom(iCopyLoc.GetType())))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory could not create a {0} (QName {1}:{2})",
					GetType().FullName, getXukLocalName(), getXukNamespaceUri()));
			}
			SrcMediaLocation copyLoc = (SrcMediaLocation)iCopyLoc;
			copyLoc.setSrc(getSrc());
			return copyLoc;
		}

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="IMediaLocation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		# endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="SrcMediaLocation"/> from a SrcMediaLocation xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a SrcMediaLocation xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			string src = source.GetAttribute("src");
			if (src == null || src == "") return false;
			setSrc(src);
			return true;
		}

		/// <summary>
		/// Reads a child of a SrcMediaLocation xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			// No children to read


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element
			}
			return true;
		}

		/// <summary>
		/// Write a SrcMediaLocation element to a XUK file representing the <see cref="SrcMediaLocation"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a SrcMediaLocation element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("src", getSrc());
			return true;
		}

		/// <summary>
		/// Write the child elements of a SrcMediaLocation element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// No children to write children
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="SrcMediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="SrcMediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region Old IXukAble Members

		///// <summary>
		///// Loads the <see cref="SrcMediaLocation"/>from an xuk element
		///// </summary>
		///// <param name="source">The source <see cref="XmlReader"/></param>
		///// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		///// <exception cref="exception.MethodParameterIsNullException">
		///// Thrown when the source <see cref="XmlReader"/> is <c>null</c>
		///// </exception>
		//public bool XukIn(XmlReader source)
		//{
		//  if (source == null)
		//  {
		//    throw new exception.MethodParameterIsNullException(
		//      "The source XmlReader is null");
		//  }
		//  if (source.NodeType != XmlNodeType.Element) return false;
		//  string src = source.GetAttribute("src");
		//  if (src == null) return false;
		//  setSrc(src);
		//  if (!source.IsEmptyElement)
		//  {
		//    //Read past element subtree, leaving the curcor the the element end tag
		//    source.ReadSubtree().Close();
		//  }
		//  return true;
		//}

		///// <summary>
		///// Writes the <see cref="SrcMediaLocation"/> to an xuk element
		///// </summary>
		///// <param name="destination">The destination <see cref="XmlWriter"/></param>
		///// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		///// <exception cref="exception.MethodParameterIsNullException">
		///// Thrown when the destination <see cref="XmlWriter"/> is <c>null</c>
		///// </exception>
		//public bool XukOut(XmlWriter destination)
		//{
		//  if (destination == null)
		//  {
		//    throw new exception.MethodParameterIsNullException(
		//      "The destination XmlWriter is null");
		//  }
		//  destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
		//  destination.WriteAttributeString("src", getSrc());
		//  destination.WriteEndElement();
		//  return true;
		//}

		
		///// <summary>
		///// Gets the local localName part of the QName representing a <see cref="SrcMediaLocation"/> in Xuk
		///// </summary>
		///// <returns>The local localName part</returns>
		//public string getXukLocalName()
		//{
		//  return this.GetType().Name;
		//}

		///// <summary>
		///// Gets the namespace uri part of the QName representing a <see cref="SrcMediaLocation"/> in Xuk
		///// </summary>
		///// <returns>The namespace uri part</returns>
		//public string getXukNamespaceUri()
		//{
		//  return urakawa.ToolkitSettings.XUK_NS;
		//}

		#endregion

		#region IValueEquatable<IMediaLocation> Members

		/// <summary>
		/// Compares <c>this</c> with a given other <see cref="IMediaLocation"/> for equality
		/// </summary>
		/// <param name="other">The given other <see cref="IMediaLocation"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IMediaLocation other)
		{
			if (!(other is SrcMediaLocation)) return false;
			SrcMediaLocation otherSrc = (SrcMediaLocation)other;
			return getSrc() == otherSrc.getSrc();
		}

		#endregion
	}
}
