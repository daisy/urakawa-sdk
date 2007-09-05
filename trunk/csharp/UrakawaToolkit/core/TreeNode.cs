using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core.visitor;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.core
{
	/// <summary>
	/// A node in the core tree of the SDK
	/// </summary>
	public class TreeNode : WithPresentation, ITreeNodeReadOnlyMethods, ITreeNodeWriteOnlyMethods, IVisitableTreeNode, IXukAble, IValueEquatable<TreeNode>
	{

		/// <summary>
		/// Containe the <see cref="Property"/>s of the node
		/// </summary>
		private List<Property> mProperties;

		/// <summary>
		/// Contains the children of the node
		/// </summary>
		private List<TreeNode> mChildren;

		/// <summary>
		/// The parent <see cref="TreeNode"/>
		/// </summary>
		private TreeNode mParent;


		/// <summary>
		/// Default constructor
		/// </summary>
		protected internal TreeNode()
		{
			mProperties = new List<Property>();
			mChildren = new List<TreeNode>();
		}

		/// <summary>
		/// Gets a list of the <see cref="Type"/>s of <see cref="Property"/> set for the <see cref="TreeNode"/>
		/// </summary>
		/// <returns>The list</returns>
		public List<Type> getListOfUsedPropertyTypes()
		{
			List<Type> res = new List<Type>();
			foreach (Property p in getListOfProperties())
			{
				if (!res.Contains(p.GetType())) res.Add(p.GetType());
			}
			return res;
		}

		/// <summary>
		/// Gets a list of all <see cref="Property"/>s of this
		/// </summary>
		/// <returns>The list</returns>
		public List<Property> getListOfProperties()
		{
			return new List<Property>(mProperties);
		}
		/// <summary>
		/// Gets a list of the <see cref="Property"/>s of this of a given <see cref="Type"/>
		/// </summary>
		/// <param name="t">The given type</param>
		/// <returns>The list</returns>
		public List<Property> getListOfProperties(Type t)
		{
			List<Property> res = new List<Property>();
			foreach (Property p in getListOfProperties())
			{
				if (p.GetType() == t) res.Add(p);
			}
			return res;
		}

		/// <summary>
		/// Gets the <see cref="Property"/>s of a the given <see cref="Property"/> sub-type
		/// </summary>
		/// <typeparam name="T">The type of the properties to get - must sub-class <see cref="Property"/></typeparam>
		/// <returns>A list of all <typeparamref name="T"/> properties of <c>this</c>, possibly an empty list</returns>
		public List<T> getListOfProperties<T>() where T : Property
		{
			List<T> res = new List<T>();
			foreach (Property p in getListOfProperties(typeof(T))) res.Add(p as T);
			return res;
		}

		/// <summary>
		/// Gets the first <see cref="Property"/> of a the given <see cref="Property"/> sub-type
		/// </summary>
		/// <param name="t">The given <see cref="Property"/> subtype</param>
		/// <returns>The first property of the given subtype - possibly null</returns>
		public Property getProperty(Type t)
		{
			List<Property> props = getListOfProperties(t);
			if (props.Count > 0) return props[0];
			return null;
		}

		/// <summary>
		/// Gets the first <see cref="Property"/> of a the given <see cref="Property"/> sub-type
		/// </summary>
		/// <typeparam name="T">The type of the property to get - must sub-class <see cref="Property"/></typeparam>
		/// <returns>The first <typeparamref name="T"/> property of this if it exists, else <c>null</c></returns>
		public T getProperty<T>() where T : Property
		{
			return getProperty(typeof(T)) as T;
		}

		/// <summary>
		/// Adds a <see cref="Property"/> to the node
		/// </summary>
		/// <param name="prop">The list of <see cref="Property"/>s to add.</param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="props"/> is null</exception>
		public void addProperties(IList<Property> props)
		{
			if (props == null) throw new exception.MethodParameterIsNullException("No list of Property was given");
			foreach (Property p in props)
			{
				addProperty(p);
			}
		}

		/// <summary>
		/// Adds a <see cref="Property"/> to the node
		/// </summary>
		/// <param name="prop">The <see cref="Property"/> to add. </param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="prop"/> is null</exception>
		/// <exception cref="exception.PropertyAlreadyHasOwnerException">Thrown when <see cref="Property"/> is already owned by another node</exception>
		/// <exception cref="exception.NodeInDifferentPresentationException">Thrown when the new <see cref="Property"/> belongs to a different <see cref="Presentation"/></exception>
		public void addProperty(Property prop)
		{
			if (prop == null) throw new exception.MethodParameterIsNullException("Can not add a null Property to the TreeNode");
			if (!mProperties.Contains(prop))
			{
				prop.setTreeNodeOwner(this);
				mProperties.Add(prop);
			}
		}

		/// <summary>
		/// Remove the <see cref="Property"/>s of a given <see cref="Type"/> from this
		/// </summary>
		/// <param name="propType">Specify the type of properties to remove</param>
		/// <returns>The list of removed properties</returns>
		public List<Property> removeProperties(Type propType)
		{
			List<Property> remProps = getListOfProperties(propType);
			foreach (Property p in remProps)
			{
				removeProperty(p);
			}
			return remProps;
		}

		/// <summary>
		/// Removes all <see cref="Property"/>s from this
		/// </summary>
		public void removeProperties()
		{
			foreach (Property p in getListOfProperties())
			{
				removeProperty(p);
			}
		}

		/// <summary>
		/// Removes a given <see cref="Property"/>
		/// </summary>
		/// <param name="prop">The <see cref="Property"/> to remove</param>
		public void removeProperty(Property prop)
		{
			if (prop == null) throw new exception.MethodParameterIsNullException("Can not remove a null Property");
			if (mProperties.Contains(prop))
			{
				mProperties.Remove(prop);
				prop.setTreeNodeOwner(null);
			}
		}

		/// <summary>
		/// Determines if this has any <see cref="Property"/>s
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if this has any properties</returns>
		public bool hasProperties()
		{
			return (mProperties.Count > 0);
		}

		/// <summary>
		/// Determines if this has any <see cref="Property"/>s of a given <see cref="Type"/>
		/// </summary>
		/// <param name="t">The given type</param>
		/// <returns>A <see cref="bool"/> indicating if this has any properties</returns>
		public bool hasProperties(Type t)
		{
			foreach (Property p in getListOfProperties())
			{
				if (p.GetType() == t) return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if a given <see cref="Property"/> is owned by this
		/// </summary>
		/// <param name="prop">The property</param>
		/// <returns>A <see cref="bool"/> indicating if the given property is a property of this</returns>
		public bool hasProperty(Property prop)
		{
			if (prop == null) throw new exception.MethodParameterIsNullException("The TreeNode can not have a null Property");
			return mProperties.Contains(prop);
		}



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

		#region IVisitableTreeNode Members

		/// <summary>
		/// Accept a <see cref="ITreeNodeVisitor"/> in depth first mode.
		/// </summary>
		/// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
		/// <remarks>
		/// Remark that only <see cref="ITreeNodeVisitor.preVisit"/> is executed during breadth-first tree traversal,
		/// since there is no notion of post in breadth first traversal
		/// </remarks>
		public void acceptDepthFirst(ITreeNodeVisitor visitor)
		{
			PreVisitDelegate preVisit = new PreVisitDelegate(visitor.preVisit);
			PostVisitDelegate postVisit = new PostVisitDelegate(visitor.postVisit);
			acceptDepthFirst(preVisit, postVisit);
		}

		/// <summary>
		/// Accept a <see cref="ITreeNodeVisitor"/> in breadth first mode
		/// </summary>
		/// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
		/// <remarks>HACK: Not yet implemented, does nothing!!!!</remarks>
		public void acceptBreadthFirst(ITreeNodeVisitor visitor)
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
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read TreeNode from a non-element node");
			}
			try
			{
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
					String.Format("An exception occured during XukIn of TreeNode: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a TreeNode xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{

		}

		private void XukInProperties(System.Xml.XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						Property newProp = getPresentation().getPropertyFactory().createProperty(source.LocalName, source.NamespaceURI);
						if (newProp != null)
						{
							newProp.setTreeNodeOwner(this);
							newProp.XukIn(source);
							addProperty(newProp);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		private void XukInChildren(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						TreeNode newChild = getPresentation().getTreeNodeFactory().createNode(source.LocalName, source.NamespaceURI);
						if (newChild != null)
						{
							newChild.XukIn(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Reads a child of a TreeNode xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mProperties":
						XukInProperties(source);
						break;
					case "mChildren":
						XukInChildren(source);
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
		}

		/// <summary>
		/// Write a TreeNode element to a XUK file representing the <see cref="TreeNode"/> instance
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
					String.Format("An exception occured during XukOut of TreeNode: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a TreeNode element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Write the child elements of a TreeNode element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mProperties", urakawa.ToolkitSettings.XUK_NS);
			foreach (Property prop in getListOfProperties())
			{
				prop.XukOut(destination);
			}
			destination.WriteEndElement();
			destination.WriteStartElement("mChildren", urakawa.ToolkitSettings.XUK_NS);
			for (int i = 0; i < this.getChildCount(); i++)
			{
				getChild(i).XukOut(destination);
			}
			destination.WriteEndElement();
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

		/// <summary>
		/// Gets a list of the child <see cref="TreeNode"/>s of this
		/// </summary>
		/// <returns>The list</returns>
		public List<TreeNode> getListOfChildren()
		{
			return new List<TreeNode>(mChildren);
		}

		/// <summary>
		/// Make a copy of the node. The copy has the same presentation and no parent.
		/// </summary>
		/// <param name="deep">If true, then copy the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <param name="inclProperties">If true, then copy the nodes property. 
		/// Otherwise, the copy has no property</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		protected virtual TreeNode copyProtected(bool deep, bool inclProperties)
		{
			TreeNode theCopy = getPresentation().getTreeNodeFactory().createNode(getXukLocalName(), getXukNamespaceUri());

			//copy the property
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

		/// <summary>
		/// Make a copy of the node. The copy will optionally be deep and will optionally include properties.
		/// The copy has the same presentation and no parent.
		/// </summary>
		/// <param name="deep">If true, then copy the node's entire subtree (ie. deep copy).  
		/// Otherwise, just copy the node itself.</param>
		/// <param name="inclProperties">If true, then copy the nodes property. 
		/// Otherwise, the copy has no property</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		public TreeNode copy(bool deep, bool inclProperties)
		{
			return copyProtected(deep, inclProperties);
		}

		/// <summary>
		/// Make a copy of the node including the properties. The copy is optionally deep. 
		/// The copy has the same presentation and no parent.
		/// </summary>
		/// <param name="deep">If true, then copy the node's entire subtree.  
		/// Otherwise, just copy the node itself.</param>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		public TreeNode copy(bool deep)
		{
			return copy(deep, true);
		}

		/// <summary>
		/// Make a deep copy of the node including properties. The copy has the same presentation and no parent.
		/// </summary>
		/// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
		public TreeNode copy()
		{
			return copy(true, true);
		}

		/// <summary>
		/// Copies the <see cref="Property"/>s of the current instance to a given destination <see cref="TreeNode"/>
		/// </summary>
		/// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
		protected void copyProperties(TreeNode destinationNode)
		{
			foreach (Property prop in getListOfProperties())
			{
				destinationNode.addProperty(prop.copy());
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
			if (node == null)
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
			if (node == null)
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
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The node to test relationship with is null");
			}
			return node.isAncestorOf(this);
		}

		/// <summary>
		/// Creates a new TreeNode with identical content (recursively) as this node,
		/// but compatible with the given Presentation (factories, managers,
		/// channels, etc.). 
		/// </summary>
		/// <param name="destPres">The destination Presentation to which this node (and all its content, recursively) should be exported.</param>
		/// <returns>The exported node</returns>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="destPres"/> is null</exception>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the facotries of <paramref name="destPres"/> can not create a node in the sub-tree beginning at <c>this</c>
		/// or a property associated object for one of the nodes in the sub-tree
		/// </exception>
		public TreeNode export(Presentation destPres)
		{
			if (destPres == null)
			{
				throw new exception.MethodParameterIsNullException("Can not export the TreeNode to a null Presentation");
			}
			TreeNode exportedNode = destPres.getTreeNodeFactory().createNode(getXukLocalName(), getXukNamespaceUri());
			if (exportedNode == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The TreeNodeFactory of the export destination Presentation can not create a TreeNode matching Xuk QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			foreach (Property prop in getListOfProperties())
			{
				exportedNode.addProperty(prop.export(destPres));
			}
			foreach (TreeNode child in getListOfChildren())
			{
				exportedNode.appendChild(child.export(destPres));
			}
			return exportedNode;
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
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(String.Format(
					"Can not insert null child at index {0:0}", insertIndex));
			}
			if (node.getParent() != null)
			{
				throw new exception.NodeNotDetachedException(
					"Can not insert child node that is already attached to a parent node");
			}
			if (insertIndex < 0 || mChildren.Count < insertIndex)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
					"Could not insert a new child at index {0:0} - index is out of bounds", insertIndex));
			}
			mChildren.Insert(insertIndex, node);
			node.mParent = this;
			getPresentation().notifyTreeNodeAdded(node);
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
			getPresentation().notifyTreeNodeRemoved(removedChild, this, index);
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
		public virtual bool valueEquals(TreeNode other)
		{
			if (other == null) return false;
			if (other.GetType() != this.GetType()) return false;
			List<Type> thisProps = getListOfUsedPropertyTypes();
			List<Type> otherProps = other.getListOfUsedPropertyTypes();
			if (thisProps.Count != otherProps.Count) return false;
			foreach (Type pt in thisProps)
			{
				List<Property> thisPs = getListOfProperties(pt);
				List<Property> otherPs = other.getListOfProperties(pt);
				if (thisPs.Count != otherPs.Count) return false;
				for (int i = 0; i < thisPs.Count; i++)
				{
					if (!thisPs[i].valueEquals(otherPs[i])) return false;
				}
			}
			if (getChildCount() != other.getChildCount()) return false;
			for (int i = 0; i < getChildCount(); i++)
			{
				if (!getChild(i).valueEquals(other.getChild(i))) return false;
			}
			return true;
		}

		#endregion
	}
}
