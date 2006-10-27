using System;

namespace urakawa.media
{
	/// <summary>
	/// MediaLocation is just a string which represents a file's path
	/// This simple idea could be extended in the (near)future.
	/// </summary>
	public class MediaLocation : IMediaLocation
	{
		private string mSrc = "";

		/// <summary>
		/// Default constructor
		/// </summary>
		protected internal MediaLocation()
		{
		}

		# region IMediaLocation members
		/// <summary>
		/// Copy the media location object.
		/// </summary>
		/// <returns>The copy</returns>
		public IMediaLocation copy()
		{
			return new MediaLocation(Location);
		}


		# endregion

		/// <summary>
		/// Returns <see cref="Location"/> as <see cref="string"/> representation of <c>this</c>
		/// </summary>
		/// <returns><see cref="Location"/></returns>
		public override string ToString()
		{
			return String.Format("MediaLocation={0}", Location);
		}

		#region IXukAble Members

		/// <summary>
		/// Loads the <see cref="MediaLocation"/>from an xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		/// <exception cref="">
		/// Thrown when the source <see cref="XmlReader"/> is <c>null</c>
		/// </exception>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The source XmlReader is null");
			}
			if (!source.NodeType == System.Xml.XmlNodeType.Element) return false;
			string src = source.GetAttribute("mLocation");
			if (src == null) return false;
			Location = src;
			if (!source.IsEmptyElement)
			{
				//Read past element subtree, leaving the curcor the the element end tag
				source.ReadSubtree().Close();
			}
			return true;
		}

		/// <summary>
		/// Writes the <see cref="MediaLication"/> to an xuk element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		/// <exception cref="">
		/// Thrown when the destination <see cref="XmlWriter"/> is <c>null</c>
		/// </exception>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The destination XmlWriter is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteAttributeString("mLocation", Location);
			destination.WriteEndElement();
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}
