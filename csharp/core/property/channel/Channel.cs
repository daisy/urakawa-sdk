using System;
using System.Xml;
using urakawa.exception;
using urakawa.media;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property.channel
{
    /// <summary>
    /// A <see cref="Channel"/> is used to associate <see cref="Media"/> 
    /// with <see cref="core.TreeNode"/>s via <see cref="ChannelsProperty"/>
    /// </summary>
    public class Channel : WithPresentation, IValueEquatable<Channel>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.Channel;
        }
        private string mName = "";
        private string mLanguage = null;

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> managing the <see cref="Channel"/>
        /// </summary>
        public ChannelsManager ChannelsManager
        {
            get
            {
                return Presentation.ChannelsManager;
            }
        }

        /// <summary>
        /// Determines if the channel is equivalent to a given other channel, 
        /// possibly from another <see cref="Presentation"/>
        /// </summary>
        /// <param name="otherChannel">The given other channel</param>
        /// <returns>A <see cref="bool"/> indicating equivalence</returns>
        public virtual bool IsEquivalentTo(Channel otherChannel)
        {
            if (otherChannel == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not test for equivalence with a null Channel");
            }
            if (this.GetType() != otherChannel.GetType()) return false;
            if (this.Name != otherChannel.Name) return false;
            if (this.Language != otherChannel.Language) return false;
            return true;
        }

        /// <summary>
        /// Exports the channel to a destination <see cref="Presentation"/>.
        /// The exported channels has the same name
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported channel</returns>
        public Channel Export(Presentation destPres)
        {
            return ExportProtected(destPres);
        }

        /// <summary>
        /// Exports the channel to a destination <see cref="Presentation"/>.
        /// The exported channels has the same name.
        /// (protected virtual method, called by public <see cref="Export"/> method)
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported channel</returns>
        /// <remarks>
        /// In derived classes, this method should be overridden. 
        /// If one wants the copy method to return the correct sub-type,
        /// override <see cref="Export"/> with the <c>new</c> keyword, making it return <see cref="ExportProtected"/>
        /// </remarks>
        protected virtual Channel ExportProtected(Presentation destPres)
        {
            Channel exportedCh = destPres.ChannelFactory.Create(GetType());
            if (exportedCh == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The ChannelsFacotry of the destination Presentation can not create a Channel matching Xuk QName {0}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            exportedCh.Name = Name;
            exportedCh.Language = Language;
            return exportedCh;
        }
        public Channel Copy()
        {
            return CopyProtected();
        }
        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected virtual Channel CopyProtected()
        {
            Channel copy = Presentation.ChannelFactory.Create(GetType());
            if (copy == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The ChannelsFacotry of the destination Presentation can not create a Channel matching Xuk QName {0}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            copy.Name = Name;
            copy.Language = Language;
            return copy;
        }

        /// <summary>
        /// Gets the human-readable name of the <see cref="Channel"/>
        /// </summary>
        /// <returns>The human-readable name</returns>
        public string Name
        {
            get { return mName; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "Can not set channel name to null");
                }
                mName = value;
            }
        }

        /// <summary>
        /// Gets the language of the channel
        /// </summary>
        /// <returns>The language</returns>
        public string Language
        {
            get { return mLanguage; }
            set
            {
                if (value == "")
                {
                    throw new exception.MethodParameterIsEmptyStringException(
                        "Can not set channel language to an empty string");
                }
                mLanguage = value;
            }
        }

        /// <summary>
        /// Checks of a given <see cref="Media"/> is accepted by the channel
        /// </summary>
        /// <param name="m">The <see cref="Media"/></param>
        /// <returns>
        /// A <see cref="bool"/> indicating if the <see cref="Media"/> is accpetable
        /// </returns>
        public virtual bool CanAccept(Media m)
        {
            return true;
        }

        /// <summary>
        /// Gets the uid of the <see cref="Channel"/>
        /// </summary>
        /// <returns>The Xuk Uid as calculated by 
        /// <c>this.getChannelsManager.GetUidOfChannel(this)</c></returns>
        public string Uid
        {
            get { return ChannelsManager.GetUidOfChannel(this); }
        }

        #region IXUKAble members

        ///// <summary>
        ///// Reads the <see cref="Channel"/> from a Channel xuk element
        ///// </summary>
        ///// <param name="source">The source <see cref="XmlReader"/></param>
        ///// <param name="handler">The handler for progress</param>
        //public void XukIn(XmlReader source, ProgressHandler handler)
        //{
        //    if (source == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
        //    }
        //    if (source.NodeType != XmlNodeType.Element)
        //    {
        //        throw new exception.XukException("Can not read Channel from a non-element node");
        //    }
        //    try
        //    {
        //        XukInAttributes(source);
        //        if (!source.IsEmptyElement)
        //        {
        //            while (source.Read())
        //            {
        //                if (source.NodeType == XmlNodeType.Element)
        //                {
        //                    XukInChild(source);
        //                }
        //                else if (source.NodeType == XmlNodeType.EndElement)
        //                {
        //                    break;
        //                }
        //                if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
        //            }
        //        }

        //    }
        //    catch (exception.XukException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new exception.XukException(
        //            String.Format("An exception occured during XukIn of Channel: {0}", e.Message),
        //            e);
        //    }
        //}

        /// <summary>
        /// Reads the attributes of a Channel xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string name = source.GetAttribute(XukStrings.Name);
            if (name == null) name = "";
            Name = name;
            string lang = source.GetAttribute(XukStrings.Language);
            if (lang != null) lang = lang.Trim();
            if (lang == "") lang = null;
            Language = lang;
            base.XukInAttributes(source);
        }

        ///// <summary>
        ///// Write a Channel element to a XUK file representing the <see cref="Channel"/> instance
        ///// </summary>
        ///// <param name="destination">The destination <see cref="XmlWriter"/></param>
        ///// <param name="baseUri">
        ///// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        ///// if <c>null</c> absolute <see cref="Uri"/>s are written
        ///// </param>
        //public void XukOut(XmlWriter destination, Uri baseUri)
        //{
        //    if (destination == null)
        //    {
        //        throw new exception.MethodParameterIsNullException(
        //            "Can not XukOut to a null XmlWriter");
        //    }

        //    try
        //    {
        //        destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
        //        XukOutAttributes(destination, baseUri);
        //        XukOutChildren(destination, baseUri);
        //        destination.WriteEndElement();

        //    }
        //    catch (exception.XukException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new exception.XukException(
        //            String.Format("An exception occured during XukOut of Channel: {0}", e.Message),
        //            e);
        //    }
        //}


        /// <summary>
        /// Writes the attributes of a Channel element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString(XukStrings.Name, Name);
            if (!String.IsNullOrEmpty(Language))
            {
                destination.WriteAttributeString(XukStrings.Language, Language);
            }
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<Channel> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public virtual bool ValueEquals(Channel other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (GetType() != other.GetType())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (Name != other.Name)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (Language != other.Language)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

        #endregion
    }
}