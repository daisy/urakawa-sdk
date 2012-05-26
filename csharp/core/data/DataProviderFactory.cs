using System;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.data
{
    /// <summary>
    /// Factory for creating <see cref="DataProvider"/>s
    /// </summary>
    public sealed class DataProviderFactory : GenericWithPresentationFactory<DataProvider>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.DataProviderFactory;
        }
        public DataProviderFactory(Presentation pres) : base(pres)
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
            if (m_skipManagerInitialization)
            {
                m_skipManagerInitialization = false;
                return;
            }
            Presentation.DataProviderManager.AddManagedObject(instance);
        }

        private bool m_skipManagerInitialization = false;
        public DataProvider Create_SkipManagerInitialization(string mimeType, string xukLN, string xukNS)
        {
            m_skipManagerInitialization = true;
            return Create(mimeType, xukLN, xukNS);
        }

        /// <summary>
        /// MIME type for MPEG-1/2 Layer III audio (MP3)
        /// </summary>
        public const string AUDIO_MP3_MIME_TYPE = "audio/mpeg";
        public const string AUDIO_MP3_EXTENSION = ".mp3";

        /// <summary>
        /// MIME type for linear PCM RIFF WAVE format audio (wav)
        /// </summary>
        public const string AUDIO_WAV_MIME_TYPE = "audio/x-wav";
        public const string AUDIO_WAV_EXTENSION = ".wav";

        /// <summary>
        /// MIME type for JPEG images
        /// </summary>
        public const string IMAGE_JPG_MIME_TYPE = "image/jpeg";
        public const string IMAGE_JPG_EXTENSION = ".jpg";
        public const string IMAGE_JPEG_EXTENSION = ".jpeg";

        /// <summary>
        /// MIME type for PNG images
        /// </summary>
        public const string IMAGE_PNG_MIME_TYPE = "image/png";
        public const string IMAGE_PNG_EXTENSION = ".png";

        /// <summary>
        /// MIME type for BMP images
        /// </summary>
        public const string IMAGE_BMP_MIME_TYPE = "image/bmp";
        public const string IMAGE_BMP_EXTENSION = ".bmp";


        /// <summary>
        /// MIME type for GIF images
        /// </summary>
        public const string IMAGE_GIF_MIME_TYPE = "image/gif";
        public const string IMAGE_GIF_EXTENSION = ".gif";


        /// <summary>
        /// MIME type for WMV videos
        /// </summary>
        public const string VIDEO_WMV_MIME_TYPE = "video/wmv";
        public const string VIDEO_WMV_EXTENSION = ".wmv";

        /// <summary>
        /// MIME type for MOV videos
        /// </summary>
        public const string VIDEO_MOV_MIME_TYPE = "video/mov";
        public const string VIDEO_MOV_EXTENSION = ".mov";

        /// <summary>
        /// MIME type for AVI videos
        /// </summary>
        public const string VIDEO_AVI_MIME_TYPE = "video/avi";
        public const string VIDEO_AVI_EXTENSION = ".avi";

        /// <summary>
        /// MIME type for MPEG videos
        /// </summary>
        public const string VIDEO_MPG_MIME_TYPE = "video/mpg";
        public const string VIDEO_MPG_EXTENSION = ".mpg";
        public const string VIDEO_MPEG_EXTENSION = ".mpeg";

        public const string VIDEO_MP4_MIME_TYPE = "video/mp4";
        public const string VIDEO_MP4_EXTENSION = ".mp4";

        //public const string VIDEO_M4V_EXTENSION = ".m4v";
        //public const string VIDEO_M2V_EXTENSION = ".m2v";
        //public const string VIDEO_3GP_EXTENSION = ".3gp";


        public const string VIDEO_OGG_MIME_TYPE = "video/ogg";
        public const string VIDEO_OGG_EXTENSION = ".ogv";

        public const string VIDEO_WEBM_MIME_TYPE = "video/webm";
        public const string VIDEO_WEBM_EXTENSION = ".webm";

        /// <summary>
        /// MIME type for Scalable Vector Graphics (SVG) images
        /// </summary>
        public const string IMAGE_SVG_MIME_TYPE = "image/svg+xml";
        public const string IMAGE_SVG_EXTENSION = ".svg";
        public const string IMAGE_SVGZ_EXTENSION = ".svgz";

        /// <summary>
        /// MIME type for Cascading Style Sheets (CSS)
        /// </summary>
        public const string STYLE_CSS_MIME_TYPE = "text/css";
        public const string STYLE_CSS_EXTENSION = ".css";

        /// <summary>
        /// MIME type for (XSLT)
        /// </summary>
        public const string STYLE_XSLT_MIME_TYPE = "text/xsl" ;
        public const string STYLE_XSLT_EXTENSION = ".xslt";
        public const string STYLE_XSL_EXTENSION = ".xsl";

