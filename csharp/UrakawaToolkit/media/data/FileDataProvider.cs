using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	//TODO: Should there exist a DataProviderFactory?
	public class FileDataProvider : IDataProvider
	{
		protected internal FileDataProvider(IMediaDataManager fact)
		{
			mFactory = fact;
		}

		protected IMediaDataManager mFactory;

		private string mRelativeFilePath;

		/// <summary>
		/// Gets the full path of the file storing the data the instance
		/// </summary>
		/// <returns>The full path</returns>
		public string getFilePath()
		{
			return mRelativeFilePath;
		}

		#region IDataProvider Members

		/// <summary>
		/// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public System.IO.Stream getInputStream()
		{
			FileStream inputStream;
			string fp = getFilePath();
			try
			{
				inputStream = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not open file {0}", fp),
					e);
			}
			return inputStream;
		}

		public System.IO.Stream getOutputStream()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}



		IMediaDataManager IDataProvider.getMediaDataManager()
		{
			throw new Exception("The method or operation is not implemented.");
		}


		public void delete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataProvider copy()
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
