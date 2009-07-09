using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.core.visitor;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.utilities;
using urakawa.progress;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace urakawa.core
{
    public struct TreeNodeAndStreamDataLength
    {
        public TreeNode m_TreeNode;
        public long m_LocalStreamDataLength;
    }

    public struct StreamWithMarkers
    {
        public Stream m_Stream;
        public List<TreeNodeAndStreamDataLength> m_SubStreamMarkers;
    }

    /// <summary>
    /// A node in the core tree of the SDK
    /// </summary>
    [DebuggerDisplay("{getDebugString()}")]
    public class TreeNode : WithPresentation, ITreeNodeReadOnlyMethods, ITreeNodeWriteOnlyMethods, IVisitableTreeNode,
                            IXukAble, IValueEquatable<TreeNode>, urakawa.events.IChangeNotifier
    {

        public TreeNode GetFirstChildWithXmlElementName(string elemName)
        {
            QualifiedName qname = GetXmlElementQName();
            if (qname != null && qname.LocalName == elemName) return this;

            for (int i = 0; i < ChildCount; i++)
            {
                TreeNode child = GetChild(i).GetFirstChildWithXmlElementName(elemName);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public override string ToString()
        {
            QualifiedName qname = GetXmlElementQName();
            if (qname != null)
            {
                return qname.LocalName;
            }
            return base.ToString();
        }

        ///<summary>
        /// returns the QName of the attached XmlProperty, if any
        ///</summary>
        ///<returns></returns>
        public QualifiedName GetXmlElementQName()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                return new QualifiedName(xmlProp.LocalName, xmlProp.NamespaceUri);
            }
            return null;
        }
        ///<summary>
        /// returns the ID attribute value of the attached XmlProperty, if any
        ///</summary>
        ///<returns>null of there is no ID attribute</returns>
        public string GetXmlElementId()
        {
            XmlProperty xmlProp = GetProperty<XmlProperty>();
            if (xmlProp != null)
            {
                XmlAttribute idAttr = xmlProp.GetAttribute("id", "");
                if (idAttr != null)
                {
                    return (string.IsNullOrEmpty(idAttr.Value) ? null : idAttr.Value);
                }
            }
            return null;
        }

        protected string getDebugString()
        {
            QualifiedName qname = GetXmlElementQName();
            String str = (qname != null ? qname.LocalName : "");
            str += "///";
            str += GetTextMediaFlattened();
            return str;
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
            return !IsAfter(node);
        }

        private TreeNode MeetFirst(TreeNode node1, TreeNode node2)
        {
            if (this == node1) return node1;
            if (this == node2) return node2;
            foreach (TreeNode child in ListOfChildren)
            {
                TreeNode met = child.MeetFirst(node1, node2);
                if (met != null)
                {
                    return met;
                }
            }
            return null;
        }

        public TreeNode GetFirstAncestorWithManagedAudio()
        {
            if (Parent == null)
            {
                return null;
            }
            ManagedAudioMedia audioMedia = Parent.GetAudioMedia() as ManagedAudioMedia;
            if (audioMedia != null) // && audioMedia.AudioMediaData != null
            {
                return Parent;
            }
            else
            {
                SequenceMedia seq = Parent.GetAudioSequenceMedia();
                if (seq != null && !seq.AllowMultipleTypes)
                {
                    foreach (Media media in seq.ListOfItems)
                    {
                        if (media is ManagedAudioMedia)
                        {
                            return Parent;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return Parent.GetFirstAncestorWithManagedAudio();
        }

        public TreeNode GetFirstDescendantWithManagedAudio()
        {
            if (ChildCount == 0)
            {
                return null;
            }

            foreach (TreeNode child in ListOfChildren)
            {
                ManagedAudioMedia audioMedia = child.GetManagedAudioMedia();
                if (audioMedia != null) // && audioMedia.AudioMediaData != null
                {
                    return child;
                }
                else
                {
                    SequenceMedia seq = child.GetAudioSequenceMedia();
                    if (seq != null && !seq.AllowMultipleTypes)
                    {
                        foreach (Media media in seq.ListOfItems)
                        {
                            if (media is ManagedAudioMedia)
                            {
                                return child;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                TreeNode childIn = child.GetFirstDescendantWithManagedAudio();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetNextSiblingWithManagedAudio()
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                ManagedAudioMedia audioMedia = next.GetManagedAudioMedia();
                if (audioMedia != null) // && audioMedia.AudioMediaData != null
                {
                    return next;
                }
                else
                {
                    SequenceMedia seq = next.GetAudioSequenceMedia();
                    if (seq != null && !seq.AllowMultipleTypes)
                    {
                        foreach (Media media in seq.ListOfItems)
                        {
                            if (media is ManagedAudioMedia)
                            {
                                return next;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                TreeNode nextIn = next.GetFirstDescendantWithManagedAudio();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            return Parent.GetNextSiblingWithManagedAudio();
        }

        public StreamWithMarkers? GetManagedAudioData()
        {
            StreamWithMarkers val;

            ManagedAudioMedia audioMedia = GetManagedAudioMedia();
            if (audioMedia != null && audioMedia.AudioMediaData != null)
            {
                val.m_Stream = audioMedia.AudioMediaData.GetAudioData();
                val.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>(1);
                TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                tnasdl.m_TreeNode = this;
                val.m_SubStreamMarkers.Add(tnasdl);
                return val;
            }
            else
            {
                SequenceMedia seq = GetAudioSequenceMedia();
                if (seq != null)
                {
                    Stream stream = seq.GetManagedAudioMediaDataStream();
                    if (stream != null)
                    {
                        val.m_Stream = stream;
                        val.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>(1);
                        TreeNodeAndStreamDataLength tnasdl = new TreeNodeAndStreamDataLength();
                        tnasdl.m_LocalStreamDataLength = val.m_Stream.Length;
                        tnasdl.m_TreeNode = this;
                        val.m_SubStreamMarkers.Add(tnasdl);
                        return val;
                    }
                }
            }
            return null;
        }

        public StreamWithMarkers? GetManagedAudioDataFlattened()
        {
            StreamWithMarkers? val = GetManagedAudioData();
            if (val != null)
            {
                return val;
            }

            List<StreamWithMarkers> listStreamsWithMarkers = new List<StreamWithMarkers>();

            for (int index = 0; index < ChildCount; index++)
            {
                TreeNode node = GetChild(index);
                StreamWithMarkers? childVal = node.GetManagedAudioDataFlattened();
                if (childVal != null)
                {
                    listStreamsWithMarkers.Add(childVal.GetValueOrDefault());
                }
            }

            if (listStreamsWithMarkers.Count == 0)
            {
                return null;
            }

            StreamWithMarkers returnVal = new StreamWithMarkers();
            returnVal.m_SubStreamMarkers = new List<TreeNodeAndStreamDataLength>();

            List<Stream> listStreams = new List<Stream>();
            foreach (StreamWithMarkers strct in listStreamsWithMarkers)
            {
                listStreams.Add(strct.m_Stream);
                returnVal.m_SubStreamMarkers.AddRange(strct.m_SubStreamMarkers);
                strct.m_SubStreamMarkers.Clear();
            }

            returnVal.m_Stream = new SequenceStream(listStreams);

            listStreamsWithMarkers.Clear();
            listStreamsWithMarkers = null;

            return returnVal;
        }

        public TreeNode GetLastDescendantWithText()
        {
            if (ChildCount == 0)
            {
                return null;
            }

            for (int i = ListOfChildren.Count - 1; i >= 0; i--)
            {
                TreeNode child = ListOfChildren[i];

                string str = child.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetLastDescendantWithText();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetFirstDescendantWithText()
        {
            if (ChildCount == 0)
            {
                return null;
            }

            foreach (TreeNode child in ListOfChildren)
            {
                string str = child.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return child;
                }

                TreeNode childIn = child.GetFirstDescendantWithText();
                if (childIn != null)
                {
                    return childIn;
                }
            }
            return null;
        }

        public TreeNode GetPreviousSiblingWithText()
        {
            return GetPreviousSiblingWithText(null);
        }

        public TreeNode GetPreviousSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.PreviousSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetLastDescendantWithText();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetPreviousSiblingWithText(upLimit);
            }
            return null;
        }

        public TreeNode GetNextSiblingWithText()
        {
            return GetNextSiblingWithText(null);
        }

        public TreeNode GetNextSiblingWithText(TreeNode upLimit)
        {
            if (Parent == null)
            {
                return null;
            }
            TreeNode next = this;
            while ((next = next.NextSibling) != null)
            {
                string str = next.GetTextMediaFlattened(false);
                if (!string.IsNullOrEmpty(str))
                {
                    return next;
                }

                TreeNode nextIn = next.GetFirstDescendantWithText();
                if (nextIn != null)
                {
                    return nextIn;
                }
            }

            if (upLimit == null || upLimit != Parent)
            {
                return Parent.GetNextSiblingWithText(upLimit);
            }
            return null;
        }

        public string GetTextMediaFlattened()
        {
            return GetTextMediaFlattened(true);
        }

        private string GetTextMediaFlattened(bool deep)
        {
            AbstractTextMedia textMedia = GetTextMedia();
            if (textMedia != null)
            {
                if (textMedia.Text.Length == 0)
                {
                    return null;
                }
                return textMedia.Text;
            }
            else
            {
                SequenceMedia seq = GetTextSequenceMedia();
                if (seq != null)
                {
                    String strText = seq.GetMediaText();
                    if (!String.IsNullOrEmpty(strText))
                    {
                        return strText;
                    }
                }
            }

            if (!deep)
            {
                return null;
            }

            string str = "";
            for (int index = 0; index < ChildCount; index++)
            {
                TreeNode node = GetChild(index);
                str += node.GetTextMediaFlattened();
            }
            if (str.Length == 0)
            {
                return null;
            }
            return str;
        }

        public AbstractImageMedia GetImageMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as AbstractImageMedia;
            }
            return null;
        }

        public SequenceMedia GetImageSequenceMedia()
        {
            Media med = GetMediaInImageChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }

        public AbstractTextMedia GetTextMedia()
        {
            Media med = GetMediaInTextChannel();
            if (med != null)
            {
                return med as AbstractTextMedia;
            }
            return null;
        }

        public SequenceMedia GetTextSequenceMedia()
        {
            Media med = GetMediaInTextChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }

        public Media GetMediaInImageChannel()
        {
            ChannelsProperty chProp = GetProperty<ChannelsProperty>();
            if (chProp != null)
            {
                Channel channel = null;
                List<Channel> listCh = Presentation.ChannelsManager.ListOfChannels;
                foreach (Channel ch in listCh)
                {
                    if (ch is ImageChannel)
                    {
                        channel = ch;
                        break;
                    }
                }
                if (channel != null)
                {
                    Media med = chProp.GetMedia(channel);
                    return med;
                }
            }
            return null;
        }

        public Media GetMediaInTextChannel()
        {
            ChannelsProperty chProp = GetProperty<ChannelsProperty>();
            if (chProp != null)
            {
                Channel channel = null;
                List<Channel> listCh = Presentation.ChannelsManager.ListOfChannels;
                foreach (Channel ch in listCh)
                {
                    if (ch is TextChannel)
                    {
                        channel = ch;
                        break;
                    }
                }
                if (channel != null)
                {
                    Media med = chProp.GetMedia(channel);
                    return med;
                }
            }
            return null;
        }

        public ManagedAudioMedia GetManagedAudioMedia()
        {
            AbstractAudioMedia media = GetAudioMedia();
            if (media != null)
            {
                return media as ManagedAudioMedia;
            }
            return null;
        }

        public AbstractAudioMedia GetAudioMedia()
        {
            Media med = GetMediaInAudioChannel();
            if (med != null)
            {
                return med as AbstractAudioMedia;
            }
            return null;
        }
        public SequenceMedia GetAudioSequenceMedia()
        {
            Media med = GetMediaInAudioChannel();
            if (med != null)
            {
                return med as SequenceMedia;
            }
            return null;
        }

        public Media GetMediaInAudioChannel()
        {
            ChannelsProperty chProp = GetProperty<ChannelsProperty>();
            if (chProp != null)
            {
                Channel channel = null;
                List<Channel> listCh = Presentation.ChannelsManager.ListOfChannels;
                foreach (Channel ch in listCh)
                {
                    if (ch is AudioChannel)
                    {
                        channel = ch;
                        break;
                    }
                }
                if (channel != null)
                {
                    Media med = chProp.GetMedia(channel);
                    return med;
                }
            }
            return null;
        }


        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNode;
        }
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="TreeNode"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        /// <summary>
        /// Event fired after the <see cref="TreeNode"/> has been added as a child 
        /// of another <see cref="TreeNode"/> (now it's parent)
        /// </summary>
        public event EventHandler<urakawa.events.core.ChildAddedEventArgs> ChildAdded;

        /// <summary>
        /// Fires the <see cref="ChildAdded"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="TreeNode"/> at which the event occured
        /// </param>
        /// <param name="addedChild">
        /// The child <see cref="TreeNode"/> that was added to <paramref name="source"/>
        /// </param>
        protected void NotifyChildAdded(TreeNode source, TreeNode addedChild)
        {
            EventHandler<urakawa.events.core.ChildAddedEventArgs> d = ChildAdded;
            if (d != null) d(this, new urakawa.events.core.ChildAddedEventArgs(source, addedChild));
        }

        /// <summary>
        /// Event fired after the <see cref="TreeNode"/> has been removed as a child 
        /// of another <see cref="TreeNode"/> (porperly it's parent)
        /// </summary>
        public event EventHandler<urakawa.events.core.ChildRemovedEventArgs> ChildRemoved;

        /// <summary>
        /// Fires the <see cref="ChildRemoved"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="TreeNode"/> at which the event occured, 
        /// i.e. the <see cref="TreeNode"/> from which a child was removed
        /// </param>
        /// <param name="removedChild">The child that was removed</param>
        /// <param name="position">The position from which the child was removed</param>
        protected void NotifyChildRemoved(TreeNode source, TreeNode removedChild, int position)
        {
            EventHandler<urakawa.events.core.ChildRemovedEventArgs> d = ChildRemoved;
            if (d != null) d(this, new urakawa.events.core.ChildRemovedEventArgs(source, removedChild, position));
        }

        /// <summary>
        /// Event fired after a <see cref="Property"/> has been added to a <see cref="TreeNode"/>
        /// </summary>
        public event EventHandler<urakawa.events.core.PropertyAddedEventArgs> PropertyAdded;

        /// <summary>
        /// Fires the <see cref="PropertyAdded"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="TreeNode"/> to which a <see cref="Property"/> was added
        /// </param>
        /// <param name="addedProp">The <see cref="Property"/> that was added</param>
        protected void NotifyPropertyAdded(TreeNode source, Property addedProp)
        {
            EventHandler<urakawa.events.core.PropertyAddedEventArgs> d = PropertyAdded;
            if (d != null) d(this, new urakawa.events.core.PropertyAddedEventArgs(source, addedProp));
        }

        /// <summary>
        /// Event fired after a <see cref="Property"/> has been removed from a <see cref="TreeNode"/>
        /// </summary>
        public event EventHandler<urakawa.events.core.PropertyRemovedEventArgs> PropertyRemoved;

        /// <summary>
        /// Fires the <see cref="PropertyRemoved"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="TreeNode"/> to which a <see cref="Property"/> was added
        /// </param>
        /// <param name="removedProp">The <see cref="Property"/> that was removed</param>
        protected void NotifyPropertyRemoved(TreeNode source, Property removedProp)
        {
            EventHandler<urakawa.events.core.PropertyRemovedEventArgs> d = PropertyRemoved;
            if (d != null) d(this, new urakawa.events.core.PropertyRemovedEventArgs(source, removedProp));
        }

        private void this_childAdded(object sender, urakawa.events.core.ChildAddedEventArgs e)
        {
            e.AddedChild.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Child_Changed);
            NotifyChanged(e);
        }

        private void Child_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private void this_childRemoved(object sender, urakawa.events.core.ChildRemovedEventArgs e)
        {
            e.RemovedChild.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Child_Changed);
            NotifyChanged(e);
        }

        private void this_propertyAdded(object sender, urakawa.events.core.PropertyAddedEventArgs e)
        {
            e.AddedProperty.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Property_Changed);
            NotifyChanged(e);
        }

        private void Property_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private void this_propertyRemoved(object sender, urakawa.events.core.PropertyRemovedEventArgs e)
        {
            e.RemovedProperty.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Property_Changed);
            NotifyChanged(e);
        }

        #endregion

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
        /// Default constructor - for system use only, 
        /// <see cref="TreeNode"/>s should only be created via. the <see cref="TreeNodeFactory"/>
        /// </summary>
        public TreeNode()
        {
            mProperties = new List<Property>();
            mChildren = new List<TreeNode>();
            ChildAdded += new EventHandler<urakawa.events.core.ChildAddedEventArgs>(this_childAdded);
            ChildRemoved += new EventHandler<urakawa.events.core.ChildRemovedEventArgs>(this_childRemoved);
            PropertyAdded += new EventHandler<urakawa.events.core.PropertyAddedEventArgs>(this_propertyAdded);
            PropertyRemoved += new EventHandler<urakawa.events.core.PropertyRemovedEventArgs>(this_propertyRemoved);
        }

        /// <summary>
        /// Gets a list of the <see cref="Type"/>s of <see cref="Property"/> set for the <see cref="TreeNode"/>
        /// </summary>
        /// <returns>The list</returns>
        public List<Type> ListOfUsedPropertyTypes
        {
            get
            {
                List<Type> res = new List<Type>();
                foreach (Property p in GetListOfProperties())
                {
                    if (!res.Contains(p.GetType())) res.Add(p.GetType());
                }
                return res;
            }
        }

        /// <summary>
        /// Gets a list of all <see cref="Property"/>s of this
        /// </summary>
        /// <returns>The list</returns>
        public List<Property> GetListOfProperties()
        {
            return new List<Property>(mProperties);
        }

        /// <summary>
        /// Gets a list of the <see cref="Property"/>s of this of a given <see cref="Type"/>
        /// </summary>
        /// <param name="t">The given type</param>
        /// <returns>The list</returns>
        public List<Property> GetListOfProperties(Type t)
        {
            List<Property> res = new List<Property>();
            foreach (Property p in GetListOfProperties())
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
        public List<T> GetListOfProperties<T>() where T : Property
        {
            List<T> res = new List<T>();
            foreach (Property p in GetListOfProperties(typeof(T))) res.Add(p as T);
            return res;
        }

        /// <summary>
        /// Gets the first <see cref="Property"/> of a the given <see cref="Property"/> sub-type
        /// </summary>
        /// <param name="t">The given <see cref="Property"/> subtype</param>
        /// <returns>The first property of the given subtype - possibly null</returns>
        public Property GetProperty(Type t)
        {
            List<Property> props = GetListOfProperties(t);
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
        /// Adds a <see cref="Property"/> to the node
        /// </summary>
        /// <param name="props">The list of <see cref="Property"/>s to add.</param>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="props"/> is null</exception>
        public void AddProperties(IList<Property> props)
        {
            if (props == null) throw new exception.MethodParameterIsNullException("No list of Property was given");
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
                throw new exception.MethodParameterIsNullException("Can not add a null Property to the TreeNode");
            if (!mProperties.Contains(prop))
            {
                if (!prop.CanBeAddedTo(this))
                {
                    throw new exception.PropertyCanNotBeAddedException(
                        "The given Property can not be added to the TreeNode");
                }
                prop.TreeNodeOwner = this;
                mProperties.Add(prop);
                NotifyPropertyAdded(this, prop);
            }
        }

        /// <summary>
        /// Remove the <see cref="Property"/>s of a given <see cref="Type"/> from this
        /// </summary>
        /// <param name="propType">Specify the type of properties to remove</param>
        /// <returns>The list of removed properties</returns>
        public List<Property> RemoveProperties(Type propType)
        {
            List<Property> remProps = GetListOfProperties(propType);
            foreach (Property p in remProps)
            {
                RemoveProperty(p);
            }
            return remProps;
        }

        /// <summary>
        /// Removes all <see cref="Property"/>s from this
        /// </summary>
        public void RemoveProperties()
        {
            foreach (Property p in GetListOfProperties())
            {
                RemoveProperty(p);
            }
        }

        /// <summary>
        /// Removes a given <see cref="Property"/>
        /// </summary>
        /// <param name="prop">The <see cref="Property"/> to remove</param>
        public void RemoveProperty(Property prop)
        {
            if (prop == null) throw new exception.MethodParameterIsNullException("Can not remove a null Property");
            if (mProperties.Contains(prop))
            {
                mProperties.Remove(prop);
                prop.TreeNodeOwner = null;
                NotifyPropertyRemoved(this, prop);
            }
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
            foreach (Property p in GetListOfProperties())
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
                throw new exception.MethodParameterIsNullException("The TreeNode can not have a null Property");
            return mProperties.Contains(prop);
        }


        /// <summary>
        /// Copies the children of the current instance to a given destination <see cref="TreeNode"/>
        /// </summary>
        /// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
        /// <remarks>The children are copied deep and any existing children of the destination <see cref="TreeNode"/>
        /// are not removed</remarks>
        protected void CopyChildren(TreeNode destinationNode)
        {
            for (int i = 0; i < this.ChildCount; i++)
            {
                destinationNode.AppendChild(GetChild(i).Copy(true));
            }
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
                for (int i = 0; i < ChildCount; i++)
                {
                    GetChild(i).AcceptDepthFirst(preVisit, postVisit);
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
                for (int i = 0; i < next.ChildCount; i++)
                {
                    nodeQueue.Enqueue(next.GetChild(i));
                }
            }
        }

        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="TreeNode"/> removing all children and <see cref="Property"/>s
        /// </summary>
        protected override void Clear()
        {
            foreach (TreeNode child in this.ListOfChildren)
            {
                RemoveChild(child);
            }
            foreach (Property prop in this.GetListOfProperties())
            {
                RemoveProperty(prop);
            }
            base.Clear();
        }

        private void XukInProperties(XmlReader source, ProgressHandler handler)
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
                            AddProperty(newProp);
                            newProp.XukIn(source, handler);
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
        private void XukInChildNode(XmlReader source, ProgressHandler handler)
        {
            TreeNode newChild = Presentation.TreeNodeFactory.Create(source.LocalName, source.NamespaceURI);
            if (newChild != null)
            {
                AppendChild(newChild);
                newChild.XukIn(source, handler);
            }
            else if (!source.IsEmptyElement)
            {
                //Read past unidentified element
                source.ReadSubtree().Close();
            }
        }
        private void XukInChildren(XmlReader source, ProgressHandler handler)
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
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        /// <summary>
        /// Reads a child of a TreeNode xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Properties)
                {
                    XukInProperties(source, handler);
                }
                else if (IsPrettyFormat() && source.LocalName == XukStrings.Children)
                {
                    XukInChildren(source, handler);
                }
                else if (!IsPrettyFormat())
                {
                    XukInChildNode(source, handler);
                }
                else
                {
                    readItem = false;
                }
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Properties, XukNamespaceUri);
            foreach (Property prop in GetListOfProperties())
            {
                prop.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            if (IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.Children, XukNamespaceUri);
            }
            for (int i = 0; i < this.ChildCount; i++)
            {
                GetChild(i).XukOut(destination, baseUri, handler);
            }
            if (IsPrettyFormat())
            {
                destination.WriteEndElement();
            }
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
        public int IndexOf(TreeNode node)
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
        /// that is not between <c>0</c> and <c>GetChildCount-1</c></exception>
        public TreeNode GetChild(int index)
        {
            if (index < 0 || mChildren.Count <= index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Could not get child at index {0:0} - index is out of bounds",
                                                                              index));
            }
            return mChildren[index];
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
        /// Gets the number of children
        /// </summary>
        /// <returns>The number of children</returns>
        public int ChildCount
        {
            get { return mChildren.Count; }
        }

        /// <summary>
        /// Gets a list of the child <see cref="TreeNode"/>s of this
        /// </summary>
        /// <returns>The list</returns>
        public List<TreeNode> ListOfChildren
        {
            get { return new List<TreeNode>(mChildren); }
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
        /// Copies the <see cref="Property"/>s of the current instance to a given destination <see cref="TreeNode"/>
        /// </summary>
        /// <param name="destinationNode">The destination <see cref="TreeNode"/></param>
        protected void CopyProperties(TreeNode destinationNode)
        {
            foreach (Property prop in GetListOfProperties())
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
                throw new exception.MethodParameterIsNullException("Can not export the TreeNode to a null Presentation");
            }
            TreeNode exportedNode = destPres.TreeNodeFactory.Create(GetType());
            if (exportedNode == null)
            {
                string msg = String.Format(
                    "The TreeNodeFactory of the export destination Presentation can not create a TreeNode matching Xuk QName {1}:{0}",
                    XukLocalName, XukNamespaceUri);
                throw new exception.FactoryCannotCreateTypeException(msg);
            }
            foreach (Property prop in GetListOfProperties())
            {
                exportedNode.AddProperty(prop.Export(destPres));
            }
            foreach (TreeNode child in ListOfChildren)
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
                int i = p.IndexOf(this);
                if (i + 1 >= p.ChildCount) return null;
                return p.GetChild(i + 1);
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
                int i = p.IndexOf(this);
                if (i == 0) return null;
                return p.GetChild(i - 1);
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
                throw new exception.MethodParameterIsNullException(
                    "The node to test relationship with is null");
            }
            TreeNode p = Parent;
            return (p != null && p == node.Parent);
        }

        public bool IsAncestorOf(TreeNode node)
        {
            if (node == null)
            {
                throw new exception.MethodParameterIsNullException(
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
                throw new exception.MethodParameterIsNullException(
                    "The node to test relationship with is null");
            }
            return node.IsAncestorOf(this);
        }

        #endregion

        #region ITreeNodeWriteOnlyMethods Members

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
        /// that is not between <c>0</c> and <c>GetChildCount-1</c></exception>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="node"/> is null</exception>
        /// <exception cref="exception.NodeNotDetachedException">
        /// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
        /// </exception>
        public void Insert(TreeNode node, int insertIndex)
        {
            if (node == null)
            {
                throw new exception.MethodParameterIsNullException(String.Format(
                                                                       "Can not insert null child at index {0:0}",
                                                                       insertIndex));
            }
            if (node.Parent != null)
            {
                throw new exception.NodeNotDetachedException(
                    "Can not insert child node that is already attached to a parent node");
            }
            if (insertIndex < 0 || mChildren.Count < insertIndex)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Could not insert a new child at index {0:0} - index is out of bounds",
                                                                              insertIndex));
            }
            mChildren.Insert(insertIndex, node);
            node.mParent = this;
            NotifyChildAdded(this, node);
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
        /// (child indexes range from <c>0</c> to <c>GetChildCount-1</c>)
        /// </exception>
        public TreeNode RemoveChild(int index)
        {
            TreeNode removedChild = GetChild(index);
            removedChild.mParent = null;
            mChildren.RemoveAt(index);
            NotifyChildRemoved(this, removedChild, index);
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
            int index = IndexOf(node);
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
            int index = IndexOf(anchorNode);
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
            int index = IndexOf(anchorNode) + 1;
            Insert(node, index);
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
        /// that is when <paramref localName="index"/> is not between <c>0</c> and <c>GetChildCount-1</c>
        /// </exception>
        /// <exception cref="exception.NodeNotDetachedException">
        /// Thrown when <paramref localName="node"/> is already attached as a child of a parent 
        /// </exception>
        public TreeNode ReplaceChild(TreeNode node, int index)
        {
            TreeNode replacedChild = GetChild(index);
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
            return ReplaceChild(node, IndexOf(oldNode));
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
            Insert(node, ChildCount);
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
                throw new exception.MethodParameterIsNullException(
                    "The given node from which to append children is null");
            }
            if (Presentation != node.Presentation)
            {
                throw new exception.NodeInDifferentPresentationException(
                    "Can not append the children of a node from a different presentation");
            }
            if (node == this)
            {
                throw new exception.NodeIsSelfException(
                    "Can not append a nodes own children to itself");
            }
            if (node.IsAncestorOf(this))
            {
                throw new exception.NodeIsAncestorException(
                    "Can not append the children of an ancestor node");
            }
            if (node.IsDescendantOf(this))
            {
                throw new exception.NodeIsDescendantException(
                    "Can not append the children of a descendant node");
            }
            while (node.ChildCount > 0)
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
                throw new exception.MethodParameterIsNullException(
                    "The given node with which to swap is null");
            }
            if (Presentation != node.Presentation)
            {
                throw new exception.NodeInDifferentPresentationException(
                    "Can not swap with a node from a different presentation");
            }
            if (node == this)
            {
                throw new exception.NodeIsSelfException(
                    "Can not swap with itself");
            }
            if (node.IsAncestorOf(this))
            {
                throw new exception.NodeIsAncestorException(
                    "Can not swap with an ancestor node");
            }
            if (node.IsDescendantOf(this))
            {
                throw new exception.NodeIsDescendantException(
                    "Can not swap with a descendant node");
            }
            if (Parent == null || node.Parent == null)
            {
                throw new exception.NodeHasNoParentException(
                    "Both nodes in a swap need to have a parent");
            }
            TreeNode thisParent = Parent;
            int thisIndex = thisParent.IndexOf(this);
            Detach();
            TreeNode nodeParent = node.Parent;
            nodeParent.InsertAfter(this, node);
            thisParent.Insert(node, thisIndex);
        }

        /// <summary>
        /// Splits <c>this</c> at the child at a given <paramref localName="index"/>, 
        /// producing a new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetChildCount()-1</c> 
        /// and leaving <c>this</c> with the children at indexes <c>0</c> to <paramref localName="index"/>-1
        /// </summary>
        /// <param name="index">The index of the child at which to split</param>
        /// <param name="copyProperties">
        /// A <see cref="bool"/> indicating the <see cref="Property"/>s of <c>this</c> 
        /// should be copied to the new <see cref="TreeNode"/>
        /// </param>
        /// <returns>
        /// The new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetChildCount()-1</c> 
        /// and optionally with a copy of the <see cref="Property"/>s
        /// </returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="index"/> is out of bounds, 
        /// that is not between <c>0</c> and <c>GetChildCount()-1</c>
        /// </exception>
        public TreeNode SplitChildren(int index, bool copyProperties)
        {
            if (index < 0 || ChildCount <= index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given index at which to split children is out of bounds");
            }
            TreeNode res = Copy(false, copyProperties);
            while (index < ChildCount)
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

        /// <summary>
        /// Compares <c>this</c> with another given <see cref="TreeNode"/> to test for equality. 
        /// The comparison is deep in that any child <see cref="TreeNode"/>s are also tested,
        /// but the ancestry is not tested
        /// </summary>
        /// <param name="other">The other <see cref="TreeNode"/></param>
        /// <returns><c>true</c> if the <see cref="TreeNode"/>s are equal, otherwise <c>false</c></returns>
        public virtual bool ValueEquals(TreeNode other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (other.GetType() != this.GetType())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            List<Type> thisProps = ListOfUsedPropertyTypes;
            List<Type> otherProps = other.ListOfUsedPropertyTypes;
            if (thisProps.Count != otherProps.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            foreach (Type pt in thisProps)
            {
                List<Property> thisPs = GetListOfProperties(pt);
                List<Property> otherPs = other.GetListOfProperties(pt);
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
            if (ChildCount != other.ChildCount)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            for (int i = 0; i < ChildCount; i++)
            {
                if (!GetChild(i).ValueEquals(other.GetChild(i)))
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