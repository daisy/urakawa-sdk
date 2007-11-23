using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// An implementation of <see cref="ITextMedia"/> based on text storage in an external file/uri
	/// </summary>
	public class ExternalTextMedia : ExternalMedia, ITextMedia
	{

		#region Event related members
		/// <summary>
		/// Event fired after the text of the <see cref="ExternalTextMedia"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.TextChangedEventArgs> textChanged;
		/// <summary>
		/// Fires the <see cref="textChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="ExternalTextMedia"/> whoose text was changed</param>
		/// <param name="newText">The new text value</param>
		/// <param name="prevText">Thye text value prior to the change</param>
		protected void notifyTextChanged(ExternalTextMedia source, string newText, string prevText)
		{
			EventHandler<urakawa.events.TextChangedEventArgs> d = textChanged;
			if (d != null) d(this, new urakawa.events.TextChangedEventArgs(source, newText, prevText));
		}

		void this_textChanged(object sender, urakawa.events.TextChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion
		

		/// <summary>
		/// Constructor setting the <see cref="IMediaFactory"/> that created the instance
		/// </summary>
		protected internal ExternalTextMedia()
		{
			this.textChanged += new EventHandler<urakawa.events.TextChangedEventArgs>(this_textChanged);
		}


		#region IMedia Members

		/// <summary>
		/// Determines if <c>this</c> is a continuous media (wich it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public override bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// Determines if <c>this</c> is a descrete media (which it is)
		/// </summary>
		/// <returns><c>true</c></returns>
		public override bool isDiscrete()
		{
			return true;
		}

		/// <summary>
		/// Determines if <c>this</c>is a sequence media (which it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public override bool isSequence()
		{
			return false;
		}


		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public new ExternalTextMedia copy()
		{
			return copyProtected() as ExternalTextMedia;
		}

		

		/// <summary>
		/// Exports the external text media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external text media</returns>
		protected override IMedia exportProtected(Presentation destPres)
		{
			ExternalTextMedia exported = base.exportProtected(destPres) as ExternalTextMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalTextMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			return exported;
		}

		/// <summary>
		/// Exports the external text media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external text media</returns>
		public new ExternalTextMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as ExternalTextMedia;
		}


		#endregion

		#region ITextMedia Members

		/// <summary>
		/// Gets the text of the <c>this</c>
		/// </summary>
		/// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
		/// <exception cref="exception.CannotReadFromExternalFileException">
		/// Thrown if the file referenced by <see cref="ExternalMedia.getSrc"/> is not accessible
		/// </exception>
		public string getText()
		{
			WebClient client = new WebClient();
			client.UseDefaultCredentials = true;
			return getText(client);
		}

		/// <summary>
		/// Gets the text of the <c>this</c> using given <see cref="ICredentials"/>
		/// </summary>
		/// <param name="credits">The given credentisals</param>
		/// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
		/// <exception cref="exception.CannotReadFromExternalFileException">
		/// Thrown if the file referenced by <see cref="ExternalMedia.getSrc"/> is not accessible
		/// </exception>
		public string getText(ICredentials credits)
		{
			WebClient client = new WebClient();
			client.Credentials = credits;
			return getText(client);
		}

		private string getText(WebClient client)
		{
			try
			{
				Uri src = getUri();
				StreamReader rd = new StreamReader(client.OpenRead(src));
				string res = rd.ReadToEnd();
				rd.Close();
				return res;
			}
			catch (Exception e)
			{
				throw new exception.CannotReadFromExternalFileException(
					String.Format("Could read the text from plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		/// <summary>
		/// Sets the text of <c>this</c>
		/// </summary>
		/// <param name="text">The new text</param>
		/// <exception cref="exception.CannotWriteToExternalFileException">
		/// Thrown when the text could not be written to the <see cref="Uri"/> (as returned by <see cref="ExternalMedia.getSrc"/>)
		/// using the <see cref="WebClient.UploadData(Uri, byte[])"/> method.
		/// </exception>
		public void setText(string text)
		{
			WebClient client = new WebClient();
			client.UseDefaultCredentials = true;
			setText(text, client);
		}

		/// <summary>
		/// Sets the text of <c>this</c> using given <see cref="ICredentials"/>
		/// </summary>
		/// <param name="text">The new text</param>
		/// <param name="credits">The given credentisals</param>
		/// <exception cref="exception.CannotWriteToExternalFileException">
		/// Thrown when the text could not be written to the <see cref="Uri"/> (as returned by <see cref="ExternalMedia.getSrc"/>)
		/// using the <see cref="WebClient.UploadData(Uri, byte[])"/> method.
		/// </exception>
		public void setText(string text, ICredentials credits)
		{
			WebClient client = new WebClient();
			client.Credentials = credits;
			setText(text, client);
		}

		private void setText(string text, WebClient client)
		{
			try
			{
				Uri uri = getUri();
				byte[] utf8Data = Encoding.UTF8.GetBytes(text);
				client.UploadData(uri, utf8Data);
			}
			catch (Exception e)
			{
				throw new exception.CannotWriteToExternalFileException(
					String.Format("Could not write the text to plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		#endregion
	}
}
