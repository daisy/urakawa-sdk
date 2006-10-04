using	System;
using	System.Collections.Generic;
using	System.Xml;

namespace	urakawa.core
{
	///	<summary>
	///	Implementation of	<see cref="CoreNode"/> interface
	///	</summary>
	public class CoreNode : TreeNode, ICoreNode
	{
		Dictionary<System.Type, IProperty> mProperties; 

		///	<summary>
		///	Compares two <see	cref="CoreNode"/>s to	see	if they	are	equal	
		///	(they	can	belong to	different	<see cref="IPresentation"/>s and still be	equal)
		///	</summary>
		///	<param name="cn1">The	first	<see cref="CoreNode"/></param>
		///	<param name="cn2">The	second <see	cref="CoreNode"/></param>
		///	<param name="testDeep">A <see	cref="bool"/>	indicating if	the	test should	be deep,
		///	ie.	if child nodes should	also be	tested for equality</param>
		///	<returns></returns>
		public static	bool areCoreNodesEqual(CoreNode	cn1, CoreNode	cn2, bool	testDeep)
		{
			Type[] t1s = cn1.getUsedPropertyTypes();
			Type[] t2s = cn1.getUsedPropertyTypes();
			if (t1s.Length!=cn2.getUsedPropertyTypes().Length) return false;
			foreach (Type t1 in t1s)
			{
				bool found = false;
				foreach (Type t2 in t2s)
				{
					if (t1 == t2)
					{
						found = true;
						break;
					}
				}
				if (!found) return false;
			}
			IChannelsProperty	chp1 = (IChannelsProperty)cn1.getProperty(typeof(ChannelsProperty));
			IChannelsProperty	chp2 = (IChannelsProperty)cn2.getProperty(typeof(ChannelsProperty));
			if (chp1!=null &&	chp2!=null)
			{
				System.Collections.IList chs1	=	chp1.getListOfUsedChannels();
				System.Collections.IList chs2	=	chp2.getListOfUsedChannels();
				if (chs1.Count!=chs2.Count)	return false;
				for	(int chIndex=0;	chIndex<chs1.Count;	chIndex++)
				{
					IChannel ch1 = (IChannel)chs1[chIndex];
					IChannel ch2 = (IChannel)chs2[chIndex];
					if (ch1.getName()!=ch2.getName())	return false;
					urakawa.media.IMedia m1	=	chp1.getMedia(ch1);
					urakawa.media.IMedia m2	=	chp2.getMedia(ch2);
					if ((m1!=null)!=(m2!=null))	return false;
					if (m1!=null)
					{
						if (m1.getType()!=m2.getType())	return false;
						if (
							m1.GetType().IsSubclassOf(typeof(urakawa.media.IClippedMedia))
							&& m1.GetType().IsSubclassOf(typeof(urakawa.media.IClippedMedia)))
						{
							urakawa.media.IClippedMedia	cm1	=	(urakawa.media.IClippedMedia)m1;
							urakawa.media.IClippedMedia	cm2	=	(urakawa.media.IClippedMedia)m2;
							if (
								typeof(urakawa.media.Time).IsAssignableFrom(cm1.getDuration().GetType())
								&& typeof(urakawa.media.Time).IsAssignableFrom(cm2.getDuration().GetType()))
							{
								if (
									((urakawa.media.Time)cm1.getDuration()).getTime()
									!=((urakawa.media.Time)cm2.getDuration()).getTime())
								{
									return false;
								}
							}

						}
						if (
							m1.GetType().IsSubclassOf(typeof(urakawa.media.IImageSize))
							&& m1.GetType().IsSubclassOf(typeof(urakawa.media.IImageSize)))
						{
							urakawa.media.IImageSize ism1	=	(urakawa.media.IImageSize)m1;
							urakawa.media.IImageSize ism2	=	(urakawa.media.IImageSize)m2;
							if (ism1.getHeight()!=ism2.getHeight())	return false;
							if (ism1.getWidth()!=ism2.getWidth())	return false;
						}
					}
				}
			}
			IXmlProperty xp1 = (IXmlProperty)cn1.getProperty(typeof(XmlProperty));
			IXmlProperty xp2 = (IXmlProperty)cn2.getProperty(typeof(XmlProperty));
			if (xp1!=null	&& xp2!=null)
			{
				if (xp1.getName()!=xp2.getName())	return false;
				if (xp1.getNamespace()!=xp2.getNamespace())	return false;
				IList<IXmlAttribute>	xp1Attrs = xp1.getListOfAttributes();
				IList<IXmlAttribute> xp2Attrs = xp2.getListOfAttributes();
				if (xp1Attrs.Count!=xp2Attrs.Count)	return false;
				foreach	(IXmlAttribute attr1 in	xp1.getListOfAttributes())
				{
					IXmlAttribute	attr2	=	xp2.getAttribute(attr1.getName(),	attr1.getNamespace());
					if (attr2==null) return	false;
					if (attr1.getValue()!=attr2.getValue())	return false;
				}
				if (cn1.getChildCount()!=cn2.getChildCount())	return false;
				if (testDeep)
				{
					for	(int index=0;	index<cn1.getChildCount(); index++)
					{
						IBasicTreeNode ch1 = cn1.getChild(index);
						IBasicTreeNode ch2 = cn1.getChild(index);
						if (!typeof(CoreNode).IsAssignableFrom(ch1.GetType())) return	false;
						if (!typeof(CoreNode).IsAssignableFrom(ch2.GetType())) return	false;
						if (!areCoreNodesEqual((CoreNode)ch1,	(CoreNode)ch2, true))	return false;
					}
				}
			}
			return true;
		}

		
		///	<summary>
		///	The	owner	<see cref="Presentation"/>
		///	</summary>
		private	Presentation	mPresentation;


