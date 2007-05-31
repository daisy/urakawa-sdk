using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.properties.channel;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa
{
	/// <summary>
	/// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
	/// </summary>
	public class Project : IXukAble, IValueEquatable<Project>
	{
		private IPresentation mPresentation;
		private List<IMetadata> mMetadata;
		private IMetadataFactory mMetadataFactory;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <remarks>
		/// Uses the current directory as basapath for the <see cref="urakawa.media.data.FileDataProviderManager"/>
		/// used as <see cref="urakawa.media.data.IDataProviderManager"/>
		/// </remarks>
		public Project()
			: this(new Uri(System.IO.Directory.GetCurrentDirectory()))
		{
		}

		/// <summary>
		/// Constructor using default a <see cref="Presentation"/> with the given base uri
		/// </summary>
		/// <param name="baseUri">The given base path</param>
		public Project(Uri baseUri)
			: this(new Presentation(baseUri), null)
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
		public Project(IPresentation pres, MetadataFactory metaFact)
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

			if (!source.ReadToFollowing("Xuk", urakawa.ToolkitSettings.XUK_NS)) return false;
			bool foundProject = false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						//If the element QName matches the Xuk QName equivalent of this, Xuk it in using this.XukIn
						if (source.LocalName == getXukLocalName() && source.NamespaceURI == getXukNamespaceUri())
						{
							foundProject = true;
							if (!XukIn(source)) return false;
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return foundProject;
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
			if (!XukOut(writer)) return false;
			writer.WriteEndElement();
			writer.WriteEndDocument();

			return true;
		}

		/// <summary>
		/// Gets the <see cref="urakawa.Presentation"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns></returns>
		public IPresentation getPresentation()
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
		/// Gets a <see cref="List{IMetadata}"/> of all <see cref="IMetadata"/>
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="List{IMetadata}"/> of metadata <see cref="IMetadata"/></returns>
		public List<IMetadata> getMetadataList()
		{
			return new List<IMetadata>(mMetadata);
		}

		/// <summary>
		/// Gets a <see cref="List{IMetadata}"/> of all <see cref="IMetadata"/>
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="List{IMetadata}"/> of <see cref="IMetadata"/></returns>
		public List<IMetadata> getMetadataList(string name)
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
			foreach (IMetadata md in getMetadataList(name))
			{
				deleteMetadata(md);
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

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Project"/> from a Project xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			getPresentation().getChannelsManager().clearChannels();
			foreach (IMetadata meta in getMetadataList())
			{
				this.deleteMetadata(meta);
			}
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a Project xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// No known attributes
			return true;
		}

		private bool XukInMetadata(XmlReader source)
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
		/// Reads a child of a Project xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.LocalName == "mMetadata" && source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				if (!XukInMetadata(source)) return false;
			}
			else if (source.LocalName == "mPresentation" && source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				bool foundPresentation = false;
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							if (!getPresentation().XukIn(source)) return false;
							foundPresentation = true;
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) break;
					}
				}
				if (!foundPresentation) return false;
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child element
			}
			return true;
		}

		/// <summary>
		/// Write a Project element to a XUK file representing the <see cref="Project"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a Project element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			//No attributes to Xuk out
			return true;
		}

		/// <summary>
		/// Writes the metadata of the project to a mMetadata xuk element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		private bool XukOutMetadata(XmlWriter destination)
		{
			destination.WriteStartElement("mMetadata", urakawa.ToolkitSettings.XUK_NS);
			foreach (IMetadata md in mMetadata)
			{
				if (!md.XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			return true;
		}


		/// <summary>
		/// Write the child elements of a Project element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			if (!XukOutMetadata(destination)) return false;
			if (getPresentation() == null) return false;
			destination.WriteStartElement("mPresentation", ToolkitSettings.XUK_NS);
			if (!getPresentation().XukOut(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Project"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Project"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion



		#region IValueEquatable<Project> Members

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(Project other)
		{
			if (!getPresentation().ValueEquals(other.getPresentation())) return false;
			List<IMetadata> thisMetadata = getMetadataList();
			List<IMetadata> otherMetadata = other.getMetadataList();
			if (thisMetadata.Count != otherMetadata.Count) return false;
			foreach (IMetadata m in thisMetadata)
			{
				bool found = false;
				foreach (IMetadata om in other.getMetadataList(m.getName()))
				{
					if (m.ValueEquals(om)) found = true;
				}
				if (!found) return false;
			}
			return true;
		}

		#endregion
	}
}
