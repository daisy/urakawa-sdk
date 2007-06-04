using	System;
using	System.Collections.Generic;
using	System.Xml;
using urakawa.core.visitor;
using urakawa.core.property;
using urakawa.properties.channel;
using urakawa.properties.xml;
using urakawa.xuk;

namespace	urakawa.core
{
	///	<summary>
	///	Implementation of	<see cref="TreeNode"/> interface
	///	</summary>
	public class TreeNode : ITreeNodeReadOnlyMethods, ITreeNodeWriteOnlyMethods, IVisitableTreeNode, IXukAble, IValueEquatable<TreeNode>
	{

		/// <summary>
		/// A <see cref="Dictionary{Type, Property}"/> storing the <see cref="Property"/>s of <c>this</c>
		/// </summary>
		Dictionary<Type, Property> mProperties;
		
		///	<summary>
		///	The	owner	<see cref="ITreePresentation"/>
		///	</summary>
		private	Presentation mPresentation;

		/// <summary>
    /// Contains the children of the node
    /// </summary>
    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="TreeNode"/>s</remarks>
    private List<TreeNode> mChildren;

    /// <summary>
    /// The parent <see cref="TreeNode"/>
    /// </summary>
    private TreeNode mParent;


		///	<summary>
		///	Constructor	setting	the	owner	<see cref="ITreePresentation"/>
		///	</summary>
		///	<param name="pres">The presentation of the constructed <see cref="TreeNode"/></param>
		protected internal TreeNode(Presentation pres)
		{
			mPresentation = pres;
			mProperties = new Dictionary<System.Type, Property>();
			mChildren = new List<TreeNode>();
		}

		#region	TreeNode	Members

		/// <summary>
		/// Gets an array of the <see cref="Type"/>s of <see cref="Property"/> set for the <see cref="TreeNode"/>
		/// </summary>
		/// <returns>The array</returns>
		public Type[] getListOfUsedPropertyTypes()
		{
			Type[] usedTypes = new Type[mProperties.Values.Count];
			mProperties.Keys.CopyTo(usedTypes, 0);
			return usedTypes;
		}


		///	<summary>
		///	Gets the <see	cref="ITreePresentation"/>	owning the <see	cref="TreeNode"/>
		///	</summary>
		///	<returns>The owner</returns>
		public Presentation getPresentation()
		{
			return mPresentation;
		}

		///	<summary>
		///	Gets the <see	cref="Property"/> of	the	given	<see cref="Type"/>
		///	</summary>
		///	<param name="propType">The given <see	cref="Type"/></param>
		///	<returns>The <see	cref="Property"/> of	the	given	<see cref="Type"/>,
		///	<c>null</c>	if no	property of	the	given	<see cref="Type"/> has been	set</returns>
		public Property getProperty(Type	propType)
		{
			if (!mProperties.ContainsKey(propType)) return null;
			return mProperties[propType];
		}

		///	<summary>
		///	Sets a <see	cref="Property"/>,	possible overwriting previously	set	<see cref="Property"/>
		///	of the same	<see cref="Type"/>
		///	</summary>
		///	<param name="prop">The <see	cref="Property"/> to	set. 
		///	If <c>null</c> is	passed,	an <see	cref="exception.MethodParameterIsNullException"/>	is thrown</param>
		///	<returns>A <see	cref="bool"/>	indicating if	a	previously set <see	cref="Property"/>
		///	was	overwritten
		///	</returns>
		public bool	setProperty(Property	prop)
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
		public Property removeProperty(Type propType)
		{
			Property	removedProperty	=	null;
			if (mProperties.ContainsKey(propType))
			{
				removedProperty = mProperties[propType];
				mProperties.Remove(propType);
				removedProperty.setOwner(null);
			}
			return removedProperty;
		}

		#endregion

		/// <summary>
		/// Copies the children of the current instance to a given destination <see cref="TreeNode"/>
		/// </summary>
		/// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
		/// <remarks>The children are copied deep and any existing children of the destination <see cref="TreeNode"/>
		/// are not removed</remarks>
		protected void copyChildren(TreeNode destinationNode)
		{
			for (int i = 0; i < this.getChildCount(); i++)
			{
				destinationNode.appendChild(getChild(i).copy(true));
			}
		}

		#region	IVisitableTreeNode Members

