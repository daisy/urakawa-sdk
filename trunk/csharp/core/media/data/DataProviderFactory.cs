using System;
using urakawa.exception;

namespace urakawa.media.data
{
    /// <summary>
    /// Factory for creating <see cref="DataProvider"/>s
    /// </summary>
    public sealed class DataProviderFactory : GenericWithPresentationFactory<DataProvider>
    {
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
        /// Creates a <see cref="FileDataProvider"/> of default <see cref="Type"/>
        /// </summary>
        /// <returns>The created data provider</returns>
        public DataProvider Create()
        {
            return Create(DefaultDataProviderType);
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
            DataProvider newProv = Create(xukLocalName, XukNamespaceUri);
            if (newProv!=null) newProv.MimeType = mimeType;
            return newProv;
        }
    }
}