		///	<summary>
		///	Constructor	setting	the	owner	<see cref="Presentation"/>
		///	</summary>
		///	<param name="pres">The presentation of the constructed <see cref="CoreNode"/></param>
		protected internal CoreNode(Presentation	pres)
		{
			mPresentation = pres;
			mProperties = new Dictionary<System.Type, IProperty>();
		}

		/// <summary>
		/// Gets the child <see cref="CoreNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="CoreNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="BasicTreeNode.getChildCount"/>()-1</c></exception>
		public new CoreNode getChild(int index)
		{
			return (CoreNode)base.getChild(index);
		}

		#region	ICoreNode	Members

		/// <summary>
		/// Gets an array of the <see cref="Type"/>s of <see cref="IProperty"/> set for the <see cref="ICoreNode"/>
		/// </summary>
		/// <returns>The array</returns>
		public Type[] getUsedPropertyTypes()
		{
			Type[] usedTypes = new Type[mProperties.Values.Count];
			mProperties.Keys.CopyTo(usedTypes, 0);
			return usedTypes;
		}

		IPresentation ICoreNode.getPresentation()
		{
			return getPresentation();
		}

		///	<summary>
		///	Gets the <see	cref="Presentation"/>	owning the <see	cref="ICoreNode"/>
		///	</summary>
		///	<returns>The owner</returns>
		public Presentation getPresentation()
		{
			return mPresentation;
		}

		///	<summary>
		///	Gets the <see	cref="IProperty"/> of	the	given	<see cref="Type"/>
		///	</summary>
		///	<param name="propType">The given <see	cref="Type"/></param>
		///	<returns>The <see	cref="IProperty"/> of	the	given	<see cref="Type"/>,
		///	<c>null</c>	if no	property of	the	given	<see cref="Type"/> has been	set</returns>
		public IProperty getProperty(Type	propType)
		{
			if (!mProperties.ContainsKey(propType)) return null;
			return mProperties[propType];
		}

		///	<summary>
		///	Sets a <see	cref="IProperty"/>,	possible overwriting previously	set	<see cref="IProperty"/>
		///	of the same	<see cref="Type"/>
		///	</summary>
		///	<param name="prop">The <see	cref="IProperty"/> to	set. 
		///	If <c>null</c> is	passed,	an <see	cref="exception.MethodParameterIsNullException"/>	is thrown</param>
		///	<returns>A <see	cref="bool"/>	indicating if	a	previously set <see	cref="IProperty"/>
		///	was	overwritten
		///	</returns>
		public bool	setProperty(IProperty	prop)
		{
			if (prop==null)	throw	new	exception.MethodParameterIsNullException("No PropertyType	was	given");
			bool overwrt = mProperties.ContainsKey(prop.GetType());
			mProperties[prop.GetType()] = prop;
			prop.setOwner(this);
			return overwrt;
		}

		///	<summary>
		///	Remove a property	from the node's	properties array
		///	</summary>
		///	<param name="propType">Specify the type	of property	to remove</param>
		///	<returns>The property	which	was	just removed,	or null	if it	did	not	exist</returns>
		public IProperty removeProperty(Type propType)
		{
			IProperty	removedProperty	=	null;
			if (mProperties.ContainsKey(propType))
			{
				removedProperty = mProperties[propType];
				mProperties.Remove(propType);
				removedProperty.setOwner(null);
			}
			return removedProperty;
		}

