using System;
using System.Diagnostics;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.data
{
    [XukNameUglyPrettyAttribute("dtPrvFct", "DataProviderFactory")]
    public sealed class DataProviderFactory : GenericWithPresentationFactory<DataProvider>
    {
        
        public DataProviderFactory(Presentation pres)
            : base(pres)
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

        public const string AUDIO_MP4_MIME_TYPE = "audio/mp4";
        public const string AUDIO_MP4_EXTENSION_ = ".mp4";
        public const string AUDIO_MP4_EXTENSION = ".m4a";


        public const string AUDIO_OGG_MIME_TYPE = "audio/ogg";
        public const string AUDIO_OGG_EXTENSION = ".ogg";

        public const string AUDIO_AMR_MIME_TYPE = "audio/amr";
        public const string AUDIO_AMR_EXTENSION = ".amr";

        public const string AUDIO_3GPP_MIME_TYPE = "audio/3gpp";
        public const string AUDIO_3GPP_EXTENSION = ".3gp";

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


        public const string RDF_MIME_TYPE = "application/rdf+xml";
        public const string RDF_EXTENSION = ".xmp";

        public const string FONT_TTF_MIME_TYPE = "application/vnd.ms-opentype";
        public const string FONT_TTF_MIME_TYPE_ = "application/x-font-ttf";
        public const string FONT_TTF_EXTENSION = ".ttf";

        public const string FONT_OTF_MIME_TYPE = FONT_TTF_MIME_TYPE;
        public const string FONT_OTF_MIME_TYPE_ = "application/x-font-otf";
        public const string FONT_OTF_EXTENSION = ".otf";

        public const string FONT_WOFF_MIME_TYPE = "application/font-woff";
        public const string FONT_WOFF_MIME_TYPE_ = "application/x-font-woff";
        public const string FONT_WOFF_EXTENSION = ".woff";

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
        public const string CSS_MIME_TYPE = "text/css";
        public const string CSS_EXTENSION = ".css";

        /// <summary>
        /// MIME type for (XSLT)
        /// </summary>
        public const string XSLT_MIME_TYPE = "text/xsl";
        public const string XSLT_MIME_TYPE_ = "application/xml";
        public const string XSLT_EXTENSION = ".xslt";
        public const string XSL_EXTENSION = ".xsl";

        /// <summary>
        /// MIME type for Pronunciation Lexicon Specification (PLS)
        /// </summary>
        public const string PLS_MIME_TYPE = "application/pls+xml";
        public const string PLS_EXTENSION = ".pls";

        /// <summary>
        /// MIME type for Navigation Control file for Xml(NCX)
        /// </summary>
        public const string NCX_MIME_TYPE = "application/x-dtbncx+xml";
        public const string NCX_EXTENSION = ".ncx";

        public const string DTBOOK_MIME_TYPE = "application/x-dtbook+xml";
        public const string DTBOOK_EXTENSION = ".dtbook";

        public const string DTB_RES_MIME_TYPE = "application/x-dtbresource+xml";
        public const string DTB_RES_EXTENSION = ".res";


        /// <summary>
        /// MIME type for Java Script
        /// </summary>
        public const string JS_MIME_TYPE = "application/javascript";
        public const string JS_EXTENSION = ".js";


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

        public const string VTT_MIME_TYPE = "text/vtt";
        public const string VTT_EXTENSION = ".vtt";

        public const string TTML_MIME_TYPE = "application/ttml+xml";
        public const string TTML_EXTENSION = ".ttml";

        public const string EPUB_MIME_TYPE = "application/epub+zip";
        public const string EPUB_EXTENSION = ".epub";

        public const string XML_MIME_TYPE = "text/xml";
        public const string XML_EXTENSION = ".xml";

        public const string XHTML_MIME_TYPE = "application/xhtml+xml";
        public const string XHTML_EXTENSION = ".xhtml";
        public const string HTML_EXTENSION = ".html";

        public const string SMIL_MIME_TYPE = "application/smil+xml";
        public const string SMIL_MIME_TYPE_ = "application/smil";
        public const string SMIL_EXTENSION = ".smil";

        public const string ICO_MIME_TYPE = "image/vnd.microsoft.icon";
        public const string ICO_EXTENSION = ".ico";

        //public const string HTML_MIME_TYPE = "text/html";
        //public const string HTML_EXTENSION = ".html";


        public static string GetExtensionFromMimeType(string mimeType)
        {
            if (mimeType != null) mimeType = mimeType.ToLowerInvariant();

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

                case DTB_RES_MIME_TYPE:
                    extension = DTB_RES_EXTENSION;
                    break;


                case AUDIO_MP3_MIME_TYPE:
                    extension = AUDIO_MP3_EXTENSION;
                    break;
                case AUDIO_MP4_MIME_TYPE:
                    extension = AUDIO_MP4_EXTENSION;
                    break;
                case AUDIO_OGG_MIME_TYPE:
                    extension = AUDIO_OGG_EXTENSION;
                    break;
                case AUDIO_WAV_MIME_TYPE:
                    extension = AUDIO_WAV_EXTENSION;
                    break;
                case AUDIO_3GPP_MIME_TYPE:
                    extension = AUDIO_3GPP_EXTENSION;
                    break;
                case AUDIO_AMR_MIME_TYPE:
                    extension = AUDIO_AMR_EXTENSION;
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

                case RDF_MIME_TYPE:
                    extension = RDF_EXTENSION;
                    break;

                case DTBOOK_MIME_TYPE:
                    //extension = DTBOOK_EXTENSION;
                    extension = XML_EXTENSION;
                    break;

                case FONT_TTF_MIME_TYPE:
                case FONT_TTF_MIME_TYPE_:
                    extension = FONT_TTF_EXTENSION;
                    break;

                //case FONT_OTF_MIME_TYPE: ==> same as FONT_TTF_MIME_TYPE
                case FONT_OTF_MIME_TYPE_:
                    extension = FONT_OTF_EXTENSION;
                    break;

                case FONT_WOFF_MIME_TYPE:
                case FONT_WOFF_MIME_TYPE_:
                    extension = FONT_WOFF_EXTENSION;
                    break;

                case CSS_MIME_TYPE:
                    extension = CSS_EXTENSION;
                    break;
                case XSLT_MIME_TYPE:
                    extension = XSLT_EXTENSION;
                    break;
                case PLS_MIME_TYPE:
                    extension = PLS_EXTENSION;
                    break;
                case TEXT_PLAIN_MIME_TYPE:
                    extension = TEXT_PLAIN_EXTENSION;
                    break;
                case DTD_MIME_TYPE:
                    extension = DTD_EXTENSION;
                    break;
                case XML_MIME_TYPE:
                    extension = XML_EXTENSION;
                    break;
                case VTT_MIME_TYPE:
                    extension = VTT_EXTENSION;
                    break;
                case TTML_MIME_TYPE:
                    extension = TTML_EXTENSION;
                    break;
                case NCX_MIME_TYPE:
                    extension = NCX_EXTENSION;
                    break;
                case JS_MIME_TYPE:
                    extension = JS_EXTENSION;
                    break;
                case XHTML_MIME_TYPE:
                    extension = XHTML_EXTENSION;
                    break;
                case SMIL_MIME_TYPE:
                case SMIL_MIME_TYPE_:
                    extension = SMIL_EXTENSION;
                    break;
                case ICO_MIME_TYPE:
                    extension = ICO_EXTENSION;
                    break;
                case EPUB_MIME_TYPE:
                    extension = EPUB_EXTENSION;
                    break;
                default:
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    extension = ".bin";
                    break;
            }
            return extension;
        }


        public static string GetMimeTypeFromExtension(string extension)
        {
            if (extension != null) extension = extension.ToLowerInvariant();

            string mime;
            switch (extension)
            {
                //case VIDEO_MP4_EXTENSION:
                //    mime = VIDEO_MP4_MIME_TYPE;
                //    break;
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

                case DTBOOK_EXTENSION:
                    mime = DTBOOK_MIME_TYPE;
                    break;

                case DTB_RES_EXTENSION:
                    mime = DTB_RES_MIME_TYPE;
                    break;



                case AUDIO_MP3_EXTENSION:
                    mime = AUDIO_MP3_MIME_TYPE;
                    break;
                case AUDIO_MP4_EXTENSION:
                case AUDIO_MP4_EXTENSION_:
                    mime = AUDIO_MP4_MIME_TYPE;
                    break;
                case AUDIO_OGG_EXTENSION:
                    mime = AUDIO_OGG_MIME_TYPE;
                    break;
                case AUDIO_WAV_EXTENSION:
                    mime = AUDIO_WAV_MIME_TYPE;
                    break;
                case AUDIO_3GPP_EXTENSION:
                    mime = AUDIO_3GPP_MIME_TYPE;
                    break;
                case AUDIO_AMR_EXTENSION:
                    mime = AUDIO_AMR_MIME_TYPE;
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

                case RDF_EXTENSION:
                    mime = RDF_MIME_TYPE;
                    break;

                case FONT_TTF_EXTENSION:
                    mime = FONT_TTF_MIME_TYPE;
                    break;

                case FONT_OTF_EXTENSION:
                    mime = FONT_OTF_MIME_TYPE;
                    break;

                case FONT_WOFF_EXTENSION:
                    mime = FONT_WOFF_MIME_TYPE;
                    break;

                case CSS_EXTENSION:
                    mime = CSS_MIME_TYPE;
                    break;
                case XSLT_EXTENSION:
                    mime = XSLT_MIME_TYPE;
                    break;
                case XSL_EXTENSION:
                    mime = XSLT_MIME_TYPE;
                    break;
                case PLS_EXTENSION:
                    mime = PLS_MIME_TYPE;
                    break;
                case TEXT_PLAIN_EXTENSION:
                    mime = TEXT_PLAIN_MIME_TYPE;
                    break;
                case DTD_EXTENSION:
                    mime = DTD_MIME_TYPE;
                    break;
                case XML_EXTENSION:
                    mime = XML_MIME_TYPE;
                    break;
                case VTT_EXTENSION:
                    mime = VTT_MIME_TYPE;
                    break;
                case TTML_EXTENSION:
                    mime = TTML_MIME_TYPE;
                    break;
                case NCX_EXTENSION:
                    mime = NCX_MIME_TYPE;
                    break;
                case JS_EXTENSION:
                    mime = JS_MIME_TYPE;
                    break;
                case XHTML_EXTENSION:
                case HTML_EXTENSION:
                    mime = XHTML_MIME_TYPE;
                    break;
                case SMIL_EXTENSION:
                    mime = SMIL_MIME_TYPE;
                    break;
                case ICO_EXTENSION:
                    mime = ICO_MIME_TYPE;
                    break;
                case EPUB_EXTENSION:
                    mime = EPUB_MIME_TYPE;
                    break;
                default:
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    mime = "N/A";
                    break;
            }
            return mime;
        }


        private Type mDefaultDataProviderType = typeof(FileDataProvider);

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
            if (newProv != null) newProv.MimeType = mimeType;
            return newProv;
        }
    }
}