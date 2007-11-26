using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.property.channel;
using urakawa.metadata;
using urakawa.xuk;
using urakawa.events;

namespace urakawa
{
	/// <summary>
	/// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
	/// </summary>
	public class Project : XukAble, IValueEquatable<Project>, IChangeNotifier
	{
		
		#region Event related members

		/// <summary>
		/// Event fired after the <see cref="Project"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		public event EventHandler<DataModelChangedEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(DataModelChangedEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangedEventArgs> d = changed;
			if (d != null) d(this, args);
		}
		/// <summary>
		/// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
		/// </summary>
		public event EventHandler<PresentationAddedEventArgs> presentationAdded;
		/// <summary>
		/// Fires the <see cref="presentationAdded"/> event
		/// </summary>
		/// <param name="source">
		/// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
		/// </param>
		/// <param name="addedPres">The <see cref="Presentation"/> that was added</param>
		protected void notifyPresentationAdded(Project source, Presentation addedPres)
		{
			EventHandler<PresentationAddedEventArgs> d = presentationAdded;
			if (d != null) d(this, new PresentationAddedEventArgs(source, addedPres));
		}

		void this_presentationAdded(object sender, PresentationAddedEventArgs e)
		{
			notifyChanged(e);
			e.AddedPresentation.changed += new EventHandler<DataModelChangedEventArgs>(Presentation_changed);
		}

		void Presentation_changed(object sender, DataModelChangedEventArgs e)
		{
			notifyChanged(e);
		}

		/// <summary>
		/// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
		/// </summary>
		public event EventHandler<PresentationRemovedEventArgs> presentationRemoved;
		/// <summary>
		/// Fires the <see cref="presentationRemoved"/> event
		/// </summary>
		/// <param name="source">
		/// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
		/// </param>
		/// <param name="removedPres">The <see cref="Presentation"/> that was added</param>
		protected void notifyPresentationRemoved(Project source, Presentation removedPres)
		{
			EventHandler<PresentationRemovedEventArgs> d = presentationRemoved;
			if (d != null) d(this, new PresentationRemovedEventArgs(source, removedPres));
		}

		void this_presentationRemoved(object sender, PresentationRemovedEventArgs e)
		{
			e.RemovedPresentation.changed -= new EventHandler<DataModelChangedEventArgs>(Presentation_changed);
			notifyChanged(e);
		}
		#endregion


		private List<Presentation> mPresentations;


		/// <summary>
		/// Default constructor
		/// </summary>
		public Project()
		{
			mPresentations = new List<Presentation>();
			this.presentationAdded += new EventHandler<PresentationAddedEventArgs>(this_presentationAdded);
			this.presentationRemoved += new EventHandler<PresentationRemovedEventArgs>(this_presentationRemoved);
		}

		private DataModelFactory mDataModelFactory;

		/// <summary>
		/// Gets the <see cref="DataModelFactory"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns>The factory</returns>
		/// <remarks>
		/// The <see cref="DataModelFactory"/> of a <see cref="Project"/> is initialized lazily,
		/// in that if the <see cref="DataModelFactory"/> has not been explicitly initialized
		/// using the <see cref="setDataModelFactory"/>, then calling <see cref="getDataModelFactory"/>
		/// will initialize the <see cref="Project"/> with a newly created <see cref="DataModelFactory"/>.
		/// </remarks>
		public DataModelFactory getDataModelFactory()
		{
			if (mDataModelFactory == null) mDataModelFactory = new DataModelFactory();
			return mDataModelFactory;
		}

		/// <summary>
		/// Initializes the <see cref="Project"/> with a <see cref="DataModelFactory"/>
		/// </summary>
		/// <param name="fact">The factory with which to initialize - must not be <c>null</c></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Project"/> has already been initialized with a <see cref="DataModelFactory"/>
		/// </exception>
		public void setDataModelFactory(DataModelFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The DataModelFactory can not be null");
			}
			if (mDataModelFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Project has already been initialized with a DataModelFactory");
			}
			mDataModelFactory = fact;
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
						//If the element QName matches the Xuk QName equivalent of this, Xuk it in using this.xukIn
						if (source.LocalName == getXukLocalName() && source.NamespaceURI == getXukNamespaceUri())
						{
							foundProject = true;
							xukIn(source);
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
				saveXUK(writer, fileUri);
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
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void saveXUK(XmlWriter writer, Uri baseUri)
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
			xukOut(writer, baseUri);
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}



