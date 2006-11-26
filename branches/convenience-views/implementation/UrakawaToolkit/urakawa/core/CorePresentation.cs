using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.core.property;

namespace urakawa.core
{
	/// <summary>
	/// Core presentation consisting of a <see cref="ICoreNodeFactory"/>, a <see cref="ICorePropertyFactory"/>
	/// and a root <see cref="ICoreNode"/>
	/// </summary>
	public class CorePresentation : ICorePresentation
	{
		private ICoreNode mRootNode;
		private ICoreNodeFactory mCoreNodeFactory;
		private ICorePropertyFactory mPropertyFactory;

		/// <summary>
		/// Constructor constructing a <see cref="CorePresentation"/> with given 
		/// <see cref="ICoreNodeFactory"/> and <see cref="ICorePropertyFactory"/>
		/// </summary>
		/// <param localName="coreNodeFact">
		/// The given <see cref="ICoreNodeFactory"/>. 
		/// If this parameter is <c>null</c>, a new <see cref="CoreNodeFactory"/> 
		/// is created for the presentation
		/// </param>
		/// <param localName="propFact">
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
		/// Gets the root <see cref="ICoreNode"/> of the <see cref="CorePresentation"/>
		/// </summary>
		/// <returns>The root <see cref="ICoreNode"/></returns>
		public ICoreNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="ICoreNode"/> of the <see cref="CorePresentation"/>
		/// </summary>
		/// <param localName="newRoot">The new root <see cref="ICoreNode"/></param>
		public void setRootNode(ICoreNode newRoot)
		{
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="ICoreNodeFactory"/>
		/// creating <see cref="ICoreNode"/>s for the <see cref="CorePresentation"/>
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
		/// <param localName="source">The source <see cref="XmlReader"/></param>
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
					ICoreNode newRootNode = getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
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
		/// <param localName="source">The <see cref="XmlReader"/> from which to read the CorePresentation element</param>
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
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
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
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
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
	}
}
