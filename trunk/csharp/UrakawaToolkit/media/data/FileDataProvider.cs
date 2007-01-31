using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	public class FileDataProvider : IDataProvider
	{
		protected internal FileDataProvider()
		{
		}

		private string mRelativeFilePath;

		/// <summary>
		/// Gets the full path of the file storing the data the instance
		/// </summary>
		/// <returns>The full path</returns>
		public string getFilePath()
		{
			return mFilePath;
		}

		#region IDataProvider Members

		/// <summary>
		/// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public System.IO.Stream getInputStream()
		{
			FileStream inputStream;
			try
			{
				inputStream = new FileStream(getFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception e)
			{
			}
			return inputStream;
		}

		public System.IO.Stream getOutputStream()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}



		IMediaDataManager IDataProvider.getManager()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IXukAble Members

		public bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
		}


		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		public virtual string getXukNamespaceUri()
		{
			return ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}