		///	<summary>
		///	Make a copy	of the node.	The	copy has the same	presentation and no	parent.
		///	</summary>
		///	<param name="deep">If	true,	then include the node's	entire subtree.	 
		///	Otherwise, just	copy the node	itself.</param>
		///	<returns>A <see	cref="CoreNode"/>	containing the copied	data.</returns>
		public CoreNode	copy(bool	deep)
		{
			CoreNode theCopy = (CoreNode)this.getPresentation().getCoreNodeFactory().createNode();

			//copy the properties
			copyProperties(theCopy);
			
			//copy the children
			if (deep)
			{
				copyChildren(theCopy);
			}

			return theCopy;
		}

		/// <summary>
		/// Copies the <see cref="IProperty"/>s of the current instance to a given destination <see cref="CoreNode"/>
		/// </summary>
		/// <param name="destinationNode">The destination <see cref="CoreNode"/></param>
		protected void copyProperties(CoreNode destinationNode)
		{
			foreach (IProperty prop in mProperties.Values)
			{
				destinationNode.setProperty(prop.copy());
			}
		}

		/// <summary>
		/// Copies the children of the current instance to a given destination <see cref="CoreNode"/>
		/// </summary>
		/// <param name="destinationNode">The destination <see cref="CoreNode"/></param>
		/// <remarks>The children are copied deep and any existing children of the destination <see cref="CoreNode"/>
		/// are not removed</remarks>
		protected void copyChildren(CoreNode destinationNode)
		{
			for (int i = 0; i < this.getChildCount(); i++)
			{
				destinationNode.appendChild(getChild(i).copy(true));
			}
		}

		#endregion

		#region	IVisitableCoreNode Members

		///	<summary>
		///	Accept a <see	cref="ICoreNodeVisitor"/>	in depth first mode
		///	</summary>
		///	<param name="visitor">The	<see cref="ICoreNodeVisitor"/></param>
		public void	acceptDepthFirst(ICoreNodeVisitor	visitor)
		{
			preVisitDelegate preVisit = new preVisitDelegate(visitor.preVisit);
			postVisitDelegate postVisit = new postVisitDelegate(visitor.postVisit);
			visitDepthFirst(preVisit, postVisit);
		}

		///	<summary>
		///	Accept a <see	cref="ICoreNodeVisitor"/>	in breadth first mode
		///	</summary>
		///	<param name="visitor">The	<see cref="ICoreNodeVisitor"/></param>
		///	<remarks>HACK: Not yet implemented,	does nothing!!!!</remarks>
		public void	acceptBreadthFirst(ICoreNodeVisitor	visitor)
		{
			throw new Exception("The method or operation is not implemented.");
		}


		/// <summary>
		/// Visits the <see cref="IVisitableCoreNode"/> depth first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate</param>
		/// <param name="postVisit">The post visit delegate</param>
		public void visitDepthFirst(preVisitDelegate preVisit, postVisitDelegate postVisit)
		{
			bool visitChildren = true;
			if (preVisit != null)
			{
				if (!preVisit(this)) visitChildren = false;
			}
			if (visitChildren)
			{
				for (int i = 0; i < getChildCount(); i++)
				{
					((ICoreNode)getChild(i)).visitDepthFirst(preVisit, postVisit);
				}
			}
			if (postVisit != null) postVisit(this);
		}


		#endregion

		#region	IXUKAble members 

		/// <summary>
		/// Reads the attributes of the CoreNode xml element (there are in fact none)
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully read</returns>
		/// <remarks>This method is intended to be overridden in custom <see cref="CoreNode"/> implementations</remarks>
		protected virtual bool XUKInAttributes(XmlReader source)
		{
			return true;
		}

		///	<summary>
		///	Reads	the	<see cref="CoreNode"/> instance	from a CoreNode	xml	element
		///	<list	type="table">
		///	<item>
		///	<term>Entry	state</term>
		///	<description>
		///	The	cursor of	<paramref	name="source"/>	must be	at the start of	the	CoreNode element
		///	</description>
		///	</item>
		///	<item>
		///	<term>Exit state</term>
		///	</item>
		///	<description>
		///	The	cursor of	 <paramref name="source"/> must	be at	the	end	of the CoreNode	element
		///	</description>
		///	</list>
		///	</summary>
		///	<param name="source">The <see	cref="XmlReader"/> from	which	to read	the	core node</param>
		///	<returns>A <see	cref="bool"/>	indicating if	the	properties were	succesfully	read</returns>
		///	<exception cref="exception.MethodParameterIsNullException">
		///	Thrown when	<paramref	name="source"/>	is null
		///	</exception>
		public bool	XUKIn(System.Xml.XmlReader source)
		{
			if (source ==	null)
			{
				throw	new	exception.MethodParameterIsNullException("Xml	Reader is	null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;

			if (!XUKInAttributes(source)) return false;

			bool bFoundError = false;

			while	(source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS && source.LocalName=="mProperties")
					{
						if (!XUKInProperties(source)) return false;
					}
					else if (source.NamespaceURI == urakawa.ToolkitSettings.XUK_NS && source.LocalName=="mChildren")
					{
						if (!XUKInChildren(source)) return false;
					}
					else
					{
						if (!source.IsEmptyElement)
						{
							//Read past unidentified element
							source.ReadSubtree().Close();
						}
					}
				}
				else if	(source.NodeType==XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF)	break;
				if (bFoundError) break;
			}

			return !bFoundError;
		}

