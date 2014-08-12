using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.core.visitor;
using urakawa.events;
using urakawa.events.core;
using urakawa.exception;
using urakawa.navigation;
using urakawa.progress;
using urakawa.property;
using urakawa.property.alt;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace urakawa.core
{
    public interface ObjectTag
    {
        object Tag { get; set; }
    }

    [XukNameUglyPrettyAttribute("n", "TreeNode")]
    public partial class TreeNode : WithPresentation, ObjectTag, ITreeNodeReadOnlyMethods, ITreeNodeWriteOnlyMethods, IVisitableTreeNode, IChangeNotifier
    {
        private object m_Tag = null;
        public object Tag
        {
            set { m_Tag = value; }
            get { return m_Tag; }
        }

        public bool HasAlternateContentProperty
        {
            get { return GetAlternateContentProperty() != null; }
        }
        public AlternateContentProperty GetOrCreateAlternateContentProperty()
        {
            return GetOrCreateProperty<AlternateContentProperty>();
        }
        public AlternateContentProperty GetAlternateContentProperty()
        {
            return GetProperty<AlternateContentProperty>();
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string attr = source.GetAttribute(XukStrings.IsMarked);
            if (attr == "true" || attr == "1")
            {
                IsMarked = true;
            }
            else
            {
                IsMarked = false;
            }
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (IsMarked)
            {
                destination.WriteAttributeString(XukStrings.IsMarked, "true");
            }
        }
        public event EventHandler<DataModelChangedEventArgs> IsMarkedChanged;

        private bool m_IsMarked;
        public bool IsMarked
        {
            get { return m_IsMarked; }
            set
            {
                if (value == m_IsMarked)
                {
                    return;
                }
                m_IsMarked = value;

                IsMarkedChangedEventArgs ev = new IsMarkedChangedEventArgs(this);
                EventHandler<DataModelChangedEventArgs> d = IsMarkedChanged;
                if (d != null)
                {
                    d.Invoke(this, ev);
                    //SAME AS : d(this, ev);
                }
                NotifyChanged(ev);
            }
        }


        public TreeNode GetFirstAncestorWithMark()
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent.IsMarked)
            {
                return Parent;
            }

            return Parent.GetFirstAncestorWithMark();
        }

        public TreeNode GetFirstDescendantWithMark()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }

            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                if (child.IsMarked)
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithMark();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetLastDescendantWithMark()
        {
            if (mChildren.Count == 0)
            {
                return null;
            }
            
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                TreeNode child = Children.Get(i);

                if (child.IsMarked)
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithMark();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithMark()
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode previous = this;
            while ((previous = previous.PreviousSibling) != null)
            {
                if (previous.IsMarked)
                {
                    return previous;
                }

                TreeNode previousIn = previous.GetLastDescendantWithMark();
                if (previousIn != null)
                {
                    return previousIn;
                }
            }

            return Parent.GetPreviousSiblingWithMark();
        }

        public TreeNode GetNextSiblingWithMark()
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                if (next.IsMarked)
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithMark();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            return Parent.GetNextSiblingWithMark();
        }




        private TreeNode MeetFirst(TreeNode node1, TreeNode node2)
        {
            if (this == node1) return node1;
            if (this == node2) return node2;
            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                TreeNode met = child.MeetFirst(node1, node2);
                if (met != null)
                {
                    return met;
                }
            }
            return null;
        }
        
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="TreeNode"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(DataModelChangedEventArgs args)
        {
            EventHandler<DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        ///// <summary>
        ///// Event fired after the <see cref="TreeNode"/> has been added as a child 
        ///// of another <see cref="TreeNode"/> (now it's parent)
        ///// </summary>
        //public event EventHandler<urakawa.events.core.ChildAddedEventArgs> ChildAdded;

        ///// <summary>
        ///// Fires the <see cref="ChildAdded"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="TreeNode"/> at which the event occured
        ///// </param>
        ///// <param name="addedChild">
        ///// The child <see cref="TreeNode"/> that was added to <paramref name="source"/>
        ///// </param>
        //protected void NotifyChildAdded(TreeNode source, TreeNode addedChild)
        //{
        //    EventHandler<urakawa.events.core.ChildAddedEventArgs> d = ChildAdded;
        //    if (d != null) d(this, new urakawa.events.core.ChildAddedEventArgs(source, addedChild));
        //}

        ///// <summary>
        ///// Event fired after the <see cref="TreeNode"/> has been removed as a child 
        ///// of another <see cref="TreeNode"/> (porperly it's parent)
        ///// </summary>
        //public event EventHandler<urakawa.events.core.ChildRemovedEventArgs> ChildRemoved;

        ///// <summary>
        ///// Fires the <see cref="ChildRemoved"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="TreeNode"/> at which the event occured, 
        ///// i.e. the <see cref="TreeNode"/> from which a child was removed
        ///// </param>
        ///// <param name="removedChild">The child that was removed</param>
        ///// <param name="position">The position from which the child was removed</param>
        //protected void NotifyChildRemoved(TreeNode source, TreeNode removedChild, int position)
        //{
        //    EventHandler<urakawa.events.core.ChildRemovedEventArgs> d = ChildRemoved;
        //    if (d != null) d(this, new urakawa.events.core.ChildRemovedEventArgs(source, removedChild, position));
        //}

        ///// <summary>
        ///// Event fired after a <see cref="Property"/> has been added to a <see cref="TreeNode"/>
        ///// </summary>
        //public event EventHandler<urakawa.events.core.PropertyAddedEventArgs> PropertyAdded;

        ///// <summary>
        ///// Fires the <see cref="PropertyAdded"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="TreeNode"/> to which a <see cref="Property"/> was added
        ///// </param>
        ///// <param name="addedProp">The <see cref="Property"/> that was added</param>
        //protected void NotifyPropertyAdded(TreeNode source, Property addedProp)
        //{
        //    EventHandler<urakawa.events.core.PropertyAddedEventArgs> d = PropertyAdded;
        //    if (d != null) d(this, new urakawa.events.core.PropertyAddedEventArgs(source, addedProp));
        //}

        ///// <summary>
        ///// Event fired after a <see cref="Property"/> has been removed from a <see cref="TreeNode"/>
        ///// </summary>
        //public event EventHandler<urakawa.events.core.PropertyRemovedEventArgs> PropertyRemoved;

        ///// <summary>
        ///// Fires the <see cref="PropertyRemoved"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="TreeNode"/> to which a <see cref="Property"/> was added
        ///// </param>
        ///// <param name="removedProp">The <see cref="Property"/> that was removed</param>
        //protected void NotifyPropertyRemoved(TreeNode source, Property removedProp)
        //{
        //    EventHandler<urakawa.events.core.PropertyRemovedEventArgs> d = PropertyRemoved;
        //    if (d != null) d(this, new urakawa.events.core.PropertyRemovedEventArgs(source, removedProp));
        //}

        private void this_childAdded(object sender, ObjectAddedEventArgs<TreeNode> ev)
        {
            ev.m_AddedObject.Changed += Child_Changed;
            NotifyChanged(ev);
        }

        private void Child_Changed(object sender, DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private void this_childRemoved(object sender, ObjectRemovedEventArgs<TreeNode> ev)
        {
            ev.m_RemovedObject.Changed -= Child_Changed;
            NotifyChanged(ev);
        }

        private void this_propertyAdded(object sender, ObjectAddedEventArgs<Property> ev)
        {
            //ChannelsProperty cProp = ev.m_AddedObject as ChannelsProperty;
            //if (cProp != null)
            //{
            //    foreach (Channel ch in cProp.UsedChannels)
            //    {
            //        media.Media med = cProp.GetMedia(ch);
            //        if (med is media.AbstractTextMedia)
            //    }
            //}
            //this.TextLocal = null;
            //this.TextFlattened = null;

            ev.m_AddedObject.Changed += Property_Changed;
            NotifyChanged(ev);
        }

        private void Property_Changed(object sender, DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private void this_propertyRemoved(object sender, ObjectRemovedEventArgs<Property> ev)
        {
            //this.TextLocal = null;
            //this.TextFlattened = null;

            ev.m_RemovedObject.Changed -= Property_Changed;
            NotifyChanged(ev);
        }

        #endregion

        /// <summary>
        /// Containe the <see cref="Property"/>s of the node
        /// </summary>
        private ObjectListProvider<Property> mProperties;

        /// <summary>
        /// Contains the children of the node
        /// </summary>
        private ObjectListProvider<TreeNode> mChildren;


        /// <summary>
        /// The parent <see cref="TreeNode"/>
        /// </summary>
        private TreeNode mParent;


        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="TreeNode"/>s should only be created via. the <see cref="TreeNodeFactory"/>
        /// </summary>
        public TreeNode()
        {
            mChildren = new ObjectListProvider<TreeNode>(this, false);
            mChildren.ObjectAdded += this_childAdded;
            mChildren.ObjectRemoved += this_childRemoved;

            mProperties = new ObjectListProvider<Property>(this, false);
            mProperties.ObjectAdded += this_propertyAdded;
            mProperties.ObjectRemoved += this_propertyRemoved;
        }


        #region IVisitableTreeNode Members

        /// <summary>
        /// Accept a <see cref="ITreeNodeVisitor"/> in depth first mode.
        /// </summary>
        /// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
        /// <remarks>
        /// Remark that only <see cref="ITreeNodeVisitor.PreVisit"/> is executed during breadth-first tree traversal,
        /// since there is no notion of post in breadth first traversal
        /// </remarks>
        public void AcceptDepthFirst(ITreeNodeVisitor visitor)
        {
            PreVisitDelegate preVisit = new PreVisitDelegate(visitor.PreVisit);
            PostVisitDelegate postVisit = new PostVisitDelegate(visitor.PostVisit);
            AcceptDepthFirst(preVisit, postVisit);
        }

        /// <summary>
        /// Accept a <see cref="ITreeNodeVisitor"/> in breadth first mode
        /// </summary>
        /// <param name="visitor">The <see cref="ITreeNodeVisitor"/></param>
        /// <remarks>HACK: Not yet implemented, does nothing!!!!</remarks>
        public void AcceptBreadthFirst(ITreeNodeVisitor visitor)
        {
            PreVisitDelegate preVisit = new PreVisitDelegate(visitor.PreVisit);
            AcceptBreadthFirst(preVisit);
        }


        /// <summary>
        /// Visits the <see cref="IVisitableTreeNode"/> depth-first
        /// </summary>
        /// <param name="preVisit">The pre-visit delegate - may be null</param>
        /// <param name="postVisit">The post visit delegate - may be null</param>
        public void AcceptDepthFirst(PreVisitDelegate preVisit, PostVisitDelegate postVisit)
        {
            //If both PreVisit and PostVisit delegates are null, there is nothing to do.
            if (preVisit == null && postVisit == null) return;
            bool visitChildren = true;
            if (preVisit != null)
            {
                if (!preVisit(this)) visitChildren = false;
            }
            if (visitChildren)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    mChildren.Get(i).AcceptDepthFirst(preVisit, postVisit);
                }
            }
            if (postVisit != null) postVisit(this);
        }

        /// <summary>
        /// Visits the <see cref="IVisitableTreeNode"/> breadth-first
        /// </summary>
        /// <param name="preVisit">The pre-visit delegate - may be null</param>
        public void AcceptBreadthFirst(PreVisitDelegate preVisit)
        {
            if (preVisit == null) return;
            Queue<TreeNode> nodeQueue = new Queue<TreeNode>();
            nodeQueue.Enqueue(this);
            while (nodeQueue.Count > 0)
            {
                TreeNode next = nodeQueue.Dequeue();
                if (!preVisit(next)) break;
                for (int i = 0; i < next.Children.Count; i++)
                {
                    nodeQueue.Enqueue(next.mChildren.Get(i));
                }
            }
        }


        public void AcceptBreadthFirst(INavigator navigator, ITreeNodeVisitor visitor)
        {
            PreVisitDelegate preVisit = new PreVisitDelegate(visitor.PreVisit);
            AcceptBreadthFirst(navigator, preVisit);
        }
        public void AcceptBreadthFirst(INavigator navigator, PreVisitDelegate preVisit)
        {
            if (preVisit == null) return;
            Queue<TreeNode> nodeQueue = new Queue<TreeNode>();
            nodeQueue.Enqueue(this);
            while (nodeQueue.Count > 0)
            {
                TreeNode next = nodeQueue.Dequeue();
                if (!preVisit(next)) break;

                int count = navigator.GetChildCount(next);
                for (int i = 0; i < count; i++)
                {
                    nodeQueue.Enqueue(navigator.GetChild(next, i));
                }
            }
        }

        public void AcceptDepthFirst(INavigator navigator, ITreeNodeVisitor visitor)
        {
            PreVisitDelegate preVisit = new PreVisitDelegate(visitor.PreVisit);
            PostVisitDelegate postVisit = new PostVisitDelegate(visitor.PostVisit);
            AcceptDepthFirst(navigator, preVisit, postVisit);
        }
        public void AcceptDepthFirst(INavigator navigator, PreVisitDelegate preVisit, PostVisitDelegate postVisit)
        {
            //If both PreVisit and PostVisit delegates are null, there is nothing to do.
            if (preVisit == null && postVisit == null) return;
            bool visitChildren = true;
            if (preVisit != null)
            {
                if (!preVisit(this)) visitChildren = false;
            }
            if (visitChildren)
            {
                int count = navigator.GetChildCount(this);
                for (int i = 0; i < count; i++)
                {
                    navigator.GetChild(this, i).AcceptDepthFirst(preVisit, postVisit);
                }
            }
            if (postVisit != null) postVisit(this);
        }


        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="TreeNode"/> removing all children and <see cref="Property"/>s
        /// </summary>
        protected override void Clear()
        {
            foreach (TreeNode child in Children.ContentsAs_ListCopy)
            {
                RemoveChild(child);
            }
            foreach (Property prop in Properties.ContentsAs_ListCopy)
            {
                RemoveProperty(prop);
            }
            base.Clear();
        }

        public override void XukIn(XmlReader source, IProgressHandler handler)
        {
            Presentation.m_XukedInTreeNodes++;
            base.XukIn(source, handler);

            //XukInAfter_TextMediaCache();
        }

        private void XukInProperties(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Property newProp = Presentation.PropertyFactory.Create(source.LocalName, source.NamespaceURI);
                        if (newProp != null)
                        {
                            newProp.XukIn(source, handler, this);
                            AddProperty(newProp);
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
                    if (source.EOF) throw new XukException("Unexpectedly reached EOF");
                }
            }
        }
        private void XukInChildNode(XmlReader source, IProgressHandler handler)
        {
            TreeNode newChild = Presentation.TreeNodeFactory.Create(source.LocalName, source.NamespaceURI);
            if (newChild != null)
            {
                newChild.mParent = this;

                newChild.XukIn(source, handler);

                newChild.mParent = null;
                AppendChild(newChild);
                //Insert(newChild, mChildren.Count);
            }
            else if (!source.IsEmptyElement)
            {
                //Read past unidentified element
                source.ReadSubtree().Close();
            }
        }
        private void XukInChildren(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        XukInChildNode(source, handler);
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new XukException("Unexpectedly reached EOF");
                }
            }
        }

        /// <summary>
        /// Reads a child of a TreeNode xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = true;

            if (source.NamespaceURI == XUK_NS && source.LocalName == XukStrings.Properties)
            {
                XukInProperties(source, handler);
            }
            else if (PrettyFormat && source.NamespaceURI == XUK_NS && source.LocalName == XukStrings.Children)
            {
                XukInChildren(source, handler);
            }
            else if (!PrettyFormat)
            {
                XukInChildNode(source, handler);
            }
            else
            {
                readItem = false;
            }

            if (!readItem) base.XukInChild(source, handler);
        }

        /// <summary>
        /// Write the child elements of a TreeNode element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Properties, XUK_NS);
            foreach (Property prop in Properties.ContentsAs_Enumerable)
            {
                prop.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            if (PrettyFormat)
            {
                destination.WriteStartElement(XukStrings.Children, XUK_NS);
            }
            for (int i = 0; i < Children.Count; i++)
            {
                mChildren.Get(i).XukOut(destination, baseUri, handler);
            }
            if (PrettyFormat)
            {
                destination.WriteEndElement();
            }
        }

        #endregion

        #region ITreeNodeReadOnlyMethods Members

        public ObjectListProvider<Property> Properties
        {
            get
            {
                return mProperties;
            }
        }

        public ObjectListProvider<TreeNode> Children
        {
            get
            {
                return mChildren;
            }
        }

        /// <summary>
        /// Gets a list of the <see cref="Type"/>s of <see cref="Property"/> set for the <see cref="TreeNode"/>
        /// </summary>
        /// <returns>The list</returns>
        public List<Type> UsedPropertyTypes
        {
            get
            {
                List<Type> res = new List<Type>();
                foreach (Property p in Properties.ContentsAs_Enumerable)
                {
                    if (!res.Contains(p.GetType())) res.Add(p.GetType());
                }
                return res;
            }
        }

        /// <summary>
        /// Gets a list of the <see cref="Property"/>s of this of a given <see cref="Type"/>
        /// </summary>
        /// <param name="t">The given type</param>
        /// <returns>The list</returns>
        public List<Property> GetProperties(Type t)
        {
            List<Property> res = new List<Property>();
            foreach (Property p in Properties.ContentsAs_Enumerable)
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
        public List<T> GetProperties<T>() where T : Property
        {
            List<T> res = new List<T>();
            foreach (Property p in GetProperties(typeof(T))) res.Add(p as T);
            return res;
        }

        /// <summary>
        /// Gets the first <see cref="Property"/> of a the given <see cref="Property"/> sub-type
        /// </summary>
        /// <param name="t">The given <see cref="Property"/> subtype</param>
        /// <returns>The first property of the given subtype - possibly null</returns>
        public Property GetProperty(Type t)
        {
            List<Property> props = GetProperties(t);
            if (props.Count > 0) return props[0];
            return null;
        }

        /// <summary>
        /// Gets the first <see cref="Property"/> of a the given <see cref="Property"/> sub-type
        /// </summary>
        /// <typeparam name="T">The type of the property to get - must sub-class <see cref="Property"/></typeparam>
        /// <returns>The first <typeparamref name="T"/> property of this if it exists, else <c>null</c></returns>
        public T GetProperty<T>() where T : Property
        {
            return GetProperty(typeof(T)) as T;
        }


        /// <summary>
        /// Determines if this has any <see cref="Property"/>s
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating if this has any properties</returns>
        public bool HasProperties()
        {
            return (mProperties.Count > 0);
        }

        /// <summary>
        /// Determines if this has any <see cref="Property"/>s of a given <see cref="Type"/>
        /// </summary>
        /// <param name="t">The given type</param>
        /// <returns>A <see cref="bool"/> indicating if this has any properties</returns>
        public bool HasProperties(Type t)
        {
            foreach (Property p in Properties.ContentsAs_Enumerable)
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
        public bool HasProperty(Property prop)
        {
            if (prop == null)
                throw new MethodParameterIsNullException("The TreeNode can not have a null Property");
            return mProperties.IndexOf(prop) != -1;
        }

        /// <summary>
        /// Gets the parent <see cref="TreeNode"/> of the instance
        /// </summary>
        /// <returns>The parent</returns>
        public TreeNode Parent
        {
            get { return mParent; }
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
        public TreeNode Copy(bool deep, bool inclProperties)
        {
            return CopyProtected(deep, inclProperties);
        }

        /// <summary>
        /// Make a copy of the node including the properties. The copy is optionally deep. 
        /// The copy has the same presentation and no parent.
        /// </summary>
        /// <param name="deep">If true, then copy the node's entire subtree.  
        /// Otherwise, just copy the node itself.</param>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        public TreeNode Copy(bool deep)
        {
            return Copy(deep, true);
        }

        /// <summary>
        /// Make a deep copy of the node including properties. The copy has the same presentation and no parent.
        /// </summary>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        public TreeNode Copy()
        {
            return Copy(true, true);
        }


        /// <summary>
        /// Copies the children of the current instance to a given destination <see cref="TreeNode"/>
        /// </summary>
        /// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
        /// <remarks>The children are copied deep and any existing children of the destination <see cref="TreeNode"/>
        /// are not removed</remarks>
        protected void CopyChildren(TreeNode destinationNode)
        {
            for (int i = 0; i < this.mChildren.Count; i++)
            {
                destinationNode.AppendChild(mChildren.Get(i).Copy(true));
            }
        }

        /// <summary>
        /// Copies the <see cref="Property"/>s of the current instance to a given destination <see cref="TreeNode"/>
        /// </summary>
        /// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
        protected void CopyProperties(TreeNode destinationNode)
        {
            foreach (Property prop in Properties.ContentsAs_Enumerable)
            {
                destinationNode.AddProperty(prop.Copy());
            }
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
        public TreeNode Export(Presentation destPres)
        {
            return ExportProtected(destPres);
        }

        /// <summary>
        /// Make a copy of the node. The copy has the same presentation and no parent.
        /// </summary>
        /// <param name="deep">If true, then copy the node's entire subtree.  
        /// Otherwise, just copy the node itself.</param>
        /// <param name="inclProperties">If true, then copy the nodes property. 
        /// Otherwise, the copy has no property</param>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        protected virtual TreeNode CopyProtected(bool deep, bool inclProperties)
        {
            TreeNode theCopy = Presentation.TreeNodeFactory.Create(GetType());

            theCopy.IsMarked = IsMarked;

            //copy the property
            if (inclProperties)
            {
                CopyProperties(theCopy);
            }

            //copy the children
            if (deep)
            {
                CopyChildren(theCopy);
            }

            return theCopy;
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
        protected virtual TreeNode ExportProtected(Presentation destPres)
        {
            if (destPres == null)
            {
                throw new MethodParameterIsNullException("Can not export the TreeNode to a null Presentation");
            }
            TreeNode exportedNode = destPres.TreeNodeFactory.Create(GetType());
            if (exportedNode == null)
            {
                string msg = String.Format(
                    "The TreeNodeFactory of the export destination Presentation can not create a TreeNode matching Xuk QName {1}:{0}",
                    GetXukName(), GetXukNamespace());
                throw new FactoryCannotCreateTypeException(msg);
            }

            exportedNode.IsMarked = IsMarked;

            foreach (Property prop in Properties.ContentsAs_Enumerable)
            {
                exportedNode.AddProperty(prop.Export(destPres));
            }
            foreach (TreeNode child in Children.ContentsAs_Enumerable)
            {
                exportedNode.AppendChild(child.Export(destPres));
            }
            return exportedNode;
        }


        /// <summary>
        /// Gets the next sibling of <c>this</c>
        /// </summary>
        /// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
        public TreeNode NextSibling
        {
            get
            {
                TreeNode p = Parent;
                if (p == null) return null;
                int i = p.Children.IndexOf(this);
                if (i < 0) return null;
                if (i + 1 >= p.mChildren.Count) return null;
                return p.mChildren.Get(i + 1);
            }
        }

        /// <summary>
        /// Gets the previous sibling of <c>this</c>
        /// </summary>
        /// <returns>The previous sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
        public TreeNode PreviousSibling
        {
            get
            {
                TreeNode p = Parent;
                if (p == null) return null;
                int i = p.Children.IndexOf(this);
                if (i < 0) return null;
                if (i == 0) return null;
                return p.mChildren.Get(i - 1);
            }
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
        public bool IsSiblingOf(TreeNode node)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(
                    "The node to test relationship with is null");
            }
            TreeNode p = Parent;
            return (p != null && p == node.Parent);
        }

        public bool IsAncestorOf(TreeNode node)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(
                    "The node to test relationship with is null");
            }
            TreeNode p = node.Parent;
            if (p == null)
            {
                return false;
            }
            else if (p == this)
            {
                return true;
            }
            else
            {
                return IsAncestorOf(p);
            }
        }

        public bool IsDescendantOf(TreeNode node)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(
                    "The node to test relationship with is null");
            }
            return node.IsAncestorOf(this);
        }

        public TreeNode Root
        {
            get
            {
                if (Parent == null)
                {
                    return this;
                }
                return Parent.Root;
            }
        }

        public bool IsAfter(TreeNode node)
        {
            if (node == this)
            {
                return false;
            }
            if (Root.MeetFirst(this, node) == node)
            {
                return true;
            }
            return false;
        }

        public bool IsBefore(TreeNode node)
        {
            if (node == this)
            {
                return false;
            }
            return !IsAfter(node);
        }

        #endregion

        #region ITreeNodeWriteOnlyMethods Members

        public T GetOrCreateProperty<T>() where T : Property, new()
        {
            T prop = GetProperty<T>();
            if (prop == null)
            {
                prop = Presentation.PropertyFactory.Create<T>();
                AddProperty(prop);
            }
            return prop;
        }

        /// <summary>
        /// Adds a <see cref="Property"/> to the node
        /// </summary>
        /// <param name="props">The list of <see cref="Property"/>s to add.</param>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="props"/> is null</exception>
        public void AddProperties(IList<Property> props)
        {
            if (props == null) throw new MethodParameterIsNullException("No list of Property was given");
            foreach (Property p in props)
            {
                AddProperty(p);
            }
        }

        /// <summary>
        /// Adds a <see cref="Property"/> to the node
        /// </summary>
        /// <param name="prop">The <see cref="Property"/> to add. </param>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="prop"/> is null</exception>
        /// <exception cref="exception.PropertyAlreadyHasOwnerException">Thrown when <see cref="Property"/> is already owned by another node</exception>
        /// <exception cref="exception.NodeInDifferentPresentationException">Thrown when the new <see cref="Property"/> belongs to a different <see cref="Presentation"/></exception>
        /// <exception cref="exception.PropertyCanNotBeAddedException">Thrown when <c><paramref name="prop"/>.<see cref="Property.CanBeAddedTo"/>(this)</c> returns <c>false</c></exception>
        public void AddProperty(Property prop)
        {
            if (prop == null)
                throw new MethodParameterIsNullException("Can not add a null Property to the TreeNode");
            if (mProperties.IndexOf(prop) == -1)
            {
                if (!prop.CanBeAddedTo(this))
                {
                    throw new PropertyCanNotBeAddedException(
                        "The given Property can not be added to the TreeNode");
                }
                prop.TreeNodeOwner = this;

                mProperties.Insert(mProperties.Count, prop);

                if (prop is XmlProperty)
                {
                    UpdateTextDirectionality(this, (XmlProperty)prop);
                }
            }
        }

        public static void UpdateTextDirectionality(TreeNode node, XmlProperty xmlProp)
        {
            node.TextDirectionality = TextDirection.Unsure;

            if (xmlProp == null || xmlProp.mLocalName == null)
            {
                return;
            }

            string lang = xmlProp.GetLangFromAttributes();
            if (!String.IsNullOrEmpty(lang))
            {
                // TODO: Arabic, Urdu, Hebrew, Yiddish, Farsi...what else?
                if (lang.Equals("ar")
                           || lang.Equals("ur")
                           || lang.Equals("he")
                           || lang.Equals("ji")
                           || lang.Equals("fa")
                           || lang.StartsWith("ar-")
                           || lang.StartsWith("ur-")
                           || lang.StartsWith("he-")
                           || lang.StartsWith("ji-")
                           || lang.StartsWith("fa-")
                    )
                {
                    node.TextDirectionality = TextDirection.RTL;
                }
                else
                //if (xmlAttr.Value.Equals("en")
                //       || xmlAttr.Value.Equals("fr")
                //       || xmlAttr.Value.StartsWith("en-")
                //       || xmlAttr.Value.StartsWith("fr-")
                //)
                {
                    node.TextDirectionality = TextDirection.LTR;
                }
            }

            XmlAttribute xmlAttr = xmlProp.GetAttribute("dir");
            if (xmlAttr != null && !String.IsNullOrEmpty(xmlAttr.Value))
            {
                if (xmlAttr.Value.Equals("rtl"))
                {
                    node.TextDirectionality = TextDirection.RTL;
                }
                else if (xmlAttr.Value.Equals("ltr"))
                {
                    node.TextDirectionality = TextDirection.LTR;
                }
#if DEBUG
                else
                {
                    Debugger.Break();
                }
#endif //DEBUG
            }
        }

        /// <summary>
        /// Remove the <see cref="Property"/>s of a given <see cref="Type"/> from this
        /// </summary>
        /// <param name="propType">Specify the type of properties to remove</param>
        /// <returns>The list of removed properties</returns>
        public List<Property> RemoveProperties(Type propType)
        {
            List<Property> remProps = GetProperties(propType);
            foreach (Property p in remProps)
            {
                RemoveProperty(p);
            }
            return remProps;
        }

        /// <summary>
        /// Removes a given <see cref="Property"/>
        /// </summary>
        /// <param name="prop">The <see cref="Property"/> to remove</param>
        public void RemoveProperty(Property prop)
        {
            if (prop == null) throw new MethodParameterIsNullException("Can not remove a null Property");
            if (mProperties.IndexOf(prop) != -1)
            {
                prop.TreeNodeOwner = null;
                if (prop is XmlProperty)
                {
                    UpdateTextDirectionality(this, null);
                }
                mProperties.Remove(prop);
            }
        }


        /// <summary>
        /// Inserts a <see cref="TreeNode"/> child at a given index. 
        /// The index of any children at or after the given index are increased by one
        /// </summary>
        /// <param name="node">
        /// The new child <see cref="TreeNode"/> to insert,
        /// must be between 0 and the number of children as returned by member method.
        /// </param>
        /// <param name="insertIndex">The index at which to insert the new child</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="insertIndex"/> is out if range, 
        /// that is not between <c>0</c> and <c>GetmChildren.Count-1</c></exception>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="node"/> is null</exception>
        /// <exception cref="exception.NodeNotDetachedException">
        /// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
        /// </exception>
        public void Insert(TreeNode node, int insertIndex)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(String.Format(
                                                                       "Can not insert null child at index {0:0}",
                                                                       insertIndex));
            }
            if (node.Parent != null)
            {
                throw new NodeNotDetachedException(
                    "Can not insert child node that is already attached to a parent node");
            }
            if (insertIndex < 0 || mChildren.Count < insertIndex)
            {
                throw new MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Could not insert a new child at index {0:0} - index is out of bounds",
                                                                              insertIndex));
            }
            node.mParent = this;
            mChildren.Insert(insertIndex, node);

            node.UpdateTextCache_AfterInsert();
        }

        /// <summary>
        /// Detaches the instance <see cref="TreeNode"/> from it's parent's children
        /// </summary>
        /// <returns>The detached <see cref="TreeNode"/> (i.e. <c>this</c>)</returns>
        public TreeNode Detach()
        {
            mParent.RemoveChild(this);
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
        /// (child indexes range from <c>0</c> to <c>GetmChildren.Count-1</c>)
        /// </exception>
        public TreeNode RemoveChild(int index)
        {
            TreeNode removedChild = mChildren.Get(index);

            removedChild.UpdateTextCache_BeforeRemove();

            removedChild.mParent = null;
            mChildren.Remove(removedChild);
            
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
        public TreeNode RemoveChild(TreeNode node)
        {
            int index = Children.IndexOf(node);
            if (index == -1)
            {
                throw new NodeDoesNotExistException("The given node is not a children of this node !");
            }
            return RemoveChild(index);
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
        public void InsertBefore(TreeNode node, TreeNode anchorNode)
        {
            int index = Children.IndexOf(anchorNode);
            if (index == -1)
            {
                throw new NodeDoesNotExistException("The given node is not a children of this node !");
            }
            Insert(node, index);
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
        public void InsertAfter(TreeNode node, TreeNode anchorNode)
        {
            int index = Children.IndexOf(anchorNode);
            if (index == -1)
            {
                throw new NodeDoesNotExistException("The given node is not a children of this node !");
            }
            Insert(node, index + 1);
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
        /// that is when <paramref localName="index"/> is not between <c>0</c> and <c>GetmChildren.Count-1</c>
        /// </exception>
        /// <exception cref="exception.NodeNotDetachedException">
        /// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
        /// </exception>
        public TreeNode ReplaceChild(TreeNode node, int index)
        {
            TreeNode replacedChild = mChildren.Get(index);
            Insert(node, index);
            replacedChild.Detach();
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
        public TreeNode ReplaceChild(TreeNode node, TreeNode oldNode)
        {
            int index = Children.IndexOf(oldNode);
            if (index == -1)
            {
                throw new NodeDoesNotExistException("The given node is not a children of this node !");
            }
            return ReplaceChild(node, index);
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
        public void AppendChild(TreeNode node)
        {
            Insert(node, mChildren.Count);
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
        /// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="Presentation"/>
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
        public void AppendChildrenOf(TreeNode node)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(
                    "The given node from which to append children is null");
            }
            if (Presentation != node.Presentation)
            {
                throw new NodeInDifferentPresentationException(
                    "Can not append the children of a node from a different presentation");
            }
            if (node == this)
            {
                throw new NodeIsSelfException(
                    "Can not append a nodes own children to itself");
            }
            if (node.IsAncestorOf(this))
            {
                throw new NodeIsAncestorException(
                    "Can not append the children of an ancestor node");
            }
            if (node.IsDescendantOf(this))
            {
                throw new NodeIsDescendantException(
                    "Can not append the children of a descendant node");
            }
            while (node.mChildren.Count > 0)
            {
                AppendChild(node.RemoveChild(0));
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
        /// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="Presentation"/>
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
        public void SwapWith(TreeNode node)
        {
            if (node == null)
            {
                throw new MethodParameterIsNullException(
                    "The given node with which to swap is null");
            }
            if (Presentation != node.Presentation)
            {
                throw new NodeInDifferentPresentationException(
                    "Can not swap with a node from a different presentation");
            }
            if (node == this)
            {
                throw new NodeIsSelfException(
                    "Can not swap with itself");
            }
            if (node.IsAncestorOf(this))
            {
                throw new NodeIsAncestorException(
                    "Can not swap with an ancestor node");
            }
            if (node.IsDescendantOf(this))
            {
                throw new NodeIsDescendantException(
                    "Can not swap with a descendant node");
            }
            if (Parent == null || node.Parent == null)
            {
                throw new NodeHasNoParentException(
                    "Both nodes in a swap need to have a parent");
            }
            TreeNode thisParent = Parent;
            int thisIndex = thisParent.Children.IndexOf(this);
            Detach();
            TreeNode nodeParent = node.Parent;
            nodeParent.InsertAfter(this, node);
            thisParent.Insert(node, thisIndex);
        }

        /// <summary>
        /// Splits <c>this</c> at the child at a given <paramref localName="index"/>, 
        /// producing a new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetmChildren.Count()-1</c> 
        /// and leaving <c>this</c> with the children at indexes <c>0</c> to <paramref localName="index"/>-1
        /// </summary>
        /// <param name="index">The index of the child at which to split</param>
        /// <param name="copyProperties">
        /// A <see cref="bool"/> indicating the <see cref="Property"/>s of <c>this</c> 
        /// should be copied to the new <see cref="TreeNode"/>
        /// </param>
        /// <returns>
        /// The new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetmChildren.Count()-1</c> 
        /// and optionally with a copy of the <see cref="Property"/>s
        /// </returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="index"/> is out of bounds, 
        /// that is not between <c>0</c> and <c>GetmChildren.Count()-1</c>
        /// </exception>
        public TreeNode SplitChildren(int index, bool copyProperties)
        {
            if (index < 0 || mChildren.Count <= index)
            {
                throw new MethodParameterIsOutOfBoundsException(
                    "The given index at which to split children is out of bounds");
            }
            TreeNode res = Copy(false, copyProperties);
            while (index < mChildren.Count)
            {
                res.AppendChild(RemoveChild(index));
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
        public bool SwapWithPreviousSibling()
        {
            TreeNode nextSibling = NextSibling;
            if (nextSibling == null) return false;
            SwapWith(nextSibling);
            return true;
        }

        /// <summary>
        /// Swaps <c>this</c> with the next sibling of <c>this</c>
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/> indicating if the swap was succesfull 
        /// (the swap is not succesfull when there is no next sibling).
        /// </returns>
        public bool SwapWithNextSibling()
        {
            TreeNode prevSibling = PreviousSibling;
            if (prevSibling == null) return false;
            SwapWith(prevSibling);
            return true;
        }

        #endregion

        #region IValueEquatable<TreeNode> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNode otherz = other as TreeNode;
            if (otherz == null)
            {
                return false;
            }

            List<Type> thisProps = UsedPropertyTypes;
            List<Type> otherProps = otherz.UsedPropertyTypes;
            if (thisProps.Count != otherProps.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            foreach (Type pt in thisProps)
            {
                List<Property> thisPs = GetProperties(pt);
                List<Property> otherPs = otherz.GetProperties(pt);
                if (thisPs.Count != otherPs.Count)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                for (int i = 0; i < thisPs.Count; i++)
                {
                    if (!thisPs[i].ValueEquals(otherPs[i]))
                    {
                        //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                        return false;
                    }
                }
            }
            if (mChildren.Count != otherz.mChildren.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            for (int i = 0; i < mChildren.Count; i++)
            {
                if (!mChildren.Get(i).ValueEquals(otherz.mChildren.Get(i)))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}