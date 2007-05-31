using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.core.events;
using urakawa.core.property;

namespace urakawa.core
{
	/// <summary>
	/// Core presentation consisting of a <see cref="ICoreNodeFactory"/>, a <see cref="ICorePropertyFactory"/>
	/// and a root <see cref="CoreNode"/>
	/// </summary>
	public class CorePresentation : ICorePresentation
	{
		private CoreNode mRootNode;
		private ICoreNodeFactory mCoreNodeFactory;
		private ICorePropertyFactory mPropertyFactory;

		/// <summary>
		/// Constructor constructing a <see cref="CorePresentation"/> with given 
		/// <see cref="ICoreNodeFactory"/> and <see cref="ICorePropertyFactory"/>
		/// </summary>
		/// <param name="coreNodeFact">
		/// The given <see cref="ICoreNodeFactory"/>. 
		/// If this parameter is <c>null</c>, a new <see cref="CoreNodeFactory"/> 
		/// is created for the presentation
		/// </param>
		/// <param name="propFact">
		/// The given <see cref="ICorePropertyFactory"/>.
		/// If this parameter is <c>null</c>, a new <see cref="CorePropertyFactory"/> 
		/// is created for the presentation
		/// </param>
		public CorePresentation(ICoreNodeFactory coreNodeFact, ICorePropertyFactory propFact)
		{
			if (coreNodeFact == null) coreNodeFact = new CoreNodeFactory();
			mCoreNodeFactory = coreNodeFact;
			mCoreNodeFactory.setPresentation(this);
			if (propFact == null) propFact = new CorePropertyFactory();
			mPropertyFactory = propFact;
			mRootNode = new CoreNode(this);
		}


		#region ICorePresentation Members

		/// <summary>
		/// Gets the root <see cref="CoreNode"/> of the <see cref="CorePresentation"/>
		/// </summary>
		/// <returns>The root <see cref="CoreNode"/></returns>
		public CoreNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="CoreNode"/> of the <see cref="CorePresentation"/>
		/// </summary>
		/// <param name="newRoot">The new root <see cref="CoreNode"/></param>
		public void setRootNode(CoreNode newRoot)
		{
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="ICoreNodeFactory"/>
		/// creating <see cref="CoreNode"/>s for the <see cref="CorePresentation"/>
		/// </summary>
		/// <returns>The <see cref="ICoreNodeFactory"/></returns>
		public ICoreNodeFactory getCoreNodeFactory()
		{
			return mCoreNodeFactory;
		}

		/// <summary>
		/// Gets the <see cref="ICorePropertyFactory"/> associated with the <see cref="CorePresentation"/>
		/// </summary>
		/// <returns>The <see cref="ICorePropertyFactory"/></returns>
		public ICorePropertyFactory getPropertyFactory()
		{
			return mPropertyFactory;
		}

		#endregion

		#region IXukAble Members

		/// <summary>
		/// Handles a Xuk child during <see cref="XukIn"/>
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <remarks>A <see cref="bool"/> indicating if the child was succesfully handled</remarks>
		protected virtual bool HandleXukChild(XmlReader source)
		{
			bool handled = false;
			if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (source.LocalName)
				{
					case "mRootNode":
						if (!XukInRootNode(source)) return false;
						break;
				}
			}
			if (!handled)
			{
				if (!source.IsEmptyElement) source.ReadSubtree().Close();
			}
			return true;
		}

		private bool XukInRootNode(System.Xml.XmlReader source)
		{
			if (source.IsEmptyElement) return false;
			bool foundRootNode = false;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					CoreNode newRootNode = getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
					if (newRootNode == null) return false;
					if (!newRootNode.XukIn(source)) return false;
					mRootNode = newRootNode;
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return !foundRootNode;
		}


		/// <summary>
		/// Reads the <see cref="CorePresentation"/> instance from a CorePresentation element in a XUK file.
		/// <list type="table">
		/// <item>
		/// <term>Entry state</term>
		/// <description>
		/// The cursor of <paramref localName="source"/> must be at the start of the CorePresentation element
		/// </description>
		/// </item>
		/// <item>
		/// <term>Exit state</term>
		/// </item>
		/// <description>
		/// The cursor of  <paramref localName="source"/> must be at the end of the CorePresentation element
		/// </description>
		/// </list>
		/// </summary>
		/// <param name="source">The <see cref="XmlReader"/> from which to read the CorePresentation element</param>
		/// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
		public virtual bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("XML Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "Presentation") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					if (!HandleXukChild(source)) return false;
				}
				else if (source.NodeType==XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return (mRootNode != null);
		}

		/// <summary>
		/// Write a CorePresentation element to a XUK file representing the <see cref="CorePresentation"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public virtual bool XukOut(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteStartElement("mRootNode", urakawa.ToolkitSettings.XUK_NS);
			if (!mRootNode.XukOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Does nothing and always returns <c>true</c>.
		/// In derived classes this method should be overwritten to write any additional Xuk elements needed
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAdditionalChildren(System.Xml.XmlWriter destination)
		{
			return true;
		}


		/// <summary>
		/// Gets the local localName part of the QName identifying the type of the instance
		/// </summary>
		/// <returns>The local localName</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName identifying the type of the instance
		/// </summary>
		/// <returns>The namespace uri</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region ICoreNodeChangedEventManager Members

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is changed, i.e. added or removed 
		/// as the child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		public void notifyCoreNodeChanged(CoreNode changedNode)
		{
			CoreNodeChangedEventHandler d = coreNodeChanged;//Copy to local variable to make thread safe
			if (d != null) d(this, new CoreNodeChangedEventArgs(changedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeAddedEventHandler coreNodeAdded;

		/// <summary>
		/// Fires the <see cref="coreNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		public void notifyCoreNodeAdded(CoreNode addedNode)
		{
			CoreNodeAddedEventHandler d = coreNodeAdded;//Copy to local variable to make thread safe
			if (d != null) d(this, new CoreNodeAddedEventArgs(addedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeRemovedEventHandler coreNodeRemoved;

		/// <summary>
		/// Fires the <see cref="coreNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		public void notifyCoreNodeRemoved(CoreNode removedNode, CoreNode formerParent, int formerPosition)
		{
			CoreNodeRemovedEventHandler d = coreNodeRemoved;
			if (d != null) d(this, new CoreNodeRemovedEventArgs(removedNode, formerParent, formerPosition));
			notifyCoreNodeChanged(removedNode);
		}

		#endregion
	}
}
