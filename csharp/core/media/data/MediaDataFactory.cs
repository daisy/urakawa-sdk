using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// <para>Factory for creating <see cref="MediaData"/>.</para>
    /// <para>Supports creation of the following <see cref="MediaData"/> types:
    /// <list type="ul">
    /// <item><see cref="audio.codec.WavAudioMediaData"/></item>
    /// </list>
    /// </para>
    /// </summary>
    public class MediaDataFactory : WithPresentation, IXukAble
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal MediaDataFactory()
        {
        }

        /// <summary>
        /// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
        /// (via the <see cref="Presentation"/> associated with <c>this</c>.
        /// Convenience for <c>getPresentation().getMediaDataManager()</c>
        /// </summary>
        /// <returns>The <see cref="MediaDataManager"/></returns>
        public MediaDataManager MediaDataManager
        {
            get { return Presentation.MediaDataManager; }
        }

        /// <summary>
        /// Creates an instance of a <see cref="MediaData"/> of type matching a given XUK QName
        /// </summary>
        /// <param name="xukLocalName">The local name part of the QName</param>
        /// <param name="xukNamespaceUri">The namespace uri part of the QName</param>
        /// <returns>The created <see cref="MediaData"/> instance or <c>null</c> if the given QName is supported</returns>
        public virtual MediaData CreateMediaData(string xukLocalName, string xukNamespaceUri)
        {
            if (xukLocalName == null || xukNamespaceUri == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "No part of the QName can be null");
            }
            if (xukNamespaceUri == XukAble.XUK_NS)
            {
                switch (xukLocalName)
                {
                    case "WavAudioMediaData":
                        return CreateWavAudioMediaData();
                    default:
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a <see cref="MediaData"/> instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="mt">The given <see cref="Type"/></param>
        /// <returns>
        /// The created <see cref="MediaData"/> instance 
        /// or <c>null</c> if the given media <see cref="Type"/> is not supported
        /// </returns>
        public virtual MediaData CreateMediaData(Type mt)
        {
            MediaData res = CreateMediaData(mt.Name, XukAble.XUK_NS);
            if (res != null)
            {
                if (res.GetType() == mt) return res;
            }
            if (typeof (AudioMediaData).IsAssignableFrom(mt))
            {
                return CreateWavAudioMediaData();
            }
            return null;
        }

        /// <summary>
        /// Creates a <see cref="AudioMediaData"/> of default type (which is <see cref="WavAudioMediaData"/>)
        /// </summary>
        /// <returns>The created <see cref="WavAudioMediaData"/></returns>
        public virtual AudioMediaData CreateAudioMediaData()
        {
            return CreateWavAudioMediaData();
        }

        /// <summary>
        /// Creates a <see cref="WavAudioMediaData"/>
        /// </summary>
        /// <returns>The created <see cref="WavAudioMediaData"/></returns>
        public WavAudioMediaData CreateWavAudioMediaData()
        {
            WavAudioMediaData res = new WavAudioMediaData();
            res.Presentation = Presentation;
            MediaDataManager.AddMediaData(res);
            return res;
        }
    }
}