		///	<summary>
		///	Accept a <see	cref="ITreeNodeVisitor"/>	in depth first mode.
		///	</summary>
		///	<param name="visitor">The	<see cref="ITreeNodeVisitor"/></param>
		///	<remarks>
		/// Remark that only <see cref="ITreeNodeVisitor.preVisit"/> is executed during breadth-first tree traversal,
		/// since there is no notion of post in breadth first traversal
		///	</remarks>
		public void	acceptDepthFirst(ITreeNodeVisitor	visitor)
		{
			PreVisitDelegate preVisit = new PreVisitDelegate(visitor.preVisit);
			PostVisitDelegate postVisit = new PostVisitDelegate(visitor.postVisit);
			acceptDepthFirst(preVisit, postVisit);
		}

		///	<summary>
		///	Accept a <see	cref="ITreeNodeVisitor"/>	in breadth first mode
		///	</summary>
		///	<param name="visitor">The	<see cref="ITreeNodeVisitor"/></param>
		///	<remarks>HACK: Not yet implemented,	does nothing!!!!</remarks>
		public void	acceptBreadthFirst(ITreeNodeVisitor	visitor)
		{
			PreVisitDelegate preVisit = new PreVisitDelegate(visitor.preVisit);
			acceptBreadthFirst(preVisit);
		}


		/// <summary>
		/// Visits the <see cref="IVisitableTreeNode"/> depth-first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate - may be null</param>
		/// <param name="postVisit">The post visit delegate - may be null</param>
		public void acceptDepthFirst(PreVisitDelegate preVisit, PostVisitDelegate postVisit)
		{
			//If both preVisit and postVisit delegates are null, there is nothing to do.
			if (preVisit == null && postVisit == null) return;
			bool visitChildren = true;
			if (preVisit != null)
			{
				if (!preVisit(this)) visitChildren = false;
			}
			if (visitChildren)
			{
				for (int i = 0; i < getChildCount(); i++)
				{
					getChild(i).acceptDepthFirst(preVisit, postVisit);
				}
			}
			if (postVisit != null) postVisit(this);
		}


		/// <summary>
		/// Visits the <see cref="IVisitableTreeNode"/> breadth-first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate - may be null</param>
		public void acceptBreadthFirst(PreVisitDelegate preVisit)
		{
			if (preVisit == null) return;
			Queue<TreeNode> nodeQueue = new Queue<TreeNode>();
			nodeQueue.Enqueue(this);
			while (nodeQueue.Count > 0)
			{
				TreeNode next = nodeQueue.Dequeue();
				if (!preVisit(next)) break;
				for (int i = 0; i < next.getChildCount(); i++)
				{
					nodeQueue.Enqueue(next.getChild(i));
				}
			}
		}


		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="TreeNode"/> from a TreeNode xuk element
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
		/// Reads the attributes of a TreeNode xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			return true;
		}