/// <summary>
        /// MIME type for Pronunciation Lexicon Specification (PLS)
        /// </summary>
        public const string STYLE_PLS_MIME_TYPE = "application/pls+xml";
        public const string STYLE_PLS_EXTENSION = ".pls";

        /// <summary>
        /// MIME type for plain text
        /// </summary>
        public const string TEXT_PLAIN_MIME_TYPE = "text/plain";
        public const string TEXT_PLAIN_EXTENSION = ".txt";

        /// <summary>
        /// MIME type for DTD
        /// </summary>
        public const string DTD_MIME_TYPE = "application/xml-dtd";
        public const string DTD_EXTENSION = ".dtd";

        /// <summary>
        /// MIME type for xml
        /// </summary>
        public const string XML_TEXT_MIME_TYPE = "text/xml";
        public const string XML_TEXT_EXTENSION = ".xml";


        public static string GetExtensionFromMimeType(string mimeType)
        {
            string extension;
            switch (mimeType)
            {
                case VIDEO_AVI_MIME_TYPE:
                    extension = VIDEO_AVI_EXTENSION;
                    break;
                case VIDEO_MOV_MIME_TYPE:
                    extension = VIDEO_MOV_EXTENSION;
                    break;
                case VIDEO_MPG_MIME_TYPE:
                    extension = VIDEO_MPG_EXTENSION;
                    break;
                case VIDEO_MP4_MIME_TYPE:
                    extension = VIDEO_MP4_EXTENSION;
                    break;
                case VIDEO_WMV_MIME_TYPE:
                    extension = VIDEO_WMV_EXTENSION;
                    break;

                case VIDEO_OGG_MIME_TYPE:
                    extension = VIDEO_OGG_EXTENSION;
                    break;

                case VIDEO_WEBM_MIME_TYPE:
                    extension = VIDEO_WEBM_EXTENSION;
                    break;


                case AUDIO_MP3_MIME_TYPE:
                    extension = AUDIO_MP3_EXTENSION;
                    break;
                case AUDIO_WAV_MIME_TYPE:
                    extension = AUDIO_WAV_EXTENSION;
                    break;
                case IMAGE_JPG_MIME_TYPE:
                    extension = IMAGE_JPG_EXTENSION;
                    break;
                case IMAGE_PNG_MIME_TYPE:
                    extension = IMAGE_PNG_EXTENSION;
                    break;
                case IMAGE_BMP_MIME_TYPE:
                    extension = IMAGE_BMP_EXTENSION;
                    break;
                case IMAGE_GIF_MIME_TYPE:
                    extension = IMAGE_GIF_EXTENSION;
                    break;
                case IMAGE_SVG_MIME_TYPE:
                    extension = IMAGE_SVG_EXTENSION;
                    break;
                case STYLE_CSS_MIME_TYPE:
                    extension = STYLE_CSS_EXTENSION;
                    break;
                case STYLE_XSLT_MIME_TYPE:
                    extension = STYLE_XSLT_EXTENSION;
                    break;
                case STYLE_PLS_MIME_TYPE:
                    extension = STYLE_PLS_EXTENSION;
                    break;
                case TEXT_PLAIN_MIME_TYPE:
                    extension = TEXT_PLAIN_EXTENSION;
                    break;
                case DTD_MIME_TYPE:
                    extension = DTD_EXTENSION;
                    break;
                case XML_TEXT_MIME_TYPE:
                    extension = XML_TEXT_EXTENSION;
                    break;
                default:
                    extension = ".bin";
                    break;
            }
            return extension;
        }


        public static string GetMimeTypeFromExtension(string extension)
        {
            string mime;
            switch (extension)
            {
                case VIDEO_MP4_EXTENSION:
                    mime = VIDEO_MP4_MIME_TYPE;
                    break;
                case VIDEO_MPG_EXTENSION:
                case VIDEO_MPEG_EXTENSION:
                //case VIDEO_M2V_EXTENSION:
                //case VIDEO_M4V_EXTENSION:
                //case VIDEO_3GP_EXTENSION:
                    mime = VIDEO_MPG_MIME_TYPE;
                    break;
                case VIDEO_AVI_EXTENSION:
                    mime = VIDEO_AVI_MIME_TYPE;
                    break;
                case VIDEO_OGG_EXTENSION:
                    mime = VIDEO_OGG_MIME_TYPE;
                    break;
                case VIDEO_WEBM_EXTENSION:
                    mime = VIDEO_WEBM_MIME_TYPE;
                    break;
                case VIDEO_WMV_EXTENSION:
                    mime = VIDEO_WMV_MIME_TYPE;
                    break;
                case VIDEO_MOV_EXTENSION:
                    mime = VIDEO_MOV_MIME_TYPE;
                    break;



                case AUDIO_MP3_EXTENSION:
                    mime = AUDIO_MP3_MIME_TYPE;
                    break;
                case AUDIO_WAV_EXTENSION:
                    mime = AUDIO_WAV_MIME_TYPE;
                    break;
                case IMAGE_JPG_EXTENSION:
                case IMAGE_JPEG_EXTENSION:
                    mime = IMAGE_JPG_MIME_TYPE;
                    break;
                case IMAGE_PNG_EXTENSION:
                    mime = IMAGE_PNG_MIME_TYPE;
                    break;
                case IMAGE_BMP_EXTENSION:
                    mime = IMAGE_BMP_MIME_TYPE;
                    break;
                case IMAGE_GIF_EXTENSION:
                    mime = IMAGE_GIF_MIME_TYPE;
                    break;
                case IMAGE_SVG_EXTENSION:
                case IMAGE_SVGZ_EXTENSION:
                    mime = IMAGE_SVG_MIME_TYPE;
                    break;
                case STYLE_CSS_EXTENSION:
                    mime = STYLE_CSS_MIME_TYPE;
                    break;
                case STYLE_XSLT_EXTENSION:
                    mime = STYLE_XSLT_MIME_TYPE;
                    break;
                case STYLE_PLS_EXTENSION:
                    mime = STYLE_PLS_MIME_TYPE;
                    break;
                case TEXT_PLAIN_EXTENSION:
                    mime = TEXT_PLAIN_MIME_TYPE;
                    break;
                case DTD_EXTENSION:
                    mime = DTD_MIME_TYPE;
                    break;
                case XML_TEXT_EXTENSION:
                    mime = XML_TEXT_MIME_TYPE;
                    break;
                default:
                    mime = "N/A";
                    break;
            }
            return mime;
        }


        private Type mDefaultDataProviderType = typeof (FileDataProvider);

        /// <summary>
        /// Gets or sets the default <see cref="DataProvider"/> <see cref="Type"/>
        /// </summary>
        /// <exception cref="MethodParameterIsNullException">
        /// Thrown when trying to set to <c>null</c>
        /// </exception>
        /// <exception cref="MethodParameterIsWrongTypeException">
        /// Thrown when trying to set to a <see cref="Type"/> that:
        /// <list type="ol">
        /// <item>Does not implement <see cref="DataProvider"/></item>
        /// <item>Is abstract</item>
        /// <item>Does npot have a default constructor</item>
        /// </list>
        /// </exception>
        public Type DefaultDataProviderType
        {
            get { return mDefaultDataProviderType; }
            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The default DataProvider Type cannot be null");
                }
                if (!(typeof(DataProvider).IsAssignableFrom(value)))
                {
                    throw new MethodParameterIsWrongTypeException("The default DataProvider Type must be a subclass of AudioMediaData");
                }
                if (value.IsAbstract)
                {
                    throw new MethodParameterIsWrongTypeException("The default DataProvider Type cannot be an abstract class");
                }
                if (value.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new MethodParameterIsWrongTypeException("The default DataProvider Type must have a default constructor");
                }
                mDefaultDataProviderType = value;
            }
        }

        /// <summary>
        /// Creates a new <typeparamref name="T"/>, assigning it a given MIME type
        /// </summary>
        /// <typeparam name="T">The subtype of <see cref="DataProvider"/> to create an instance of</typeparam>
        /// <param name="mimeType">The MIME type to assign to the created <see cref="DataProvider"/> instance</param>
        /// <returns>The created instance</returns>
        public T Create<T>(string mimeType) where T : DataProvider, new()
        {
            T newProv = Create<T>();
            newProv.MimeType = mimeType;
            return newProv;
        }

        /// <summary>
        /// Creates a <see cref="FileDataProvider"/> for the given MIME type
        /// </summary>
        /// <param name="mimeType">The MIME type to assign to the created <see cref="DataProvider"/> instance</param>
        /// <returns>The created data provider</returns>
        public DataProvider Create(string mimeType)
        {
            DataProvider newProv = Create(DefaultDataProviderType);
            newProv.MimeType = mimeType;
            return newProv;
        }

        /// <summary>
        /// Creates a <see cref="DataProvider"/> instance of a given <see cref="Type"/>, 
        /// assigning it a given MIME type
        /// </summary>
        /// <param name="t">
        /// <param name="mimeType">The MIME type to assign to the created <see cref="DataProvider"/> instance</param>
        /// The <see cref="Type"/> of the instance to create,
        /// cannot be null and must implement <see cref="DataProvider"/> and
        /// and have a public constructor with no arguments
        /// </param>
        /// <returns>
        /// The created instance - <c>null</c> if <paramref name="t"/> is not a <see cref="DataProvider"/>
        /// or if <paramref name="t"/> has no default public constructor 
        /// </returns>
        public DataProvider Create(Type t, string mimeType)
        {
            DataProvider newProv = Create(t);
            if (newProv != null) newProv.MimeType = mimeType;
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
        public DataProvider Create(string mimeType, string xukLocalName, string xukNamespaceUri)
        {
            DataProvider newProv = Create(xukLocalName, xukNamespaceUri);
            if (newProv!=null) newProv.MimeType = mimeType;
            return newProv;
        }
    }
}