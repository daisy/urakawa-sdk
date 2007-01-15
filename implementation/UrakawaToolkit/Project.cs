using System;
using System.Xml;

namespace urakawa.project
{
	/// <summary>
	/// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
	/// </summary>
	public class Project
	{
		private urakawa.core.Presentation mPresentation;
		private System.Collections.IList mMetadata;
		private IMetadataFactory mMetadataFactory;

		/// <summary>
		/// Default constructor
		/// </summary>
		public Project()
			: this(null, null)
		{
		}

		/// <summary>
		/// Constructor which initializes the project with a presentation
		/// and metadata factory.
		/// </summary>
		/// <param name="pres">The presentation object</param>
		/// <param name="metaFact">The metadata factory</param>
		public Project(urakawa.core.Presentation pres, MetadataFactory metaFact)
		{
			if (pres == null) pres = new urakawa.core.Presentation();
			mPresentation = pres;
			mMetadata = new System.Collections.ArrayList();
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
			System.Xml.XmlTextReader source = new System.Xml.XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;
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
		public bool openXUK(XmlReader source)
		{
			mPresentation.getChannelsManager().removeAllChannels();
			mPresentation.setRootNode(mPresentation.getCoreNodeFactory().createNode());
			if (!source.ReadToFollowing("XUK", urakawa.ToolkitSettings.XUK_NS)) return false;
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
								mMetadata = new System.Collections.ArrayList();
								if (!XUKInMetadata(source)) return false;
								processedElement = true;
								break;
							case "Presentation":
								foundPresentation = true;
								if (!mPresentation.XUKIn(source)) return false;
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
			if (source.Name != "ProjectMetadata") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

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
					{
						if (!newMeta.XUKIn(source)) return true;
						mMetadata.Add(newMeta);
					}
				}
				if (source.NodeType == XmlNodeType.EndElement)
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
			writer.WriteStartElement("XUK", urakawa.ToolkitSettings.XUK_NS);
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
				md.XUKOut(writer);
			}
			writer.WriteEndElement();
			if (mPresentation != null)
			{
				if (!mPresentation.XUKOut(writer)) return false;
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();

			return true;
		}

		/// <summary>
		/// Gets the <see cref="urakawa.core.Presentation"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns></returns>
		public urakawa.core.Presentation getPresentation()
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
		/// Gets a <see cref="System.Collections.IList"/> of all metadata <see cref="object"/>s
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="System.Collections.IList"/> of metadata <see cref="object"/>s</returns>
		public System.Collections.IList getMetadataList()
		{
			return mMetadata;
		}

		/// <summary>
		/// Gets a <see cref="System.Collections.IList"/> of all metadata <see cref="object"/>s
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="System.Collections.IList"/> of metadata <see cref="object"/>s</returns>
		public System.Collections.IList getMetadataList(string name)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();
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