		///	<summary>
		///	Helper function	to read	in the properties	and	invoke their respective	XUKIn	methods. 
		///	Reads	the	<see cref="IProperty"/>s of	the	<see cref="CoreNode"/> instance	from a mProperties xml element
		///	<list	type="table">
		///	<item>
		///	<term>Entry	state</term>
		///	<description>
		///	The	cursor of	<paramref	name="source"/>	must be	at the start of	the	mProperties	element
		///	</description>
		///	</item>
		///	<item>
		///	<term>Exit state</term>
		///	</item>
		///	<description>
		///	The	cursor of	 <paramref name="source"/> must	be at	the	end	of the mProperties element
		///	</description>
		///	</list>
		///	</summary>
		///	<remarks>If	the	mProperties	element	is empty,	the	start	and	the	end	of of	it are the nsame positions</remarks>
		///	<param name="source">The <see	cref="XmlReader"/> from	which	to read	the	properties</param>
		///	<returns>A <see	cref="bool"/>	indicating if	the	properties were	succesfully	read</returns>
		///	<exception cref="exception.MethodParameterIsNullException">
		///	Thrown when	the	<paramref	name="source"/>	<see cref="XmlReader"/>	is null
		///	</exception>
		protected bool XUKInProperties(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml	Reader is	null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "mProperties") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			if (source.IsEmptyElement) return true;

			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IProperty newProp = getPresentation().getPropertyFactory().createProperty(source.LocalName, source.NamespaceURI);
					if (newProp != null)
					{
						if (!newProp.XUKIn(source)) return false;
						setProperty(newProp);
					}
					else if (!source.IsEmptyElement)
					{
						//Reads sub tree and places cursor at end element
						source.ReadSubtree().Close();
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
		/// Reads the children of the <see cref="CoreNode"/> from a mChildren XUK element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the children were succesfully read</returns>
		protected bool XUKInChildren(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Source XmlReader is null");
			}
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					CoreNode newChild = getPresentation().getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
					if (newChild != null)
					{
						if (!newChild.XUKIn(source)) return false;
						appendChild(newChild);
					}
					else if (!source.IsEmptyElement)
					{
						//Read past unidentified element
						source.ReadSubtree().Close();
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
		/// Writes the attributes of the CoreNode element representing the instance (there are currently no attributes)
		/// </summary>
		/// <param name="wr">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully written</returns>
		/// <remarks>This method is intended to be overridden in custom <see cref="CoreNode"/> implementations</remarks>
		protected virtual bool XUKOutAttributes(XmlWriter wr)
		{
			return true;
		}

		///	<summary>
		///	Writes the CoreNode	element	to a XUK file	representing the <see	cref="CoreNode"/>	instance
		///	</summary>
		///	<param name="destination">The	destination	<see cref="XmlWriter"/></param>
		///	<returns>A <see	cref="bool"/>	indicating the write was succesful</returns>
		public bool	XUKOut(System.Xml.XmlWriter	destination)
		{
			if (destination	== null)
			{
				throw	new	exception.MethodParameterIsNullException("Xml	Writer is	null");
			}
			destination.WriteStartElement(getLocalName(), getNamespaceURI());
			XUKOutAttributes(destination);
			destination.WriteStartElement("mProperties", urakawa.ToolkitSettings.XUK_NS);
			foreach (IProperty prop in mProperties.Values)
			{
				if (!prop.XUKOut(destination)) return false;
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mChildren", urakawa.ToolkitSettings.XUK_NS);
			for	(int i = 0;	i<this.getChildCount();	i++)
			{
				if (!getChild(i).XUKOut(destination)) return false;
			}
			destination.WriteEndElement();
			destination.WriteEndElement();
			return true;
		}
		#endregion

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="CoreNode"/> in XUK
		/// </summary>
		/// <returns>The local name part</returns>
		protected virtual string getLocalName()
		{
			return "CoreNode";
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="CoreNode"/> in XUK
		/// </summary>
		/// <returns>The namespace uri part</returns>
		protected virtual string getNamespaceURI()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

	}
}
