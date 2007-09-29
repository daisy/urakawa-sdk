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
		/// <summary>
		/// Constructor setting the <see cref="IMediaFactory"/> that created the instance
		/// </summary>
		/// <param name="fact">The creating instance</param>
		protected internal ExternalTextMedia(IMediaFactory fact) : base(fact)
		{
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
		protected override ExternalMedia copyProtected()
		{
			return export(getMediaFactory().getPresentation());
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
		protected override ExternalMedia exportProtected(Presentation destPres)
		{
			ExternalTextMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as ExternalTextMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalTextMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}

			exported.setSrc(destPres.getRootUri().MakeRelativeUri(getUri()).ToString());
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

		private Uri getUri()
		{
			return new Uri(getMediaFactory().getPresentation().getRootUri(), getSrc());
		}

		/// <summary>
		/// Gets the text of the <c>this</c>
		/// </summary>
		/// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
		public string getText()
		{
			try
			{
				Uri src = getUri();
				WebClient client = new WebClient();
				client.UseDefaultCredentials = true;
				StreamReader rd = new StreamReader(client.OpenRead(src));
				string res = rd.ReadToEnd();
				rd.Close();
				return res;
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could read the text from plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		/// <summary>
		/// Sets the text of <c>this</c>
		/// </summary>
		/// <param name="text">The new text</param>
		/// <exception cref="exception.OperationNotValidException">
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
				Uri src = new Uri(getMediaFactory().getPresentation().getRootUri(), getSrc());
				byte[] utf8Data = Encoding.UTF8.GetBytes(text);
				client.UploadData(src, utf8Data);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not write the text to plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		#endregion
	}
}