		///	<summary>
		///	Helper function	to read	in the properties	and	invoke their respective	XUKIn	methods. 
		///	Reads	the	<see cref="Property"/>s of	the	<see cref="TreeNode"/> instance	from a mProperties xml element
		///	<list	type="table">
		///	<item>
		///	<term>Entry	state</term>
		///	<description>
		///	The	cursor of	<paramref	localName="source"/>	must be	at the start of	the	mProperties	element
		///	</description>
		///	</item>
		///	<item>
		///	<term>Exit state</term>
		///	</item>
		///	<description>
		///	The	cursor of	 <paramref localName="source"/> must	be at	the	end	of the mProperties element
		///	</description>
		///	</list>
		///	</summary>
		///	<remarks>If	the	mProperties	element	is empty,	the	start	and	the	end	of of	it are the nsame positions</remarks>
		///	<param name="source">The <see	cref="XmlReader"/> from	which	to read	the	properties</param>
		///	<returns>A <see	cref="bool"/>	indicating if	the	properties were	succesfully	read</returns>
		///	<exception cref="exception.MethodParameterIsNullException">
		///	Thrown when	the	<paramref	localName="source"/>	<see cref="XmlReader"/>	is null
		///	</exception>
		protected bool XukInProperties(System.Xml.XmlReader source)
		{
			if (source.IsEmptyElement) return true;

			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					Property newProp = getPresentation().getPropertyFactory().createProperty(source.LocalName, source.NamespaceURI);
					if (newProp != null)
					{
						newProp.setOwner(this);
						if (!newProp.XukIn(source)) return false;
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
		/// Reads the children of the <see cref="TreeNode"/> from a mChildren XUK element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the children were succesfully read</returns>
		protected bool XukInCoreNodeChildren(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Source XmlReader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					TreeNode newChild = getPresentation().getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
					if (newChild != null)
					{
						if (!newChild.XukIn(source)) return false;
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
		/// Reads a child of a TreeNode xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mProperties":
						if (!XukInProperties(source)) return false;
						break;
					case "mChildren":
						if (!XukInCoreNodeChildren(source)) return false;
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		/// <summary>
		/// Write a TreeNode element to a XUK file representing the <see cref="TreeNode"/> instance
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
		/// Writes the attributes of a TreeNode element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a TreeNode element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mProperties", urakawa.ToolkitSettings.XUK_NS);
			foreach (Property prop in mProperties.Values)
			{
				if (!prop.XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mChildren", urakawa.ToolkitSettings.XUK_NS);
			for (int i = 0; i < this.getChildCount(); i++)
			{
				if (!getChild(i).XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="TreeNode"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="TreeNode"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

    #region ITreeNodeReadOnlyMethods Members

		/// <summary>
		/// Gets the index of a given child <see cref="TreeNode"/>
		/// </summary>
		/// <param name="node">The given child <see cref="TreeNode"/></param>
		/// <returns>The index of the given child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the <see cref="TreeNode"/></exception>
		public int indexOf(TreeNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("The given node is null");
			}
			if (!mChildren.Contains(node))
			{
				throw new exception.NodeDoesNotExistException("The given node is not a child");
			}
			return mChildren.IndexOf(node);
		}

		/// <summary>
		/// Gets the child <see cref="TreeNode"/> at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The child <see cref="TreeNode"/> at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out if range, 
		/// that is not between 0 and <c><see cref="getChildCount"/>()-1</c></exception>
		public TreeNode getChild(int index)
		{
			if (index < 0 || mChildren.Count <= index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"Could not get child at index {0:0} - index is out of bounds", index));
			}
			return mChildren[index];
		}

    /// <summary>
    /// Gets the parent <see cref="TreeNode"/> of the instance
    /// </summary>
    /// <returns>The parent</returns>
    public TreeNode getParent()
    {
      return mParent;
    }

    /// <summary>
    /// Gets the number of children
    /// </summary>
    /// <returns>The number of children</returns>
    public int getChildCount()
    {
      return mChildren.Count;
    }

		///	<summary>
		///	Make a copy	of the node. The copy has the same presentation and no parent.
		///	</summary>
		///	<param name="deep">If	true,	then copy the node's	entire subtree.	 
		///	Otherwise, just	copy the node	itself.</param>
		/// <param name="inclProperties">If true, then copy the nodes properties. 
		/// Otherwise, the copy has no properties</param>
		///	<returns>A <see	cref="TreeNode"/>	containing the copied	data.</returns>
		public virtual TreeNode copy(bool deep, bool inclProperties)
		{
			TreeNode theCopy = getPresentation().getCoreNodeFactory().createNode(getXukLocalName(), getXukNamespaceUri());

			//copy the properties
			if (inclProperties)
			{
				copyProperties(theCopy);
			}
			
			//copy the children
			if (deep)
			{
				copyChildren(theCopy);
			}

			return theCopy;
		}

		///	<summary>
		///	Make a deep copy of the node. The copy has the same presentation and no parent.
		///	</summary>
		///	<param name="deep">If	true,	then copy the node's	entire subtree.	 
		///	Otherwise, just	copy the node	itself.</param>
		///	<returns>A <see	cref="TreeNode"/>	containing the copied	data.</returns>
		public virtual TreeNode copy(bool deep)
		{
			return copy(deep, true);
		}

		///	<summary>
		///	Make a deep copy of the node including properties. The copy has the same presentation and no parent.
		///	</summary>
		///	<returns>A <see	cref="TreeNode"/>	containing the copied	data.</returns>
		public virtual TreeNode copy()
		{
			return copy(true, true);
		}

		/// <summary>
		/// Copies the <see cref="Property"/>s of the current instance to a given destination <see cref="TreeNode"/>
		/// </summary>
		/// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
		protected void copyProperties(TreeNode destinationNode)
		{
			foreach (Property prop in mProperties.Values)
			{
				destinationNode.setProperty(prop.copy());
			}
		}


		/// <summary>
		/// Gets the next sibling of <c>this</c>
		/// </summary>
		/// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
		public TreeNode getNextSibling()
		{
			TreeNode p = getParent();
			if (p == null) return null;
			int i = p.indexOf(this);
			if (i + 1 >= p.getChildCount()) return null;
			return p.getChild(i + 1);
		}

		/// <summary>
		/// Gets the previous sibling of <c>this</c>
		/// </summary>
		/// <returns>The previous sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
		public TreeNode getPreviousSibling()
		{
			TreeNode p = getParent();
			if (p == null) return null;
			int i = p.indexOf(this);
			if (i == 0) return null;
			return p.getChild(i - 1);
		}

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a sibling of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a sibling of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="node"/> is <c>null</c>
		/// </exception>
		public bool isSiblingOf(TreeNode node)
		{
			if (node==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The node to test relationship with is null");
			}
			TreeNode p = getParent();
			return (p != null && p == node.getParent());
		}

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is an ancestor of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is an ancestor of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="node"/> is <c>null</c>
		/// </exception>
		public bool isAncestorOf(TreeNode node)
		{
			if (node==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The node to test relationship with is null");
			}
			TreeNode p = getParent();
			if (p == null)
			{
				return false;
			}
			else if (p == node)
			{
				return true;
			}
			else
			{
				return p.isAncestorOf(node);
			}
		}

		/// <summary>
		/// Tests if a given <see cref="TreeNode"/> is a descendant of <c>this</c>
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if <paramref localName="node"/> is a descendant of <c>this</c>, 
		/// otherwise<c>false</c></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="node"/> is <c>null</c>
		/// </exception>
		/// <remarks>This method is equivalent to <c>node.isAncestorOf(this)</c> 
		/// when <paramref localName="node"/> is not <c>null</c></remarks>
		public bool isDescendantOf(TreeNode node)
		{
			if (node==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The node to test relationship with is null");
			}
			return node.isAncestorOf(this);
		}

		#endregion

		#region ITreeNodeWriteOnlyMethods Members

    /// <summary>
    /// Inserts a <see cref="TreeNode"/> child at a given index. 
    /// The index of any children at or after the given index are increased by one
    /// </summary>
    /// <param name="node">The new child <see cref="TreeNode"/> to insert,
    /// must be between 0 and the number of children as returned by member method.
    /// Must be an instance of 
    /// <see cref="getChildCount"/></param>
    /// <param name="insertIndex">The index at which to insert the new child</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref localName="insertIndex"/> is out if range, 
    /// that is not between 0 and <c><see cref="getChildCount"/>()</c></exception>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
    public void insert(TreeNode node, int insertIndex)
    {
      if (node==null)
      {
        throw new exception.MethodParameterIsNullException(String.Format(
          "Can not insert null child at index {0:0}", insertIndex));
      }
			if (node.getParent() != null)
			{
				throw new exception.NodeNotDetachedException(
					"Can not insert child node that is already attached to a parent node");
			}
      if (insertIndex<0 || mChildren.Count<insertIndex)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
          "Could not insert a new child at index {0:0} - index is out of bounds", insertIndex));
      }
			mChildren.Insert(insertIndex, node);
			node.mParent = this;
			getPresentation().notifyCoreNodeAdded(node);
    }

    /// <summary>
    /// Detaches the instance <see cref="TreeNode"/> from it's parent's children
    /// </summary>
    /// <returns>The detached <see cref="TreeNode"/> (i.e. <c>this</c>)</returns>
    public TreeNode detach()
    {
      mParent.removeChild(this);
      mParent = null;
      return this;
    }

		/// <summary>
		/// Removes the child at a given index. 
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The removed child</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out of bounds, 
		/// that is not the index of a child 
		/// (child indexes range from 0 to <c><see cref="getChildCount"/>()-1</c>)
		/// </exception>
		public TreeNode removeChild(int index)
		{
			TreeNode removedChild = getChild(index);
			removedChild.mParent = null;
			mChildren.RemoveAt(index);
			getPresentation().notifyCoreNodeRemoved(removedChild, this, index);
			return removedChild;
		}

		/// <summary>
		/// Removes a given <see cref="TreeNode"/> child. 
		/// </summary>
		/// <param name="node">The <see cref="TreeNode"/> child to remove</param>
		/// <returns>The removed child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the instance <see cref="TreeNode"/></exception>
		public TreeNode removeChild(TreeNode node)
		{
			int index = indexOf(node);
			return removeChild(index);
		}

		/// <summary>
		/// Inserts a new <see cref="TreeNode"/> child before the given child.
		/// </summary>
		/// <param name="node">The new <see cref="TreeNode"/> child node</param>
		/// <param name="anchorNode">The child before which to insert the new child</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="anchorNode"/> 
		/// have null values</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
		public void insertBefore(TreeNode node, TreeNode anchorNode)
		{
			int index = indexOf(anchorNode);
			insert(node, index);
		}

		/// <summary>
		/// Inserts a new <see cref="TreeNode"/> child after the given child.
		/// </summary>
		/// <param name="node">The new <see cref="TreeNode"/> child node</param>
		/// <param name="anchorNode">The child after which to insert the new child</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="anchorNode"/> 
		/// have null values</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
		public void insertAfter(TreeNode node, TreeNode anchorNode)
		{
			int index = indexOf(anchorNode) + 1;
			insert(node, index);
		}

		/// <summary>
		/// Replaces the child <see cref="TreeNode"/> at a given index with a new <see cref="TreeNode"/>
		/// </summary>
		/// <param name="node">The new <see cref="TreeNode"/> with which to replace</param>
		/// <param name="index">The index of the child <see cref="TreeNode"/> to replace</param>
		/// <returns>The replaced child <see cref="TreeNode"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when index is out if range, 
		/// that is when <paramref localName="index"/> is not between 0 
		/// and <c><see cref="getChildCount"/>()-1</c>c></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
		public TreeNode replaceChild(TreeNode node, int index)
		{
			TreeNode replacedChild = getChild(index);
			insert(node, index);
			replacedChild.detach();
			return replacedChild;
		}

		/// <summary>
		/// Replaces an existing child <see cref="TreeNode"/> with i new one
		/// </summary>
		/// <param name="node">The new child with which to replace</param>
		/// <param name="oldNode">The existing child node to replace</param>
		/// <returns>The replaced <see cref="TreeNode"/> child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="oldNode"/> 
		/// have null values
		/// </exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="oldNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
		public TreeNode replaceChild(TreeNode node, TreeNode oldNode)
		{
			return replaceChild(node, indexOf(oldNode));
		}

		/// <summary>
		/// Appends a child <see cref="TreeNode"/> to the end of the list of children
		/// </summary>
		/// <param name="node">The new child to append</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="oldNode"/> 
		/// have null values
		/// </exception>
		/// <exception cref="exception.NodeNotDetachedException">
		/// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
		/// </exception>
		public void appendChild(TreeNode node)
		{
			insert(node, getChildCount());
		}

		/// <summary>
		/// Appends the children of a given <see cref="TreeNode"/> to <c>this</c>, 
		/// leaving the given <see cref="TreeNode"/> without children
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.NodeInDifferentPresentationException">
		/// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="ITreePresentation"/>
		/// </exception>
		/// <exception cref="exception.NodeIsAncestorException">
		/// Thrown when parameter <paramref localName="node"/> is an ancestor of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsDescendantException">
		/// Thrown when <paramref localName="node"/> is a descendant of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsSelfException">
		/// Thrown when parameter <paramref localName="node"/> is identical to <c>this</c>
		/// </exception>
		public void appendChildrenOf(TreeNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The given node from which to append children is null");
			}
			if (getPresentation() != node.getPresentation())
			{
				throw new exception.NodeInDifferentPresentationException(
					"Can not append the children of a node from a different presentation");
			}
			if (node == this)
			{
				throw new exception.NodeIsSelfException(
					"Can not append a nodes own children to itself");
			}
			if (isAncestorOf(node))
			{
				throw new exception.NodeIsAncestorException(
					"Can not append the children of an ancestor node");
			}
			if (isDescendantOf(node))
			{
				throw new exception.NodeIsDescendantException(
					"Can not append the children of a descendant node");
			}
			while (node.getChildCount() > 0)
			{
				appendChild(node.removeChild(0));
			}
		}

		/// <summary>
		/// Swaps <c>this</c> with a given <see cref="TreeNode"/> 
		/// </summary>
		/// <param name="node">The given <see cref="TreeNode"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.NodeInDifferentPresentationException">
		/// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="ITreePresentation"/>
		/// </exception>
		/// <exception cref="exception.NodeIsAncestorException">
		/// Thrown when parameter <paramref localName="node"/> is an ancestor of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsDescendantException">
		/// Thrown when <paramref localName="node"/> is a descendant of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsSelfException">
		/// Thrown when parameter <paramref localName="node"/> is identical to <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeHasNoParentException">
		/// Thrown when <c>this</c> or <paramref name="node"/> has no parent
		/// </exception>
		public void swapWith(TreeNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The given node with which to swap is null");
			}
			if (getPresentation() != node.getPresentation())
			{
				throw new exception.NodeInDifferentPresentationException(
					"Can not swap with a node from a different presentation");
			}
			if (node == this)
			{
				throw new exception.NodeIsSelfException(
					"Can not swap with itself");
			}
			if (isAncestorOf(node))
			{
				throw new exception.NodeIsAncestorException(
					"Can not swap with an ancestor node");
			}
			if (isDescendantOf(node))
			{
				throw new exception.NodeIsDescendantException(
					"Can not swap with a descendant node");
			}
			if (getParent() == null || node.getParent() == null)
			{
				throw new exception.NodeHasNoParentException(
					"Both nodes in a swap need to have a parent");
			}
			TreeNode thisParent = getParent();
			int thisIndex = thisParent.indexOf(this);
			detach();
			TreeNode nodeParent = node.getParent();
			nodeParent.insertAfter(this, node);
			thisParent.insert(node, thisIndex);
		}

