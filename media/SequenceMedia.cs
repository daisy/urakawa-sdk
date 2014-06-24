using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.data;
using urakawa.media.data.audio;
using urakawa.media.data.utilities;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media
{
            
#if ENABLE_SEQ_MEDIA

    /// <summary>
    /// SequenceMedia is a collection of same-type media objects
    /// The first object in the collection determines the collection's type.
    /// </summary>
    public class SequenceMedia : Media
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.SequenceMedia;
        }
        private void Item_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        private ObjectListProvider<Media> mSequence;

        public ObjectListProvider<Media> ChildMedias
        {
            get
            {
                return mSequence;
            }
        }

        private bool mAllowMultipleTypes;

        private void Reset()
        {
            mAllowMultipleTypes = false;
            foreach (Media item in mSequence.ContentsAs_ListCopy)
            {
                RemoveItem(item);
            }
        }

        /// <summary>
        /// Constructor setting the associated <see cref="MediaFactory"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="fact"/> is <c>null</c>
        /// </exception>
        public SequenceMedia()
        {
            mSequence = new ObjectListProvider<Media>(this, true);
            Reset();
        }


        /// <summary>
        /// Inserts a given <see cref="Media"/> item at a given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <param name="newItem">The given <see cref="Media"/> item</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the given <see cref="Media"/> to insert is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given index is out of bounds
        /// </exception>
        /// <exception cref="exception.MediaNotAcceptable">
        ///	Thrown if the <see cref="SequenceMedia"/> can not accept the media
        /// </exception>
        public void InsertItem(int index, Media newItem)
        {
            if (newItem == null)
            {
                throw new exception.MethodParameterIsNullException("The new item can not be null");
            }
            if (index < 0 || mSequence.Count < index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The index at which to insert media is out of bounds");
            }
            if (!CanAcceptMedia(newItem))
            {
                throw new exception.MediaNotAcceptable(
                    "The new media to insert is of a type that is incompatible with the sequence media");
            }
            mSequence.Insert(index, newItem);

            newItem.Changed += Item_Changed;

        }


        /// <summary>
        /// Remove an item from the sequence.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The removed <see cref="Media"/> item</returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given index is out of bounds
        /// </exception>
        public Media RemoveItem(int index)
        {
            Media removedMedia = mSequence.Get(index);
            RemoveItem(removedMedia);
            return removedMedia;
        }

        /// <summary>
        /// Removes a given <see cref="Media"/> item from the sequence
        /// </summary>
        /// <param name="item">The item</param>
        /// <exception cref="exception.MediaNotInSequenceException">
        /// Thrown when the given item is not part of the sequence
        /// </exception>
        public void RemoveItem(Media item)
        {
            if (mSequence.IndexOf(item) == -1)
            {
                throw new exception.MediaNotInSequenceException(
                    "Cannot remove a Media item that is not part of the sequence");
            }
            mSequence.Remove(item);

            item.Changed -= Item_Changed;

        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if multiple <see cref="Media"/> types are allowed in the sequence
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool AllowMultipleTypes
        {
            get { return mAllowMultipleTypes; }
            set
            {
                if (!value)
                {
                    int count = mSequence.Count;
                    if (count > 0)
                    {
                        Type firstItemType = mSequence.Get(0).GetType();
                        int i = 1;
                        while (i < count)
                        {
                            if (mSequence.Get(i).GetType() != firstItemType)
                            {
                                throw new exception.OperationNotValidException(
                                    "Can not prohibit multiple Media types in the sequence, since the Type of the sequence items differ");
                            }
                        }
                    }
                }
                mAllowMultipleTypes = value;
            }
        }

        #region Media Members

        /// <summary>
        /// Use the first item in the collection to determine if this sequence is continuous or not.
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get
            {
                if (mSequence.Count > 0)
                {
                    return mSequence.Get(0).IsContinuous;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Use the first item in the collection to determine if this 
        /// sequence is discrete or not.
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get
            {
                //use the first item in the collection to determine the value
                if (mSequence.Count > 0)
                {
                    return mSequence.Get(0).IsDiscrete;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This function always returns true, because this 
        /// object is always considered to be a sequence (even if it contains only one item).
        /// </summary>
        /// <returns><c>true</c></returns>
        public override bool IsSequence
        {
            get { return true; }
        }

        /// <summary>
        /// Make a copy of this media sequence
        /// </summary>
        /// <returns>The copy</returns>
        public new SequenceMedia Copy()
        {
            return CopyProtected() as SequenceMedia;
        }

        /// <summary>
        /// Make a copy of this media sequence
        /// </summary>
        /// <returns>The copy</returns>
        protected override Media CopyProtected()
        {
            SequenceMedia newSeqMedia = (SequenceMedia)base.CopyProtected();
            foreach (Media item in mSequence.ContentsAs_Enumerable)
            {
                //newSeqMedia.mSequence.Add(item.Copy());
                newSeqMedia.InsertItem(mSequence.Count, item.Copy());
            }
            return newSeqMedia;
        }

        /// <summary>
        /// Exports the sequence media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported sequence media</returns>
        public new SequenceMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as SequenceMedia;
        }

        /// <summary>
        /// Exports the sequence media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported sequence media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            SequenceMedia newSeqMedia = (SequenceMedia)base.ExportProtected(destPres);
            foreach (Media item in mSequence.ContentsAs_Enumerable)
            {
                //newSeqMedia.mSequence.Add(item.Export(destPres));
                newSeqMedia.InsertItem(mSequence.Count, item.Export(destPres));
            }
            return newSeqMedia;
        }

        #endregion

        /// <summary>
        /// Test a new media object to see if it can belong to this collection 
        /// (optionally a sequence can allow only a single <see cref="Type"/> of <see cref="Media"/>)
        /// </summary>
        /// <param name="proposedAddition">The media to test</param>
        /// <returns></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the proposed addition is null
        /// </exception>
        public virtual bool CanAcceptMedia(Media proposedAddition)
        {
            if (proposedAddition == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The proposed addition is null");
            }
            if (mSequence.Count > 0 && !AllowMultipleTypes)
            {
                if (mSequence.Get(0).GetType() != proposedAddition.GetType()) return false;
            }
            return true;
        }

        #region IXUKAble members

        /// <summary>
        /// Clears/resets the <see cref="SequenceMedia"/> 
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a SequenceMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string val = source.GetAttribute(XukStrings.AllowMultipleMediaTypes);
            if (val == "true" || val == "1")
            {
                AllowMultipleTypes = true;
            }
            else
            {
                AllowMultipleTypes = false;
            }
        }

        /// <summary>
        /// Reads a child of a SequenceMedia xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Sequence)
                {
                    XukInSequence(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!readItem) base.XukIn(source, handler);
        }

        private void XukInSequence(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Media newMedia = Presentation.MediaFactory.Create(source.LocalName, source.NamespaceURI);
                        if (newMedia != null)
                        {
                            newMedia.XukIn(source, handler);
                            if (!CanAcceptMedia(newMedia))
                            {
                                throw new exception.XukException(
                                    String.Format("Media type {0} is not supported by the sequence",
                                                  newMedia.GetType().FullName));
                            }
                            InsertItem(mSequence.Count, newMedia);
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
        /// Writes the attributes of a SequenceMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.AllowMultipleMediaTypes, AllowMultipleTypes ? "true" : "false");

        }

        /// <summary>
        /// Write the child elements of a SequenceMedia element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (mSequence.Count > 0)
            {
                destination.WriteStartElement(XukStrings.Sequence, XukAble.XUK_NS);
                for (int i = 0; i < mSequence.Count; i++)
                {
                    mSequence.Get(i).XukOut(destination, baseUri, handler);
                }
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<Media> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            SequenceMedia otherz = other as SequenceMedia;
            if (otherz == null)
            {
                return false;
            }
            if (AllowMultipleTypes != otherz.AllowMultipleTypes)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (otherz.mSequence.Count != mSequence.Count)
            {
                return false;
            }
            for (int i = 0; i < mSequence.Count; i++)
            {
                if (!mSequence.Get(i).ValueEquals(otherz.mSequence.Get(i)))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
            }

            return true;
        }

        #endregion

        public String GetMediaText()
        {
            if (AllowMultipleTypes)
            {
                return null;
            }
            string strSeq = "";
            foreach (Media media in mSequence.ContentsAs_Enumerable)
            {
                if (media is AbstractTextMedia)
                {
                    strSeq += ((AbstractTextMedia)media).Text;
                }
                else
                {
                    break;
                }
            }
            if (strSeq.Length == 0)
            {
                return null;
            }
            return strSeq;
        }

        public Time GetDurationOfManagedAudioMedia()
        {
            if (AllowMultipleTypes)
            {
                return null;
            }

            Time dur = new Time();
            foreach (Media media in mSequence.ContentsAs_Enumerable)
            {
                if (media is ManagedAudioMedia)
                {
                    if (((ManagedAudioMedia)media).HasActualAudioMediaData)
                    {
                        dur.Add(((ManagedAudioMedia)media).Duration);
                    }
                }
                else
                {
                    return null;
                }
            }
            return dur;
        }

        public Stream OpenPcmInputStreamOfManagedAudioMedia()
        {
            if (AllowMultipleTypes)
            {
                return null;
            }

#if USE_NORMAL_LIST
            List
#else
            LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> streams = new
#if USE_NORMAL_LIST
            List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<Stream>();

            foreach (Media media in mSequence.ContentsAs_Enumerable)
            {
                if (media is ManagedAudioMedia)
                {
                    if (((ManagedAudioMedia)media).HasActualAudioMediaData)
                    {
                        Stream stream = ((ManagedAudioMedia)media).AudioMediaData.OpenPcmInputStream();
                        if (stream != null)
                        {
                            streams.Add(stream);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (!streams.IsEmpty)
            {
                return new SequenceStream(streams);
            }
            return null;
        }
    }
        
#endif //ENABLE_SEQ_MEDIA

}