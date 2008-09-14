using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// Factory for creating <see cref="DataProvider"/>s
    /// </summary>
    public class DataProviderFactory : GenericFactory<DataProvider>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal DataProviderFactory()
        {
        }

        /// <summary>
        /// Inistalizes an created instance by assigning it an owning <see cref="Presentation"/>
        /// </summary>
        /// <param name="instance">The instance to initialize</param>
        /// <remarks>
        /// In derived factories, this method can be overridden in order to do additional initialization.
        /// In this case the developer must remember to call <c>base.InitializeInstance(instance)</c>
        /// </remarks>
        protected override void InitializeInstance(DataProvider instance)
        {
            base.InitializeInstance(instance);
            Presentation.DataProviderManager.AddDataProvider(instance);
        }

        /// <summary>
        /// MIME type for MPEG-4 AAC audio
        /// </summary>
        public const string AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";

        /// <summary>
        /// MIME type for MPEG-1/2 Layer III audio (MP3)
        /// </summary>
        public const string AUDIO_MP3_MIME_TYPE = "audio/mpeg";

        /// <summary>
        /// MIME type for linear PCM RIFF WAVE format audio (wav)
        /// </summary>
        public const string AUDIO_WAV_MIME_TYPE = "audio/x-wav";

        /// <summary>
        /// MIME type for JPEG images
        /// </summary>
        public const string IMAGE_JPG_MIME_TYPE = "image/jpeg";

        /// <summary>
        /// MIME type for PNG images
        /// </summary>
        public const string IMAGE_PNG_MIME_TYPE = "image/png";

        /// <summary>
        /// MIME type for Scalable Vector Graphics (SVG) images
        /// </summary>
        public const string IMAGE_SVG_MIME_TYPE = "image/svg+xml";

        /// <summary>
        /// MIME type for Cascading Style Sheets (CSS)
        /// </summary>
        public const string STYLE_CSS_MIME_TYPE = "text/css";

        /// <summary>
        /// MIME type for plain text
        /// </summary>
        public const string TEXT_PLAIN_MIME_TYPE = "text/plain";


        /// <summary>
        /// Gets the file extension for a given MIME type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns>The extension</returns>
        public static string GetExtensionFromMimeType(string mimeType)
        {
            string extension;
            switch (mimeType)
            {
                case AUDIO_MP4_MIME_TYPE:
                    extension = ".mp4";
                    break;
                case AUDIO_MP3_MIME_TYPE:
                    extension = ".mp3";
                    break;
                case AUDIO_WAV_MIME_TYPE:
                    extension = ".wav";
                    break;
                case IMAGE_JPG_MIME_TYPE:
                    extension = ".jpg";
                    break;
                case IMAGE_PNG_MIME_TYPE:
                    extension = ".png";
                    break;
                case IMAGE_SVG_MIME_TYPE:
                    extension = ".svg";
                    break;
                case STYLE_CSS_MIME_TYPE:
                    extension = ".css";
                    break;
                case TEXT_PLAIN_MIME_TYPE:
                    extension = ".txt";
                    break;
                default:
                    extension = ".bin";
                    break;
            }
            return extension;
        }

        /// <summary>
        /// Creates a <see cref="FileDataProvider"/> for the given MIME type
        /// </summary>
        /// <param name="mimeType">The given MIME type</param>
        /// <returns>The created data provider</returns>
        public virtual DataProvider CreateDataProvider(string mimeType)
        {
            return CreateFileDataProvider(mimeType);
        }

        /// <summary>
        /// Creates a <see cref="FileDataProvider"/> for the given MIME type
        /// </summary>
        /// <param name="mimeType">The given MIME type</param>
        /// <returns>The created data provider</returns>
        public FileDataProvider CreateFileDataProvider(string mimeType)
        {
            if (mimeType == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "Can not create a FileDataProvider for a null MIME type");
            }
            FileDataProvider newProv = Create<FileDataProvider>();
            newProv.MimeType = mimeType;
            return newProv;
        }

        /// <summary>
        /// Creates a data provider for the given mime type of type mathcing the given xuk QName
        /// </summary>
        /// <param name="mimeType">The given MIME type</param>
        /// <param name="xukLocalName">The local name part of the given xuk QName</param>
        /// <param name="xukNamespaceUri">The namespace uri part of the given xuk QName</param>
        /// <returns>The created data provider</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="xukLocalName"/> or <paramref name="xukNamespaceUri"/> is <c>null</c>
        /// </exception>
        public virtual DataProvider CreateDataProvider(string mimeType, string xukLocalName, string xukNamespaceUri)
        {
            if (xukLocalName == null || xukNamespaceUri == null)
            {
                throw new exception.MethodParameterIsNullException("No part of the xuk QName can be null");
            }
            if (xukNamespaceUri == XukAble.XUK_NS)
            {
                switch (xukLocalName)
                {
                    case "FileDataProvider":
                        return CreateFileDataProvider(mimeType);
                }
            }
            return null;
        }
    }
}