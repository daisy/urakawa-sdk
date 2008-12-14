using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data
{

	/// <summary>
	/// Implementation of interface <see cref="IDataProvider"/> using files as data storage.
	/// <remarks>
	/// Note that the <see cref="IDataProviderManager"/> owning a <see cref="FileDataProvider"/> 
	/// must be a <see cref="FileDataProviderManager"/>. 
	/// Trying to initialize a <see cref="FileDataProvider"/> with a non-<see cref="FileDataProviderManager"/>
	/// implementation of <see cref="IDataProviderManager"/> 
	/// will cause a <see cref="exception.MethodParameterIsWrongTypeException"/>
	/// </remarks>
	/// </summary>
	public class FileDataProvider : XukAble, IDataProvider
	{


		#region Non-used CloseNotifyingFileStream protected member class
		///// <summary>
		///// A <see cref="FileStream"/> that notifies about it closing via the <see cref="CloseNotifyingFileStream.Closed"/> event
		///// </summary>
		//protected class CloseNotifyingFileStream : FileStream
		//{
		//  /// <summary>
		//  /// Initializes a new instance of the <see cref="CloseNotifyingFileStream"/> class with the specified path, 
		//  /// creation mode, read/write permission, and sharing permission. 
		//  /// </summary>
		//  /// <param name="path">
		//  /// A relative or absolute path for the file that the current  <see cref="CloseNotifyingFileStream"/> object will encapsulate
		//  /// </param>
		//  /// <param name="mode">
		//  /// A <see cref="FileMode"/> constant that determines how to open or create the file.'
		//  /// </param>
		//  /// <param name="access">A <see cref="FileAccess"/> constant 
		//  /// that determines how the file can be accessed by the <see cref="CloseNotifyingFileStream"/> object. 
		//  /// This gets the <see cref="Stream.CanRead"/> and <see cref="Stream.CanWrite"/> properties of the <see cref="CloseNotifyingFileStream"/> object. 
		//  /// <see cref="Stream.CanSeek"/>CanSeek is true if path specifies a disk file. 
		//  /// </param>
		//  /// <param name="share">
		//  /// A <see cref="FileShare"/> constant that determines how the file will be shared by processes.
		//  /// </param>
		//  public CloseNotifyingFileStream(string path, FileMode mode, FileAccess access, FileShare share)
		//    : base(path, mode, access, share)
		//  {

		//  }

		//  /// <summary>
		//  /// Event fired when the <see cref="Stream"/> has closed
		//  /// </summary>
		//  public event EventHandler Closed;

		//  /// <summary>
		//  /// Fires the <see cref="Closed"/> event
		//  /// </summary>
		//  private void FireClosed()
		//  {
		//    EventHandler h = Closed;
		//    if (h!=null) h(this, EventArgs.Empty);
		//  }

		//  /// <summary>
		//  /// Closes the stream
		//  /// </summary>
		//  public override void Close()
		//  {
		//    base.Close();
		//    FireClosed();
		//  }
		//}
		#endregion

		/// <summary>
		/// Constructs a new file data provider with a given manager and relative path
		/// </summary>
		/// <param name="mngr">The manager of the constructed instance</param>
		/// <param name="relPath">The relative path of the data file of the constructed instance</param>
		/// <param name="mimeType">The MIME type of the data to store in the constructed instance</param>
		protected internal FileDataProvider(FileDataProviderManager mngr, string relPath, string mimeType)
		{
			setDataProviderManager(mngr);
			mDataFileRelativePath = relPath;
			mMimeType = mimeType;
		}

		private FileDataProviderManager mManager;

		private string mDataFileRelativePath;

		List<utilities.CloseNotifyingStream> mOpenInputStreams = new List<utilities.CloseNotifyingStream>();

		utilities.CloseNotifyingStream mOpenOutputStream = null;

		/// <summary>
		/// Gets the path of the file storing the data of the instance, realtive to the path of data file directory
		/// of the owning <see cref="FileDataProviderManager"/>
		/// </summary>
		/// <returns></returns>
		public string getDataFileRelativePath()
		{
			return mDataFileRelativePath;
		}

		/// <summary>
		/// Gets the full path of the file storing the data the instance
		/// </summary>
		/// <returns>The full path</returns>
		public string getDataFileFullPath()
		{
			return Path.Combine(mManager.getDataFileDirectoryFullPath(), mDataFileRelativePath);
		}

		#region IDataProvider Members

		private bool hasBeenInitialized = false;

		/// <summary>
		/// Gets the UID of the data provider in the context of the manager. 
		/// Convenience for <c>getDataProviderManager().getUidOfDataProvider(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		public string getUid()
		{
			return getDataProviderManager().getUidOfDataProvider(this);
		}

		private void checkDataFile()
		{
			string dirPath = Path.GetDirectoryName(getDataFileFullPath());
			if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
			if (File.Exists(getDataFileFullPath()))
			{
				if (!hasBeenInitialized)
				{
					File.Delete(getDataFileFullPath());
				}
				else
				{
					return;
				}
			}
			if (hasBeenInitialized)
			{
				throw new exception.DataMissingException(
					String.Format("The data file {0} does not exist", getDataFileFullPath()));
			}
			try
			{
				File.Create(getDataFileFullPath()).Close();
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not create data file {0}: {1}", getDataFileFullPath(), e.Message),
					e);
			}
			hasBeenInitialized = true;
		}

		/// <summary>
		/// Gets an input <see cref="Stream"/> providing read access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The input stream</returns>
		/// <exception cref="exception.DataMissingException">
		/// Thrown if the data stored in the <see cref="IDataProvider"/> is missing from the underlying storage mechanism
		/// </exception>
		/// <exception cref="exception.OutputStreamOpenException">
		/// Thrown if an output <see cref="Stream"/> from the data provider is already/still open
		/// </exception>
		/// <exception cref="exception.OperationNotValidException">
		/// Thrown if the underlying data file could not be opened in read-mode - see inner <see cref="Exception"/> for datails of cause
		/// </exception>
		public Stream getInputStream()
		{
			if (mOpenOutputStream != null)
			{
				throw new exception.OutputStreamOpenException(
					"Cannot open an input Stream while an output Stream is open");
			}
			FileStream inputFS;
			string fp = getDataFileFullPath();
			checkDataFile();
			try
			{
				inputFS = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not open file {0}", fp),
					e);
			}
			utilities.CloseNotifyingStream res = new urakawa.media.data.utilities.CloseNotifyingStream(inputFS);
			res.StreamClosed += new EventHandler(InputStreamClosed_StreamClosed);
			mOpenInputStreams.Add(res);
			return res;
		}

		void InputStreamClosed_StreamClosed(object sender, EventArgs e)
		{
			utilities.CloseNotifyingStream cnStm = sender as utilities.CloseNotifyingStream;
			if (cnStm!=null)
			{
				if (mOpenInputStreams.Contains(cnStm)) mOpenInputStreams.Remove(cnStm);
				cnStm.StreamClosed -= new EventHandler(InputStreamClosed_StreamClosed);
                cnStm = null;
			}
		}

		/// <summary>
		/// Gets an output <see cref="Stream"/> providing write access to the <see cref="FileDataProvider"/>
		/// </summary>
		/// <returns>The ouput stream</returns>
		/// <exception cref="exception.DataMissingException">
		/// Thrown if the data stored in the <see cref="IDataProvider"/> is missing from the underlying storage mechanism
		/// </exception>
		/// <exception cref="exception.InputStreamsOpenException">
		/// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
		/// </exception>
		/// <exception cref="exception.OutputStreamOpenException">
		/// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
		/// </exception>
		/// <exception cref="exception.OperationNotValidException">
		/// Thrown if the underlying data file could not be opened in write-mode - see inner <see cref="Exception"/> for datails of cause
		/// </exception>
		public Stream getOutputStream()
		{
			FileStream outputFS;
			if (mOpenOutputStream != null)
			{
				throw new exception.OutputStreamOpenException(
					"Cannot open an output Stream while another output Stream is already open");
			}
			if (mOpenInputStreams.Count > 0)
			{
				throw new exception.InputStreamsOpenException(
					"Cannot open an output Stream while one or more input Streams are open");
			}
			checkDataFile();
			string fp = getDataFileFullPath();
			try
			{
				outputFS = new FileStream(fp, FileMode.Open, FileAccess.Write, FileShare.Read);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not open file {0}", fp),
					e);
			}
			mOpenOutputStream = new urakawa.media.data.utilities.CloseNotifyingStream(outputFS);
			mOpenOutputStream.StreamClosed += new EventHandler(OutputStream_StreamClosed);
			return mOpenOutputStream;
		}

		void OutputStream_StreamClosed(object sender, EventArgs e)
		{
            if (Type.ReferenceEquals(sender, mOpenOutputStream))
            {
                mOpenOutputStream.StreamClosed -= new EventHandler(OutputStream_StreamClosed);
                mOpenOutputStream = null;
            }
		}

		/// <summary>
		/// Deletes the file data provider, including any existing data files. Also detaches it self 
		/// the owning data provider manager
		/// </summary>
		/// <exception cref="exception.OutputStreamOpenException">
		/// Thrown if a output <see cref="Stream"/> from the <see cref="IDataProvider"/> is currently open
		/// </exception>
		/// <exception cref="exception.InputStreamsOpenException">
		/// Thrown if one or more input <see cref="Stream"/>s from the <see cref="IDataProvider"/> are currently open
		/// </exception>
		/// <exception cref="exception.OperationNotValidException">
		/// Thrown if an exception occurs while deleting the data file of <c>this</c>. 
		/// The occuring exception can be accessed as the inner exception of the thrown exception.
		/// </exception>
		public void delete()
		{
			if (mOpenOutputStream != null)
			{
				throw new exception.OutputStreamOpenException(
					"Cannot delete the FileDataProvider while an output Stream is still open");
			}
			if (mOpenInputStreams.Count > 0)
			{
				throw new exception.InputStreamsOpenException(
					"Cannot delete the FileDataProvider while one or more input Streams are still open");
			}
			if (File.Exists(getDataFileFullPath()))
			{
				try
				{
					File.Delete(getDataFileFullPath());
				}
				catch (Exception e)
				{
					throw new exception.OperationNotValidException(String.Format(
						"Could not delete data file {0}: {1}",
						getDataFileFullPath(), e.Message), e);
				}
			}
			getDataProviderManager().removeDataProvider(this, false);
		}

		/// <summary>
		/// Copies the file data provider including the data 
		/// </summary>
		/// <returns>The copy</returns>
		public IDataProvider copy()
		{
			IDataProvider c = getDataProviderManager().getDataProviderFactory().createDataProvider(
				getMimeType(), getXukLocalName(), getXukNamespaceUri());
			if (c == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The data provider factory can not create a data provider matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			Stream thisData = getInputStream();
			try
			{
				FileDataProviderManager.appendDataToProvider(thisData, (int)(thisData.Length - thisData.Position), c);
			}
			finally
			{
				thisData.Close();
			}
			return c;
		}

		IDataProviderManager IDataProvider.getDataProviderManager()
		{
			return getDataProviderManager();
		}

		/// <summary>
		/// Gets the <see cref="FileDataProviderManager"/> managing <c>this</c>
		/// </summary>
		/// <returns>The manager</returns>
		public FileDataProviderManager getDataProviderManager()
		{
			return mManager;
		}

		private string mMimeType;

		/// <summary>
		/// Gets the MIME type of the media stored in the data provider
		/// </summary>
		/// <returns>The MIME type</returns>
		public string getMimeType()
		{
			return mMimeType;
		}

		void IDataProvider.setDataProviderManager(IDataProviderManager mngr)
		{
			FileDataProviderManager fMngr = mngr as FileDataProviderManager;
			if (fMngr == null)
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The IDataProviderManager of a FileDataProvider must be a FileDataProviderManager");
			}
			setDataProviderManager(fMngr);
		}

		/// <summary>
		/// Initializes the <see cref="IDataProvider"/> with an owning <see cref="IDataProviderManager"/>,
		/// adding it to the manager
		/// </summary>
		/// <param name="mngr">The owning manager</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IDataProvider"/>
		/// </exception>
		/// <remarks>
		/// This method should only be called during construction, calling this method at a later stage will cause
		/// a <exception cref="exception.IsAlreadyInitializedException"/>
		/// </remarks>
		public void setDataProviderManager(FileDataProviderManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The FileDataProvider can be initialized with a null manager");
			}
			if (mManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The FileDataProvider has already been initialized with an owning manager");
			}
			mManager = mngr;
			mManager.addDataProvider(this);
		}

		#endregion

		#region IXukAble Members

		/// <summary>
		/// Reads the attributes of a FileDataProvider xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			mDataFileRelativePath = source.GetAttribute("dataFileRelativePath");
			if (mDataFileRelativePath == null || mDataFileRelativePath == "")
			{
				throw new exception.XukException("dataFileRelativePath is missing from FileDataProvider element");
			}
            /*
            if (!File.Exists(getDataFileFullPath()))
            {
                throw new exception.DataMissingException(
    String.Format("The data file {0} does not exist", getDataFileFullPath()));
            }
             */
			hasBeenInitialized = true;//Assume that the data file exists
			mMimeType = source.GetAttribute("mimeType");
            base.xukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a FileDataProvider element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			checkDataFile();//Ensure that data file exist even if no data has yet been written to it.
			destination.WriteAttributeString("dataFileRelativePath", getDataFileRelativePath());
			destination.WriteAttributeString("mimeType", getMimeType());
            base.xukOutAttributes(destination, baseUri);
		}
		#endregion

		#region IValueEquatable<IDataProvider> Members

		/// <summary>
		/// Determines if the 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool valueEquals(IDataProvider other)
		{
			if (other == null) return false;
			if (GetType() != other.GetType()) return false;
			FileDataProvider o = (FileDataProvider)other;
			if (o.getMimeType() != getMimeType()) return false;
			if (!FileDataProviderManager.compareDataProviderContent(this, o)) return false;
			return true;
		}

		#endregion

		#region IDataProvider Members


		/// <summary>
		/// Exports <c>this</c> to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination <see cref="Presentation"/></param>
		/// <returns>The exported <see cref="FileDataProvider"/></returns>
		public FileDataProvider export(Presentation destPres)
		{
			FileDataProvider expFDP = destPres.getDataProviderFactory().createDataProvider(
				getMimeType(), getXukLocalName(), getXukNamespaceUri()) as FileDataProvider;
			if (expFDP == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The DataProviderFactory of the destination Presentation can notcreate a FileDataProviderManager for XUK QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			Stream thisStm = getInputStream();
			try
			{
				FileDataProviderManager.appendDataToProvider(thisStm, (int)thisStm.Length, expFDP);
			}
			finally
			{
				thisStm.Close();
			}
			return expFDP;
		}


		IDataProvider IDataProvider.export(Presentation destPres)
		{
			return export(destPres);
		}

		#endregion
	}
}
