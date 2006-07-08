using System;
using System.Xml;

//TODO
//confirm namespace name
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
		public Project() : this(new urakawa.core.Presentation(), new MetadataFactory())
		{
		}

		public Project(urakawa.core.Presentation pres, MetadataFactory metaFact)
		{
			mPresentation = pres;
			mMetadata = new System.Collections.ArrayList();
			mMetadataFactory =metaFact; 
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
		/// <returns>A <see cref="bool"/> indicating if the XUK file was succesfully opened and loaded</returns>
		public bool openXUK(Uri fileUri)
		{
			mPresentation = new urakawa.core.Presentation();
			mMetadataFactory = new MetadataFactory();


			System.Xml.XmlTextReader source = new System.Xml.XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;
			bool foundXUK = false;
			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element && source.LocalName=="XUK")
				{
					foundXUK = true;
					break;
				}
				if (source.EOF) break;
			}
			if (!foundXUK) return false;
			bool foundError = false;
			bool foundPresentation = false;
			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					switch (source.LocalName)
					{
						case "ProjectMetadata":
							mMetadata = new System.Collections.ArrayList();
							if (!XUKinMetadata(source))
							{
								foundError = true;
							}
							break;
						case "Presentation":
							foundPresentation = true;
							mPresentation = new urakawa.core.Presentation();
							if (!mPresentation.XUKin(source)) foundError = true;
							break;
						default:
							foundError = true;
							break;
					}
				}
				else if (source.NodeType==XmlNodeType.EndElement) 
				{
					break;
				}
				if (source.EOF) break;
				if (foundError) break;
			}

			return foundPresentation && !foundError;
		}

		private bool XUKinMetadata(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			//if we are not at the opening tag of a core node element, return false
			if (!(source.LocalName == "ProjectMetadata" && source.NodeType == System.Xml.XmlNodeType.Element))
			{
				return false;
			}
      
			if (source.IsEmptyElement) return true;
			bool foundError = false;
			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					IMetadata newMeta = mMetadataFactory.createMetadata(source.LocalName);
					if (newMeta.XUKin(source)) 
					{
						mMetadata.Add(newMeta);
					}
					else
					{
						foundError = true;
					}
				}
				if (source.NodeType==XmlNodeType.EndElement)
				{
					break;
				}
				if (foundError) break;
				if (source.EOF) break;
			}

			return !foundError;
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

			writeBeginningOfFile(writer);
			writeMetadata(writer);

			bool didItWork = false;
			
			if (mPresentation != null)
				didItWork = mPresentation.XUKout(writer);

			writeEndOfFile(writer);

			return didItWork;
		}

		private void writeMetadata(XmlWriter writer)
		{
			foreach (IMetadata md in mMetadata)
			{
				md.XUKout(writer);
			}
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
				if (md.getName()==name) list.Add(md);
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
				if (md.getName()==name) mMetadata.Remove(md);
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


		
    
		private void writeBeginningOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("XUK");
			writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", 
				"http://www.w3.org/2001/XMLSchema-instance", "xuk.xsd");
			
		}

		private void writeEndOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Close();
		}

	}
}
