using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.xuk;

namespace urakawa.metadata
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
	public class Metadata : IXukAble, urakawa.events.IChangeNotifier
	{
		
		#region Event related members

		/// <summary>
		/// Event fired after the <see cref="Metadata"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(urakawa.events.DataModelChangedEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangedEventArgs> d = changed;
			if (d != null) d(this, args);
		}
		/// <summary>
		/// Event fired after the name of the <see cref="Metadata"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.metadata.NameChangedEventArgs> nameChanged;
		/// <summary>
		/// Fires the <see cref="nameChanged"/> event
		/// </summary>
		/// <param name="newName">The new name</param>
		/// <param name="prevName">The name prior to the change</param>
		protected void notifyNameChanged(string newName, string prevName)
		{
			EventHandler<urakawa.events.metadata.NameChangedEventArgs> d = nameChanged;
			if (d != null) d(this, new urakawa.events.metadata.NameChangedEventArgs(this, newName, prevName));
		}
		/// <summary>
		/// Event fired after the content of the <see cref="Metadata"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.metadata.ContentChangedEventArgs> contentChanged;
		/// <summary>
		/// Fires the <see cref="contentChanged"/> event
		/// </summary>
		/// <param name="newContent">The new content</param>
		/// <param name="prevContent">The content prior to the change</param>
		protected void notifyContentChanged(string newContent, string prevContent)
		{
			EventHandler<urakawa.events.metadata.ContentChangedEventArgs> d = contentChanged;
			if (d != null) d(this, new urakawa.events.metadata.ContentChangedEventArgs(this, newContent, prevContent));
		}
		/// <summary>
		/// Event fired after the optional attribute of the <see cref="Metadata"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.metadata.OptionalAttributeChangedEventArgs> optionalAttributeChanged;
		/// <summary>
		/// Fires the <see cref="optionalAttributeChanged"/> event
		/// </summary>
		/// <param name="name">The name of the optional attribute</param>
		/// <param name="newVal">The new value of the optional attribute</param>
		/// <param name="prevValue">The value of the optional attribute prior to the change</param>
		protected void notifyOptionalAttributeChanged(string name, string newVal, string prevValue)
		{
			EventHandler<urakawa.events.metadata.OptionalAttributeChangedEventArgs> d = optionalAttributeChanged;
			if (d != null) d(this, new urakawa.events.metadata.OptionalAttributeChangedEventArgs(this, name, newVal, prevValue));
		}

		#endregion


		private string mName;

		private Dictionary<string, string> mAttributes;

		/// <summary>
		/// Default constructor, Name, Content and Scheme are initialized to <see cref="String.Empty"/>
		/// </summary>
		internal Metadata()
		{
			mName = "";
			mAttributes = new Dictionary<string, string>();
			mAttributes.Add("content", "");
		}
		
		#region Metadata Members

		/// <summary>
		/// Gets the name
		/// </summary>
		/// <returns>The name</returns>
		public string getName()
		{
			return mName;
		}

		/// <summary>
		/// Sets the name
		/// </summary>
		/// <param name="newName">The new name value</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newLocalName"/> is null
		/// </exception>
		public void setName(string newName)
		{
			if (newName == null)
			{
				throw new exception.MethodParameterIsNullException(
				  "The name can no t be null");
			}
			string prevName = mName;
			mName = newName;
			if (prevName!=mName) notifyNameChanged(newName, prevName);
		}

		/// <summary>
		/// Gets the content
		/// </summary>
		/// <returns>The content, or null if none has been set yet.</returns>
		public string getContent()
		{
            return mAttributes.ContainsKey("content") ? mAttributes["content"] : null;
		}

		/// <summary>
		/// Sets the content
		/// </summary>
		/// <param name="newContent">The  new content value</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newContent"/> is null
		/// </exception>
		public void setContent(string newContent)
		{
			if (newContent == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Content can not be null");
			}
			string prevContent = getContent();
			mAttributes["content"] = newContent;
			if (newContent != prevContent) notifyContentChanged(newContent, prevContent);
		}

		/// <summary>
		/// Gets the value of a named attribute
		/// </summary>
		/// <param name="name">The name of the attribute</param>
		/// <returns>The value of the attribute - <see cref="String.Empty"/> if the attribute does not exist</returns>
		public string getOptionalAttributeValue(string name)
		{
			if (mAttributes.ContainsKey(name))
			{
				return mAttributes[name];
			}
			return "";
		}

		/// <summary>
		/// Sets the value of a named attribute
		/// </summary>
		/// <param name="name">The name of the attribute</param>
		/// <param name="value">The new value for the attribute</param>
		public void setOptionalAttributeValue(string name, string value)
		{
			if (value == null)
			{
				throw new exception.MethodParameterIsNullException(
					"A metadata attribute can not have null value");
			}
			if (name == "name") setName(value);
			if (name == "content") setContent(name);
			string prevValue = getOptionalAttributeValue(name);
			if (mAttributes.ContainsKey(name))
			{
				mAttributes[name] = value;
			}
			else
			{
				mAttributes.Add(name, value);
			}
			if (prevValue != name) notifyOptionalAttributeChanged(name, value, prevValue);
		}

		/// <summary>
		/// Gets the names of all attributes with non-empty names
		/// </summary>
		/// <returns>A <see cref="List{String}"/> containing the attribute names</returns>
		public List<string> getOptionalAttributeNames()
		{
			List<string> names = new List<string>(mAttributes.Keys);
			foreach (string name in new List<string>(names))
			{
				if (mAttributes[name] == "") names.Remove(name);
			}
			return names;
		}

		#endregion


		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Metadata"/> from a Metadata xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void xukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read Metadata from a non-element node");
			}
			mAttributes.Clear();
			try
			{
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source);
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
					String.Format("An exception occured during xukIn of Metadata: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Metadata xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
			if (source.MoveToFirstAttribute())
			{
				bool moreAttrs = true;
				while (moreAttrs)
				{
					setOptionalAttributeValue(source.Name, source.Value);
					moreAttrs = source.MoveToNextAttribute();
				}
				source.MoveToElement();
			}
		}

		/// <summary>
		/// Reads a child of a Metadata xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a Metadata element to a XUK file representing the <see cref="Metadata"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void xukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not xukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukOut of Metadata: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Metadata element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("name", getName());
			foreach (string a in getOptionalAttributeNames())
			{
				if (a != "name")
				{
					destination.WriteAttributeString(a, getOptionalAttributeValue(a));
				}
			}
		}

		/// <summary>
		/// Write the child elements of a Metadata element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region IValueEquatable<Metadata> Members

		/// <summary>
		/// Determines if <c>this</c> is value equal to another given <see cref="Metadata"/>
		/// </summary>
		/// <param name="other">The other <see cref="Metadata"/></param>
		/// <returns>The result as a <see cref="bool"/></returns>
		public bool ValueEquals(Metadata other)
		{
			if (!(other is Metadata)) return false;
			Metadata mOther = (Metadata)other;
			if (getName() != other.getName()) return false;
			List<string> names = getOptionalAttributeNames();
			List<string> otherNames = getOptionalAttributeNames();
			if (names.Count != otherNames.Count) return false;
			foreach (string name in names)
			{
				if (!otherNames.Contains(name)) return false;
				if (getOptionalAttributeValue(name) != other.getOptionalAttributeValue(name)) return false;
			}
			return true;
		}

		#endregion
	}
}
