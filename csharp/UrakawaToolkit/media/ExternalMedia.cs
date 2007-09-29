using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// Common abstract base class for external (ie. <see cref="ILocated"/> <see cref="IMedia"/>
	/// </summary>
	public abstract class ExternalMedia : IMedia, ILocated
	{
		private IMediaFactory mMediaFactory;
		private string mLanguage;
		private string mSrc;

		internal ExternalMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The IMediaFactory can not be null");
			}
			mMediaFactory = fact;
			mLanguage = null;
			mSrc = ".";
		}

		#region IMedia Members

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> if the <see cref="ExternalMedia"/>
		/// </summary>
		/// <returns>The factory</returns>
		public IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}

		/// <summary>
		/// Determines if the <see cref="ExternalMedia"/> is continuous
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="ExternalMedia"/> is continuous</returns>
		public abstract bool isContinuous();


		/// <summary>
		/// Determines if the <see cref="ExternalMedia"/> is discrete
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="ExternalMedia"/> is discrete</returns>
		public abstract bool isDiscrete();

		/// <summary>
		/// Determines if the <see cref="ExternalMedia"/> is a <see cref="SequenceMedia"/>
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="ExternalMedia"/> is a <see cref="SequenceMedia"/></returns>
		public abstract bool isSequence();

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of the <see cref="ExternalMedia"/>
		/// </summary>
		/// <returns>The copy</returns>
		public ExternalMedia copy()
		{
			return copyProtected();
		}

		/// <summary>
		/// Creates a copy of the <see cref="ExternalMedia"/>
		/// - part of a technical solution to have the <see cref="copy"/> method return the correct <see cref="Type"/>
		/// </summary>
		/// <returns>The copy</returns>
		protected abstract ExternalMedia copyProtected();

		IMedia IMedia.export(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="ExternalMedia"/></returns>
		/// <remarks>The current instance is left untouched to the export</remarks>
		public ExternalMedia export(Presentation destPres)
		{
			return exportProtected(destPres);
		}

		/// <summary>
		/// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
		/// - part of a technical solution to have the <see cref="export"/> method return the correct <see cref="Type"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="ExternalMedia"/></returns>
		/// <remarks>The current instance is left untouched to the export</remarks>
		protected abstract ExternalMedia exportProtected(Presentation destPres);

		/// <summary>
		/// Sets the language of the external media
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the external media
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="ExternalMedia"/> from a ExternalMedia xuk element
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
				throw new exception.XukException("Can not read ExternalMedia from a non-element node");
			}
			try
			{
				mSrc = ".";
				mLanguage = null;
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
					String.Format("An exception occured during XukIn of ExternalMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ExternalMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string val = source.GetAttribute("src");
			if (val != null) setSrc(val);
			val = source.GetAttribute("language");
			if (val != null) setLanguage(val);
		}

		/// <summary>
		/// Reads a child of a ExternalMedia xuk element. 
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
		/// Write a ExternalMedia element to a XUK file representing the <see cref="ExternalMedia"/> instance
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
					String.Format("An exception occured during XukOut of ExternalMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ExternalMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			if (getSrc() != "")
			{
				Uri srcUri = new Uri(getMediaFactory().getPresentation().getRootUri(), getSrc());
				if (baseUri == null)
				{
					destination.WriteAttributeString("src", srcUri.AbsoluteUri);
				}
				else
				{
					destination.WriteAttributeString("src", baseUri.MakeRelativeUri(srcUri).ToString());
				}
			}
			if (getLanguage() != null)
			{
				destination.WriteAttributeString("language", getLanguage());
			}
		}

		/// <summary>
		/// Write the child elements of a ExternalMedia element.
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
		/// Gets the local name part of the QName representing a <see cref="ExternalMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ExternalMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Determines if the <see cref="ExternalMedia"/> has the same value as a given other <see cref="IMedia"/>
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns>A <see cref="bool"/> indicating value equality</returns>
		public virtual bool valueEquals(IMedia other)
		{
			if (other == null) return false;
			if (GetType() != other.GetType()) return false;
			ExternalMedia emOther = (ExternalMedia)other;
			if (getLanguage() != emOther.getLanguage()) return false;
			if (getSrc() != emOther.getSrc()) return false;
			return true;
		}

		#endregion

		#region ILocated Members

		/// <summary>
		/// Gets the src value. The default value is "."
		/// </summary>
		/// <returns>The src value</returns>
		public string getSrc()
		{
			return mSrc;
		}

		/// <summary>
		/// Sets the src value.
		/// </summary>
		/// <param name="newSrc">The new src value, can not be <c>null</c> or <see cref="String.Empty"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newSrc"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsEmptyStringException">
		/// Thrown when <paramref name="newSrc"/> is <see cref="String.Empty"/>
		/// </exception>
		public void setSrc(string newSrc)
		{
			if (newSrc == null) throw new exception.MethodParameterIsNullException("The src value can not be null");
			if (newSrc == "") throw new exception.MethodParameterIsEmptyStringException("The src value can not be an empty string");
			mSrc = newSrc;
		}

		#endregion
	}
}
