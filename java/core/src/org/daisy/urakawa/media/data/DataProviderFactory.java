package org.daisy.urakawa.media.data;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.media.data.FileDataProvider
 */
public final class DataProviderFactory extends GenericWithPresentationFactory<FileDataProvider>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public DataProviderFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }
    /**
     * @hidden
     */
    @Override
    protected void initializeInstance(FileDataProvider instance)
    {
        super.initializeInstance(instance);
        try
        {
            instance.initialize(
                    getPresentation().getDataProviderManager()
                            .getNewDataFileRelPath(
                                    getExtensionFromMimeType(mMimeType)),
                    mMimeType);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    // TODO: this can easily be broken during concurrent access !! (must fix)
    private String mMimeType = null;

    /**
     * @param mimeType
     * @return
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public FileDataProvider createFileDataProvider(String mimeType)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        if (mimeType == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (mimeType.length() == 0)
        {
            throw new MethodParameterIsEmptyStringException();
        }
        mMimeType = mimeType;
        FileDataProvider fdp;
        try
        {
            fdp = create(FileDataProvider.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return fdp;
    }

    /**
     * @param mimeType
     * @param xukLocalName
     * @param xukNamespaceURI
     * @return
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public FileDataProvider createFileDataProvider(String mimeType, String xukLocalName,
            String xukNamespaceURI) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException
    {
        mMimeType = mimeType;
        return create(xukLocalName, xukNamespaceURI);
    }

    /**
	 * 
	 */
    public static String AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
    /**
	 * 
	 */
    public static String AUDIO_MP3_MIME_TYPE = "audio/mpeg";
    /**
	 * 
	 */
    public static String AUDIO_WAV_MIME_TYPE = "audio/x-wav";
    /**
	 * 
	 */
    public static String IMAGE_JPG_MIME_TYPE = "image/jpeg";
    /**
	 * 
	 */
    public static String IMAGE_PNG_MIME_TYPE = "image/png";
    /**
	 * 
	 */
    public static String IMAGE_SVG_MIME_TYPE = "image/svg+xml";
    /**
	 * 
	 */
    public static String STYLE_CSS_MIME_TYPE = "text/css";
    /**
	 * 
	 */
    public static String TEXT_PLAIN_MIME_TYPE = "text/plain";

    /**
     * @param mimeType
     * @return
     * @hidden
     */
    public String getExtensionFromMimeType(String mimeType)
    {
        String extension;
        if (mimeType == AUDIO_MP4_MIME_TYPE)
            extension = ".mp4";
        else
            if (mimeType == AUDIO_MP3_MIME_TYPE)
                extension = ".mp3";
            else
                if (mimeType == AUDIO_WAV_MIME_TYPE)
                    extension = ".wav";
                else
                    if (mimeType == IMAGE_JPG_MIME_TYPE)
                        extension = ".jpg";
                    else
                        if (mimeType == IMAGE_PNG_MIME_TYPE)
                            extension = ".png";
                        else
                            if (mimeType == IMAGE_SVG_MIME_TYPE)
                                extension = ".svg";
                            else
                                if (mimeType == STYLE_CSS_MIME_TYPE)
                                    extension = ".css";
                                else
                                    if (mimeType == TEXT_PLAIN_MIME_TYPE)
                                        extension = ".txt";
                                    else
                                        extension = ".bin";
        return extension;
    }
}
