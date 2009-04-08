using System;
using System.Xml;
using urakawa.progress;

namespace urakawa.xuk
{
	/// <summary>
	/// Common base class for classes that implement <see cref="IXukAble"/>
	/// </summary>
	public class XukAble : IXukAble
	{
		
		#region IXUKAble members

		/// <summary>
		/// Clears the <see cref="XukAble"/> of any data - called at the beginning of <see cref="xukIn"/>
		/// </summary>
		protected virtual void clear()
		{
		}

	    /// <summary>
	    /// The implementation of XUKIn is expected to read and remove all tags
	    /// up to and including the closing tag matching the element the reader was at when passed to it.
	    /// The call is expected to be forwarded to any owned element, in effect making it a recursive read of the XUK file
	    /// </summary>
	    /// <param name="source">The XmlReader to read from</param>
	    /// <param name="handler">The handler for progress</param>
	    public void xukIn(XmlReader source, ProgressHandler handler)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read XukAble from a non-element node");
			}
            if (handler!=null)
            {
                if (handler.notifyProgress())
                {
                    string msg = String.Format("XukIn cancelled at element {0}:{1}", getXukLocalName(),
                                               getXukNamespaceUri());
                    throw new exception.ProgressCancelledException(msg);
                }
            }
			try
			{
				clear();
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source, handler);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
                catch (exception.ProgressCancelledException)
                {
                    throw;
                }
			catch (exception.XukException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of XukAble: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a XukAble xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a XukAble xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <param name="handler">The handler of progress</param>
		protected virtual void xukInChild(XmlReader source, ProgressHandler handler)
		{
    		if (!source.IsEmptyElement) source.ReadSubtree().Close();//Read past unknown child 
		}

		/// <summary>
		/// Write a XukAble element to a XUK file representing the <see cref="XukAble"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
        /// <param name="handler">The handler for progress</param>
        public void xukOut(XmlWriter destination, Uri baseUri, ProgressHandler handler)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
            if (handler!=null)
            {
                if (handler.notifyProgress()) 
                {
                    string msg = String.Format("XukOut cancelled at {0}", ToString());
                    throw new exception.ProgressCancelledException(msg);
                }

            }
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri, handler);
				destination.WriteEndElement();

			}
            catch(exception.ProgressCancelledException)
            {
                throw;
            }
			catch (exception.XukException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of XukAble: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a XukAble element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a XukAble element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
        /// <param name="handler">The handler for progress</param>
        protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="XukAble"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="XukAble"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
