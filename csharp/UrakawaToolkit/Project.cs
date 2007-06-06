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
		private Presentation mPresentation;
		private List<Metadata> mMetadata;
		private MetadataFactory mMetadataFactory;

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
		public Project(Presentation pres, MetadataFactory metaFact)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException("The Presentation of the Project can not be null");
			}
			mPresentation = pres;
			mMetadata = new List<Metadata>();
			if (metaFact==null) metaFact = new MetadataFactory();
			mMetadataFactory = metaFact;
		}

		/// <summary>
		/// Retrieves the <see cref="MetadataFactory"/> creating <see cref="Metadata"/> 
		/// for the <see cref="Project"/> instance
		/// </summary>
		/// <returns></returns>
		public MetadataFactory getMetadataFactory()
		{
			return mMetadataFactory;
		}



		/// <summary>
		/// Opens an XUK file and loads the project from this
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the source XUK file</param>
		public void openXUK(Uri fileUri)
		{
			XmlTextReader source = new XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = WhitespaceHandling.Significant;
			try
			{
				openXUK(source);
			}
			finally
			{
				source.Close();
			}
		}

		/// <summary>
		/// Opens the <see cref="Project"/> from an <see cref="XmlReader"/>
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the source <see cref="XmlReader"/> is null</exception>
		public void openXUK(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("The source XmlReader is null");
			}

			if (!source.ReadToFollowing("Xuk", urakawa.ToolkitSettings.XUK_NS))
			{
				throw new exception.XukException("Could not find Xuk element in Project Xuk file");
			}
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
							XukIn(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
			if (!foundProject)
			{
				throw new exception.XukException("Found no Project in Xuk file");
			}
		}

		/// <summary>
		/// Saves the <see cref="Project"/> to a XUK file
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
		public void saveXUK(Uri fileUri)
		{
			//@todo
			//we should probably track the file encoding in the future
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(fileUri.LocalPath, System.Text.UnicodeEncoding.UTF8);
			writer.Indentation = 1;
			writer.IndentChar = ' ';
			writer.Formatting = System.Xml.Formatting.Indented;
			try
			{
				saveXUK(writer);
			}
			finally
			{
				writer.Close();
			}
		}

		/// <summary>
		/// Saves the project to XUK via. a <see cref="XmlWriter"/>
		/// </summary>
		/// <param name="writer">The destination <see cref="XmlWriter"/></param>
		public void saveXUK(XmlWriter writer)
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
			XukOut(writer);
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		/// <summary>
		/// Gets the <see cref="urakawa.Presentation"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns></returns>
		public Presentation getPresentation()
		{
			return mPresentation;
		}

		/// <summary>
		/// Appends a <see cref="Metadata"/> to the <see cref="Project"/>
		/// </summary>
		/// <param name="metadata">The <see cref="Metadata"/> to add</param>
		public void appendMetadata(Metadata metadata)
		{
			mMetadata.Add(metadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="List{Metadata}"/> of metadata <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList()
		{
			return new List<Metadata>(mMetadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="List{Metadata}"/> of <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList(string name)
		{
			List<Metadata> list = new List<Metadata>();
			foreach (Metadata md in mMetadata)
			{
				if (md.getName() == name) list.Add(md);
			}
			return list;
		}

		/// <summary>
		/// Deletes all <see cref="Metadata"/>s with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		public void deleteMetadata(string name)
		{
			foreach (Metadata md in getMetadataList(name))
			{
				deleteMetadata(md);
			}
		}

		/// <summary>
		/// Deletes a given <see cref="Metadata"/>
		/// </summary>
		/// <param name="metadata">The given <see cref="Metadata"/></param>
		public void deleteMetadata(Metadata metadata)
		{
			mMetadata.Remove(metadata);
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Project"/> from a Project xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read Project from a non-element node");
			}
			try
			{
				getPresentation().getChannelsManager().clearChannels();
				foreach (Metadata meta in getMetadataList())
				{
					this.deleteMetadata(meta);
				}
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of Project: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Project xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			// No known attributes

		}

		private void XukInMetadata(XmlReader source)
		{
			if (source.IsEmptyElement) return;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					Metadata newMeta = mMetadataFactory.createMetadata(source.LocalName, source.NamespaceURI);
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
						newMeta.XukIn(source);
						mMetadata.Add(newMeta);
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF)
				{
					throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}


		/// <summary>
		/// Reads a child of a Project xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.LocalName == "mMetadata" && source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				XukInMetadata(source);
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
							getPresentation().XukIn(source);
							foundPresentation = true;
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}
				if (!foundPresentation)
				{
					throw new exception.XukException("Found no Presentation inside Project element");
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child element
			}
		}

		/// <summary>
		/// Write a Project element to a XUK file representing the <see cref="Project"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of Project: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Project element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Writes the metadata of the project to a mMetadata xuk element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		private void XukOutMetadata(XmlWriter destination)
		{
			destination.WriteStartElement("mMetadata", urakawa.ToolkitSettings.XUK_NS);
			foreach (Metadata md in mMetadata)
			{
				md.XukOut(destination);
			}
			destination.WriteEndElement();
		}


		/// <summary>
		/// Write the child elements of a Project element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			XukOutMetadata(destination);
			if (getPresentation() == null)
			{
				throw new exception.XukException("Presentation of Project is null");
			}
			destination.WriteStartElement("mPresentation", ToolkitSettings.XUK_NS);
			getPresentation().XukOut(destination);
			destination.WriteEndElement();
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
			List<Metadata> thisMetadata = getMetadataList();
			List<Metadata> otherMetadata = other.getMetadataList();
			if (thisMetadata.Count != otherMetadata.Count) return false;
			foreach (Metadata m in thisMetadata)
			{
				bool found = false;
				foreach (Metadata om in other.getMetadataList(m.getName()))
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