		/// <summary>
		/// Splits <c>this</c> at the child at a given <paramref localName="index"/>, 
		/// producing a new <see cref="TreeNode"/> with the children 
		/// at indexes <c><paramref localName="index"/></c> to <c>getChildCount()-1</c> 
		/// and leaving <c>this</c> with the children at indexes <c>0</c> to <paramref localName="index"/>-1
		/// </summary>
		/// <param name="index">The index of the child at which to split</param>
		/// <param name="copyProperties">
		/// A <see cref="bool"/> indicating the <see cref="Property"/>s of <c>this</c> 
		/// should be copied to the new <see cref="TreeNode"/>
		/// </param>
		/// <returns>
		/// The new <see cref="TreeNode"/> with the children 
		/// at indexes <c><paramref localName="index"/></c> to <c>getChildCount()-1</c> 
		/// and optionally with a copy of the <see cref="Property"/>s
		/// </returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out of bounds, 
		/// that is not between <c>0</c> and <c>getChildCount()-1</c>
		/// </exception>
		public TreeNode splitChildren(int index, bool copyProperties)
		{
			if (index < 0 || getChildCount() <= index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The given index at which to split children is out of bounds");
			}
			TreeNode res = copy(false, copyProperties);
			while (index < getChildCount())
			{
				res.appendChild(removeChild(index));
			}
			return res;
		}


