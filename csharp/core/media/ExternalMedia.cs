using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.events.media;

namespace urakawa.media
{
	/// <summary>
	/// Common abstract base class for external (ie. <see cref="ILocated"/> <see cref="IMedia"/>
	/// </summary>
	public abstract class ExternalMedia : AbstractMedia, ILocated
	{
		
		#region Event related members
		/// <summary>
		/// Event fired after the src has changed
		/// </summary>
		public event EventHandler<SrcChangedEventArgs> srcChanged;
		/// <summary>
		/// Fires the <see cref="srcChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="ExternalMedia"/> whoose src value changed</param>
		/// <param name="newVal">The new src value</param>
		/// <param name="prevVal">The src value prior to the change</param>
		protected void notifySrcChanged(ExternalMedia source, string newVal, string prevVal)
		{
			EventHandler<SrcChangedEventArgs> d = srcChanged;
			if (d != null) d(this, new SrcChangedEventArgs(source, newVal, prevVal));
		}

		void this_srcChanged(object sender, SrcChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion


		private string mSrc;

		internal ExternalMedia()
		{
			mSrc = ".";
			this.srcChanged += new EventHandler<SrcChangedEventArgs>(this_srcChanged);
		}

		#region IMedia Members

		/// <summary>
		/// Creates a copy of the <see cref="ExternalMedia"/>
		/// </summary>
		/// <returns>The copy</returns>
		public new ExternalMedia copy()
		{
			return copyProtected() as ExternalMedia;
		}

		/// <summary>
		/// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="ExternalMedia"/></returns>
		/// <remarks>The current instance is left untouched to the export</remarks>
		public new ExternalMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as ExternalMedia;
		}

		/// <summary>
		/// Exports the <see cref="ExternalMedia"/> to a given destination <see cref="Presentation"/>
		/// - part of a technical solution to have the <see cref="export"/> method return the correct <see cref="Type"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="ExternalMedia"/></returns>
		/// <remarks>The current instance is left untouched to the export</remarks>
		protected override IMedia exportProtected(Presentation destPres)
		{
			ExternalMedia expEM = base.exportProtected(destPres) as ExternalMedia;
			if (expEM == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			if (Uri.IsWellFormedUriString(getSrc(), UriKind.Relative))
			{
				string destSrc = destPres.RootUri.MakeRelativeUri(getUri()).ToString();
				if (destSrc == "") destSrc = ".";
				expEM.setSrc(destSrc);
			}
			else
			{
				expEM.setSrc(getSrc());
			}
			return expEM;
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Clears to <see cref="ExternalMedia"/>, resetting the src value
		/// </summary>
		protected override void clear()
		{
			mSrc = ".";
			base.clear();
		}

		/// <summary>
		/// Reads the attributes of a ExternalMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string val = source.GetAttribute("src");
			if (val == null || val == "") val = ".";
			setSrc(val);
			base.xukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a ExternalMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			if (getSrc() != "")
			{
				Uri srcUri = new Uri(getMediaFactory().Presentation.RootUri, getSrc());
				if (baseUri == null)
				{
					destination.WriteAttributeString("src", srcUri.AbsoluteUri);
				}
				else
				{
					destination.WriteAttributeString("src", baseUri.MakeRelativeUri(srcUri).ToString());
				}
			}
			base.xukOutAttributes(destination, baseUri);
		}

		#endregion


		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Determines if the <see cref="ExternalMedia"/> has the same value as a given other <see cref="IMedia"/>
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns>A <see cref="bool"/> indicating value equality</returns>
		public override bool ValueEquals(IMedia other)
		{
			if (!base.ValueEquals(other)) return false;
			ExternalMedia emOther = (ExternalMedia)other;
			if (getUri() != emOther.getUri()) return false;
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
			string prevSrc = mSrc;
			mSrc = newSrc;
			if (mSrc!=prevSrc) notifySrcChanged(this, mSrc, prevSrc);
		}

		/// <summary>
		/// Gets the <see cref="Uri"/> of the <see cref="ExternalMedia"/> 
		/// - uses <c>getMediaFactory().getPresentation().getRootUri()</c> as base <see cref="Uri"/>
		/// </summary>
		/// <returns>The <see cref="Uri"/></returns>
		/// <exception cref="exception.InvalidUriException">
		/// Thrown when the value returned by <see cref="getSrc"/> is not a well-formed <see cref="Uri"/>
		/// </exception>
		public Uri getUri()
		{
			if (!Uri.IsWellFormedUriString(getSrc(), UriKind.RelativeOrAbsolute))
			{
				throw new exception.InvalidUriException(String.Format(
					"The src value '{0}' is not a well-formed Uri", getSrc()));
			}
			return new Uri(getMediaFactory().Presentation.RootUri, getSrc());
		}

		#endregion
	}
}
