using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	public class PlainTextMediaData : MediaData
	{

		private Encoding mEncoding = Encoding.UTF8;

		protected internal PlainTextMediaData(IMediaDataManager mngr)
		{
			setMediaDataManager(mngr);
		}

		/// <summary>
		/// Gets the <see cref="Encoding"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="Encoding"/></returns>
		public Encoding getEncoding()
		{
			return mEncoding;
		}

		/// <summary>
		/// Sets the <see cref="Encoding"/> of <c>this</c>
		/// </summary>
		/// <param name="enc">The new <see cref="Encoding"/> - can not be <c>null</c></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="enc"/> is <c>null</c></exception>
		public void setEncoding(Encoding enc)
		{
			if (enc == null)
			{
				throw new exception.MethodParameterIsNullException("The new encoding can not be null");
			}
			mEncoding = enc;
		}

		/// <summary>
		/// Gets the text of <c>this</c>
		/// </summary>
		/// <returns>The text</returns>
		public string getText()
		{
			StreamReader rd = new StreamReader(getDataProvider().getInputStream(), getEncoding());
			string text = rd.ReadToEnd();
			rd.Close();
			return text;
		}

		/// <summary>
		/// Sets the text of <c>this</c>
		/// </summary>
		/// <param name="newText">The text</param>
		public void setText(string newText)
		{
			Stream oStr = getDataProvider().getOutputStream();
			oStr.Seek(0, SeekOrigin.Begin);
			StreamWriter wr = new StreamWriter(oStr, getEncoding());
			wr.Write(newText);
			wr.Close();
		}

		#region IMediaData Members

		public IDataProvider getDataProvider()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		protected override IMediaData copyL()
		{
			return copy();
		}

		public PlainTextMediaData copy()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		#region IXukAble Members

		public override bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		#endregion

		public override void delete()
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		protected override IList<IDataProvider> getUsedDataProviders()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Determines if the value of <c>this</c> is equal to the value of another <see cref="IMediaData"/>.
		/// The value is consideres equal if the following is <c>true</c>
		/// <list type="ol">
		///	<item>The <paramref name="other"/> <see cref="IMediaData"/> is a <see cref="PlainTextMediaData"/></item>
		/// <item>The <see cref="Encoding"/> of <c>this</c> and <paramref name="other"/> have the same name</item>
		/// <item>The text of <c>this</c> and <paramref name="other"/> are equal</item>
		/// </list>
		/// </summary>
		/// <param name="other">The other <see cref="IMediaData"/></param>
		/// <returns><c>true</c> if the values of <c>this</c> and <paramref name="other"/> are equal</returns>
		public override bool ValueEquals(IMediaData other)
		{
			if (other is PlainTextMediaData)
			{
				PlainTextMediaData oPTMD = (PlainTextMediaData)other;
				if (getEncoding().EncodingName == oPTMD.getEncoding().EncodingName)
				{
					if (getText() == oPTMD.getText())
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