		/// <summary>
		/// Swaps <c>this</c> with the previous sibling of <c>this</c>
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> indicating if the swap was succesfull 
		/// (the swap is not succesfull when there is no previous sibling).
		/// </returns>
		public bool swapWithPreviousSibling()
		{
			TreeNode nextSibling = getNextSibling();
			if (nextSibling == null) return false;
			swapWith(nextSibling);
			return true;
		}

		/// <summary>
		/// Swaps <c>this</c> with the next sibling of <c>this</c>
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> indicating if the swap was succesfull 
		/// (the swap is not succesfull when there is no next sibling).
		/// </returns>
		public bool swapWithNextSibling()
		{
			TreeNode prevSibling = getPreviousSibling();
			if (prevSibling == null) return false;
			swapWith(prevSibling);
			return true;
		}

		#endregion

		#region IValueEquatable<TreeNode> Members

		/// <summary>
		/// Compares <c>this</c> with another given <see cref="TreeNode"/> to test for equality. 
		/// The comparison is deep in that any child <see cref="TreeNode"/>s are also tested,
		/// but the ancestry is not tested
		/// </summary>
		/// <param name="other">The other <see cref="TreeNode"/></param>
		/// <returns><c>true</c> if the <see cref="TreeNode"/>s are equal, otherwise <c>false</c></returns>
		public virtual bool ValueEquals(TreeNode other)
		{
			if (other == null) return false;
			if (other.GetType() != this.GetType()) return false;
			Type[] thisProps = getListOfUsedPropertyTypes();
			Type[] otherProps = other.getListOfUsedPropertyTypes();
			if (thisProps.Length != otherProps.Length) return false;
			foreach (Type pt in thisProps)
			{
				Property thisP = getProperty(pt);
				Property otherP = other.getProperty(pt);
				if (otherP == null) return false;
				if (!thisP.ValueEquals(otherP)) return false;
			}
			if (getChildCount() != other.getChildCount()) return false;
			for (int i = 0; i < getChildCount(); i++)
			{
				if (!getChild(i).ValueEquals(other.getChild(i))) return false;
			}
			return true;
		}

		#endregion
	}
}