		/// <summary>
		/// Gets the <see cref="urakawa.Presentation"/> of the <see cref="Project"/> at a given index
		/// </summary>
		/// <param name="index">The index of the <see cref="Presentation"/> to get</param>
		/// <returns>The presentation at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()-1]</c>
		/// </exception>
		public Presentation getPresentation(int index)
		{
			if (index<0 || getNumberOfPresentations()<=index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
					index, getNumberOfPresentations()-1));
			}
			return mPresentations[index];
		}

		/// <summary>
		/// Gets a list of the <see cref="Presentation"/>s in the <see cref="Project"/>
		/// </summary>
		/// <returns>The list</returns>
		public List<Presentation> getListOfPresentations()
		{
			return new List<Presentation>(mPresentations);
		}

		/// <summary>
		/// Sets the <see cref="Presentation"/> at a given index
		/// </summary>
		/// <param name="newPres">The <see cref="Presentation"/> to set</param>
		/// <param name="index">The given index</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newPres"/> is <c>null</c></exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()-1]</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when <paramref name="newPres"/> already exists in <c>this</c> with another <paramref name="index"/>
		/// </exception>
		public void setPresentation(Presentation newPres, int index)
		{
			if (newPres == null)
			{
				throw new exception.MethodParameterIsNullException("The new Presentation can not be null");
			}
			if (index < 0 || getNumberOfPresentations() < index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
					index, getNumberOfPresentations()));
			}
			if (mPresentations.Contains(newPres))
			{
				if (mPresentations.IndexOf(newPres) != index)
				{
					throw new exception.IsAlreadyManagerOfException(String.Format(
						"The new Presentation already exists in the Project at index {0:0}",
						mPresentations.IndexOf(newPres)));
				}
			}
			if (index < getNumberOfPresentations())
			{
				removePresentation(index);
				mPresentations.Insert(index, newPres);
			}
			else
			{
				mPresentations.Add(newPres);
			}
			newPres.setProject(this);
			notifyPresentationAdded(this, newPres);
		}

		/// <summary>
		/// Adds a <see cref="Presentation"/> to the <see cref="Project"/>
		/// </summary>
		/// <param name="newPres">The <see cref="Presentation"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newPres"/> is <c>null</c></exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when <paramref name="newPres"/> already exists in <c>this</c>
		/// </exception>
		public void addPresentation(Presentation newPres)
		{
			setPresentation(newPres, getNumberOfPresentations());
		}

		/// <summary>
		/// Adds a newly created <see cref="Presentation"/> to the <see cref="Project"/>,
		/// as returned by <c><see cref="getDataModelFactory"/>().<see cref="DataModelFactory.createPresentation()"/>()</c>
		/// </summary>
		/// <returns>The newly created and added <see cref="Presentation"/></returns>
		public Presentation addNewPresentation()
		{
			Presentation newPres = getDataModelFactory().createPresentation();
			addPresentation(newPres);
			return newPres;
		}

		/// <summary>
		/// Gets the number of <see cref="Presentation"/>s in the <see cref="Project"/>
		/// </summary>
		/// <returns>The number of <see cref="Presentation"/>s</returns>
		public int getNumberOfPresentations()
		{
			return mPresentations.Count;
		}

		/// <summary>
		/// Removes the <see cref="Presentation"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The removed <see cref="Presentation"/></returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()-1]</c>
		/// </exception>
		public Presentation removePresentation(int index)
		{
			if (index < 0 || getNumberOfPresentations() <= index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
					index, getNumberOfPresentations() - 1));
			}
			Presentation pres = getPresentation(index);
			mPresentations.RemoveAt(index);
			notifyPresentationRemoved(this, pres);
			return pres;
		}

		/// <summary>
		/// Removes all <see cref="Presentation"/>s from the <see cref="Project"/>
		/// </summary>
		public void removeAllPresentations()
		{
			mPresentations.Clear();
		}

		
		#region IXUKAble members

		protected override void clear()
		{
			removeAllPresentations();
			base.clear();
		}

		/// <summary>
		/// Reads a child of a Project xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				switch (source.LocalName)
				{
					case "mPresentations":
						xukInPresentations(source);
						readItem = true;
						break;
				}
			}
			if (!readItem) base.xukInChild(source);
		}

		private void xukInPresentations(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						Presentation pres = getDataModelFactory().createPresentation(
							source.LocalName, source.NamespaceURI);
						if (pres != null)
						{
							this.addPresentation(pres);
							pres.xukIn(source);
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
		}

		/// <summary>
		/// Write the child elements of a Project element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			base.xukOutChildren(destination, baseUri);
			destination.WriteStartElement("mPresentations", ToolkitSettings.XUK_NS);
			foreach (Presentation pres in getListOfPresentations())
			{
				pres.xukOut(destination, baseUri);
			}
			destination.WriteEndElement();
		}

		#endregion



		#region IValueEquatable<Project> Members

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool valueEquals(Project other)
		{
			if (other==null) return false;
			if (GetType()!=other.GetType()) return false;
			if (getNumberOfPresentations()!=other.getNumberOfPresentations()) return false;
			for (int index=0; index<getNumberOfPresentations(); index++)
			{
				if (!getPresentation(index).valueEquals(other.getPresentation(index))) return false;
			}
			return true;
		}

		#endregion
	}
}
