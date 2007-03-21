using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.properties.channel;
using urakawa.metadata;

namespace urakawa
{
	/// <summary>
	/// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
	/// </summary>
	public class Project
	{
		private urakawa.Presentation mPresentation;
		private IList<IMetadata> mMetadata;
		private IMetadataFactory mMetadataFactory;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <remarks>
		/// Uses the current directory as basapath for the <see cref="urakawa.media.data.FileDataProviderManager"/>
		/// used as <see cref="urakawa.media.data.IDataProviderManager"/>
		/// </remarks>
		public Project()
			: this(System.IO.Directory.GetCurrentDirectory(), null)
		{
		}

		/// <summary>
		/// Constructor using a <see cref="Presentation"/> with the given base path and data directory 
		/// and a <see cref="MetadataFactory"/>
		/// </summary>
		/// <param name="basePath">The given base path</param>
		/// <param name="dataDir">The given data directory</param>
		public Project(string basePath, string dataDir)
			: this(new Presentation(basePath, dataDir), null)
		{
		}

		/// <summary>
		/// Constructor which initializes the project with a presentation
		/// and metadata factory.
		/// </summary>
		/// <param name="pres">The presentation object</param>
		/// <param name="metaFact">
		/// The metadata factory - if <c>null</c> a newly creates <see cref="MetadataFactory"/> is used
		/// </param>
		public Project(urakawa.Presentation pres, MetadataFactory metaFact)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException("The Presentation of the Project can not be null");
			}
			mPresentation = pres;
			mMetadata = new List<IMetadata>();
			if (metaFact==null) metaFact = new MetadataFactory();
			mMetadataFactory = metaFact;
		}

		/// <summary>
		/// Retrieves the <see cref="IMetadataFactory"/> creating <see cref="IMetadata"/> 
		/// for the <see cref="Project"/> instance
		/// </summary>
		/// <returns></returns>
		public IMetadataFactory getMetadataFactory()
		{
			return mMetadataFactory;
		}



		/// <summary>
		/// Opens an XUK file and loads the project from this
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the source XUK file</param>
		/// <returns>A <see cref="bool"/> indicating if the XUK 
		/// file was succesfully opened and loaded</returns>
		public bool openXUK(Uri fileUri)
		{
			XmlTextReader source = new XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = WhitespaceHandling.Significant;
			bool success = openXUK(source);
			source.Close();
			return success;
		}

		/// <summary>
		/// Opens the <see cref="Project"/> from an <see cref="XmlReader"/>
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="Project"/> 
		/// was succesfully opened</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the source <see cref="XmlReader"/> is null</exception>
		public bool openXUK(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("The source XmlReader is null");
			}
			IChannelsManager chMgr = getPresentation().getChannelsManager();
			foreach (IChannel ch in chMgr.getListOfChannels())
			{
				chMgr.removeChannel(ch);
			}
			foreach (IMetadata meta in getMetadataList())
			{
				this.deleteMetadata(meta);
			}

			if (!source.ReadToFollowing("Xuk", urakawa.ToolkitSettings.XUK_NS)) return false;
			bool foundPresentation = false;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					bool processedElement = false;
					if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS)
					{
						switch (source.LocalName)
						{
							case "ProjectMetadata":
								if (!XUKInMetadata(source)) return false;
								processedElement = true;
								break;
							case "Presentation":
								foundPresentation = true;
								if (!mPresentation.XukIn(source)) return false;
								processedElement = true;
								break;
							default:
								break;
						}
					}
					if (!processedElement)
					{
						if (!source.IsEmptyElement)
						{
							//Read past unidentified element
							source.ReadSubtree().Close();
						}
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}

			return foundPresentation;
		}

		private bool XUKInMetadata(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;

			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IMetadata newMeta = mMetadataFactory.createMetadata(source.LocalName, source.NamespaceURI);
					if (newMeta == null)
					{
						if (!source.IsEmptyElement)
						{
							//Read past unidentified element
							source.ReadSubtree().Close();
						}
					}
					else
					{
						if (!newMeta.XukIn(source)) return true;
						mMetadata.Add(newMeta);
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}

			return true;
		}

		/// <summary>
		/// Saves the <see cref="Project"/> to a XUK file
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="Project"/> was succesfully saved to XUK</returns>
		public bool saveXUK(Uri fileUri)
		{
			//@todo
			//we should probably track the file encoding in the future
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(fileUri.LocalPath, System.Text.UnicodeEncoding.UTF8);
			writer.Indentation = 1;
			writer.IndentChar = ' ';
			writer.Formatting = System.Xml.Formatting.Indented;
			bool success = saveXUK(writer);
			writer.Close();
			return success;
		}

		/// <summary>
		/// Saves the project to XUK via. a <see cref="XmlWriter"/>
		/// </summary>
		/// <param name="writer">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="Project"/> was succesfully saved to XUK</returns>
		public bool saveXUK(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("Xuk", urakawa.ToolkitSettings.XUK_NS);
			if (urakawa.ToolkitSettings.XUK_XSD_PATH != String.Empty)
			{
				if (urakawa.ToolkitSettings.XUK_NS == String.Empty)
				{
					writer.WriteAttributeString(
						"xsi", "noNamespaceSchemaLocation", 
						"http://www.w3.org/2001/XMLSchema-instance", 
						urakawa.ToolkitSettings.XUK_XSD_PATH);
				}
				else
				{
					writer.WriteAttributeString(
						"xsi", 
						"noNamespaceSchemaLocation",
						"http://www.w3.org/2001/XMLSchema-instance", 
						String.Format("{0} {1}", urakawa.ToolkitSettings.XUK_NS, urakawa.ToolkitSettings.XUK_XSD_PATH));
				}
			}
			writer.WriteStartElement("ProjectMetadata", urakawa.ToolkitSettings.XUK_NS);
			foreach (IMetadata md in mMetadata)
			{
				md.XukOut(writer);
			}
			writer.WriteEndElement();
			if (mPresentation != null)
			{
				if (!mPresentation.XukOut(writer)) return false;
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();

			return true;
		}

		/// <summary>
		/// Gets the <see cref="urakawa.Presentation"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns></returns>
		public urakawa.Presentation getPresentation()
		{
			return mPresentation;
		}

		/// <summary>
		/// Appends a <see cref="IMetadata"/> to the <see cref="Project"/>
		/// </summary>
		/// <param name="metadata">The <see cref="IMetadata"/> to add</param>
		public void appendMetadata(IMetadata metadata)
		{
			mMetadata.Add(metadata);
		}

		/// <summary>
		/// Gets a <see cref="IList{IMetadata}"/> of all <see cref="IMetadata"/>
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="IList{IMetadata}"/> of metadata <see cref="IMetadata"/></returns>
		public IList<IMetadata> getMetadataList()
		{
			return new List<IMetadata>(mMetadata);
		}

		/// <summary>
		/// Gets a <see cref="IList{IMetadata}"/> of all <see cref="IMetadata"/>
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="IList{IMetadata}"/> of <see cref="IMetadata"/></returns>
		public IList<IMetadata> getMetadataList(string name)
		{
			List<IMetadata> list = new List<IMetadata>();
			foreach (IMetadata md in mMetadata)
			{
				if (md.getName() == name) list.Add(md);
			}
			return list;
		}

		/// <summary>
		/// Deletes all <see cref="IMetadata"/>s with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		public void deleteMetadata(string name)
		{
			foreach (IMetadata md in mMetadata)
			{
				if (md.getName() == name) mMetadata.Remove(md);
			}
		}

		/// <summary>
		/// Deletes a given <see cref="IMetadata"/>
		/// </summary>
		/// <param name="metadata">The given <see cref="IMetadata"/></param>
		public void deleteMetadata(IMetadata metadata)
		{
			mMetadata.Remove(metadata);
		}





	}
}
